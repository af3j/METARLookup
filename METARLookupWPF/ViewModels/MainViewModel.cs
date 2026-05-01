using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METARLookupWPF.Models;
using METARLookupWPF.Services;

namespace METARLookupWPF.ViewModels;

/// <summary>
/// Root view-model for <c>MainWindow</c>. Owns the search bar, favorites strip,
/// auto-refresh timer, and tab coordination. Child view-models are injected and
/// exposed as read-only properties for binding in the sub-views.
/// Uses the CommunityToolkit.Mvvm source generator: [ObservableProperty] generates
/// the backing field, property, and INotifyPropertyChanged call automatically.
/// </summary>
public partial class MainViewModel(
    IAvWeatherService weatherService,
    IAirportService airportService,
    IAtisService atisService,
    IAirportSearchService airportSearchService,
    MetarViewModel metarVm,
    TafViewModel tafVm,
    SigmetViewModel sigmetVm,
    ChartsViewModel chartsVm,
    CalculatorsViewModel calculatorsVm,
    IUserSettingsService settingsService) : ObservableObject
{
    // ── App metadata ─────────────────────────────────────────────────────────

    public string AppVersionString { get; } =
        Assembly.GetExecutingAssembly().GetName().Version is { } v
            ? $"METAR Lookup v{v.ToString(v.Revision > 0 ? 4 : v.Build > 0 ? 3 : 2)} • Farrand Tech Services - farrandtech.com • Data: aviationweather.gov"
            : "METAR Lookup • Farrand Tech Services - farrandtech.com • Data: aviationweather.gov";

    // ── Bindable state ────────────────────────────────────────────────────────

    /// <summary>Text currently typed in the ICAO search box.</summary>
    [ObservableProperty] private string _searchText = string.Empty;

    /// <summary>True while any async fetch is in progress; drives the loading spinner.</summary>
    [ObservableProperty] private bool _isBusy;

    /// <summary>Text shown in the status bar at the bottom of the window.</summary>
    [ObservableProperty] private string _statusMessage = "Enter an ICAO code and press Lookup.";

    /// <summary>Error message shown in the red error banner (visible only when HasError is true).</summary>
    [ObservableProperty] private string _errorMessage = string.Empty;

    /// <summary>Controls visibility of the red error banner.</summary>
    [ObservableProperty] private bool _hasError;

    /// <summary>The zero-based index of the currently selected tab in MainTabs.</summary>
    [ObservableProperty] private int _selectedTabIndex;

    /// <summary>ICAO code of the last successfully looked-up airport.</summary>
    [ObservableProperty] private string _currentIcao = string.Empty;

    /// <summary>Latitude of the current airport; used to centre the Leaflet map.</summary>
    [ObservableProperty] private double? _currentLat;

    /// <summary>Longitude of the current airport; used to centre the Leaflet map.</summary>
    [ObservableProperty] private double? _currentLon;

    /// <summary>Airport suggestions shown in the AutoSuggestBox dropdown as the user types.</summary>
    [ObservableProperty] private ObservableCollection<AirportSuggestion> _airportSuggestions = [];

    /// <summary>
    /// Set to true when the user selects a suggestion rather than typing a raw ICAO code.
    /// Bypasses the 3–4 char length check in LookupAsync (ICAO is guaranteed valid).
    /// </summary>
    private bool _selectedFromSuggestion;

    // ── Child view-models (injected, exposed read-only for XAML binding) ──────

    public MetarViewModel MetarVm => metarVm;
    public TafViewModel TafVm => tafVm;
    public SigmetViewModel SigmetVm => sigmetVm;
    public ChartsViewModel ChartsVm => chartsVm;
    public CalculatorsViewModel CalculatorsVm => calculatorsVm;

    /// <summary>
    /// Pinned favourite airports shown in the header strip.
    /// Capped at 8 entries; adding beyond the cap removes the oldest.
    /// </summary>
    public ObservableCollection<FavoriteStation> Favorites { get; } =
        new(settingsService.Load().Favorites
            .Select(f => new FavoriteStation { Icao = f.Icao, FlightCategory = f.FlightCategory }));

    // Tracks the most recent in-flight request so it can be cancelled when the user
    // starts a new lookup before the previous one completes.
    private CancellationTokenSource? _cts;

    // ── Commands ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Validates the search text and triggers a full data fetch.
    /// Bound to both the search box QuerySubmitted event and the Lookup button.
    /// </summary>
    [RelayCommand]
    private async Task LookupAsync()
    {
        var icao = SearchText.Trim().ToUpperInvariant();

        if (!_selectedFromSuggestion)
        {
            if (string.IsNullOrEmpty(icao) || icao.Length < 3 || icao.Length > 4)
            {
                SetError("Please enter a valid 3–4 character ICAO code.");
                return;
            }
        }

        _selectedFromSuggestion = false;
        AirportSuggestions.Clear();
        await FetchAllAsync(icao);
    }

    /// <summary>
    /// Called by the AutoSuggestBox TextChanged handler (UserInput reason only).
    /// Runs a fast in-memory search and updates the suggestion dropdown.
    /// </summary>
    public void UpdateSuggestions(string text)
    {
        var results = airportSearchService.Search(text.Trim());
        AirportSuggestions.Clear();
        foreach (var s in results)
            AirportSuggestions.Add(s);
    }

    /// <summary>
    /// Called by the AutoSuggestBox SuggestionChosen handler.
    /// Populates SearchText with the ICAO code and triggers the full lookup,
    /// bypassing the length validation guard since the ICAO is guaranteed valid.
    /// </summary>
    public async Task SelectSuggestionAsync(AirportSuggestion suggestion)
    {
        _selectedFromSuggestion = true;
        SearchText = suggestion.Icao;
        await FetchAllAsync(suggestion.Icao);
    }

    /// <summary>
    /// Fetches all data for <paramref name="icao"/> in two parallel batches:
    /// first the METAR/airport/ATIS (needed for the main display and map coordinates),
    /// then TAF, SIGMETs, and nearby METARs in a second concurrent batch.
    /// Also called by the auto-refresh timer.
    /// </summary>
    public async Task FetchAllAsync(string icao)
    {
        // Cancel any previous in-flight request and start a fresh token.
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        ActivityLog.Record($"Searched for {icao}");
        IsBusy = true;
        HasError = false;
        StatusMessage = $"Loading data for {icao}…";

        try
        {
            // ── Batch 1: METAR + airport info + ATIS ─────────────────────────
            // These three are fetched concurrently because they are independent and
            // METAR/airport results are needed before we can start the map fetch.
            var metarTask = weatherService.GetMetarAsync(icao, ct);
            var airportTask = airportService.GetAirportAsync(icao, ct);
            var atisTask = atisService.GetAtisAsync(icao, ct);

            await Task.WhenAll(metarTask, airportTask, atisTask);

            var metar = metarTask.Result;
            var airport = airportTask.Result;
            var atisList = atisTask.Result;

            metarVm.Load(metar, airport, atisList);

            CurrentIcao = icao;

            // Use airport service coords first; fall back to the primary METAR's own lat/lon
            // if the airport service returned null (e.g. airport-data.com failure).
            var lat = airport?.Latitude ?? metar?.Latitude;
            var lon = airport?.Longitude ?? metar?.Longitude;
            CurrentLat = lat;
            CurrentLon = lon;

            // ── Batch 2: TAF + SIGMETs + nearby METARs ───────────────────────
            var tafTask    = weatherService.GetTafAsync(icao, ct);
            var sigmetTask = weatherService.GetSigmetsAsync(ct);

            // Nearby METAR fetch requires coordinates, so it is only launched if
            // we have valid lat/lon from either the airport service or the primary METAR.
            Task<List<Metar>>? nearbyTask = null;
            if (lat.HasValue && lon.HasValue)
                nearbyTask = weatherService.GetNearbyMetarsAsync(lat.Value, lon.Value, 1.0, ct);

            // Spread-operator syntax includes nearbyTask only when non-null.
            await Task.WhenAll([tafTask, sigmetTask, .. (nearbyTask != null ? new[] { nearbyTask } : [])]);

            tafVm.Load(tafTask.Result);
            sigmetVm.Load(sigmetTask.Result);

            // Pre-fill calculators with live values so the user doesn't have to re-enter them.
            if (metar != null)
                calculatorsVm.PreFillFromMetar(metar.TempC, metar.AltimeterInHg, metar.ElevationFeet, metar.WindDir, metar.WindSpeedKt);

            // Updating NearbyMetars last so the PropertyChanged handler in MainWindow.xaml.cs
            // can use it as a reliable signal that the full fetch cycle is complete.
            NearbyMetars = nearbyTask?.Result ?? [];
            OnPropertyChanged(nameof(NearbyMetars));

            StatusMessage = $"{icao} — updated {DateTime.Now:HH:mm:ss}";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Lookup cancelled.";
        }
        catch (Exception ex)
        {
            SetError($"Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// METARs for stations near the current airport, used to draw colour-coded
    /// flight-category pins on the Leaflet map. Updated at the end of each fetch cycle.
    /// </summary>
    public List<Metar> NearbyMetars { get; private set; } = [];

    /// <summary>Pins the current airport to the favourites strip if not already present.</summary>
    [RelayCommand]
    private void AddFavorite()
    {
        if (string.IsNullOrEmpty(CurrentIcao)) return;
        if (Favorites.Any(f => f.Icao == CurrentIcao)) return;

        // Enforce an 8-station cap by removing the oldest entry when full.
        if (Favorites.Count >= 8) Favorites.RemoveAt(0);

        Favorites.Add(new FavoriteStation
        {
            Icao = CurrentIcao,
            FlightCategory = MetarVm.FlightCategoryText
        });

        SaveSettings();
    }

    /// <summary>
    /// Loads a favourite station by populating the search box and triggering a lookup.
    /// Bound to mouse-click on each favourite chip in the header.
    /// </summary>
    [RelayCommand]
    private async Task SelectFavoriteAsync(FavoriteStation fav)
    {
        SearchText = fav.Icao;
        await LookupAsync();
    }

    [RelayCommand]
    private void RemoveFavorite(FavoriteStation fav)
    {
        Favorites.Remove(fav);
        SaveSettings();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SetError(string msg)
    {
        ErrorMessage = msg;
        HasError = true;
        StatusMessage = msg;
    }

    /// <summary>
    /// Serializes the current Favorites to the settings file.
    /// Round-trips through Load() first so IsDarkTheme (owned by MainWindow) is preserved.
    /// </summary>
    private void SaveSettings()
    {
        var settings = settingsService.Load();
        settings.Favorites = Favorites
            .Select(f => new SavedFavorite { Icao = f.Icao, FlightCategory = f.FlightCategory })
            .ToList();
        settingsService.Save(settings);
    }
}

/// <summary>
/// Represents a pinned airport in the favourites strip.
/// Stores the ICAO code and flight category so the chip can be colour-coded
/// without requiring a live data re-fetch.
/// </summary>
public partial class FavoriteStation : ObservableObject
{
    [ObservableProperty] private string _icao = string.Empty;

    /// <summary>Flight category at the time the favourite was saved (VFR, MVFR, IFR, LIFR).</summary>
    [ObservableProperty] private string _flightCategory = string.Empty;
}
