using System.Collections.ObjectModel;
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
    MetarViewModel metarVm,
    TafViewModel tafVm,
    SigmetViewModel sigmetVm,
    ChartsViewModel chartsVm,
    CalculatorsViewModel calculatorsVm) : ObservableObject
{
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

    /// <summary>When true, a 5-minute repeating timer re-fetches data for the current ICAO automatically.</summary>
    [ObservableProperty] private bool _autoRefresh;

    /// <summary>The zero-based index of the currently selected tab in MainTabs.</summary>
    [ObservableProperty] private int _selectedTabIndex;

    /// <summary>ICAO code of the last successfully looked-up airport.</summary>
    [ObservableProperty] private string _currentIcao = string.Empty;

    /// <summary>Latitude of the current airport; used to centre the Leaflet map.</summary>
    [ObservableProperty] private double? _currentLat;

    /// <summary>Longitude of the current airport; used to centre the Leaflet map.</summary>
    [ObservableProperty] private double? _currentLon;

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
    public ObservableCollection<FavoriteStation> Favorites { get; } = [];

    private System.Timers.Timer? _refreshTimer;

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
        if (string.IsNullOrEmpty(icao) || icao.Length < 3 || icao.Length > 4)
        {
            SetError("Please enter a valid 3–4 character ICAO code.");
            return;
        }

        await FetchAllAsync(icao);
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
            CurrentLat = airport?.Latitude;
            CurrentLon = airport?.Longitude;

            // ── Batch 2: TAF + SIGMETs + nearby METARs ───────────────────────
            var tafTask    = weatherService.GetTafAsync(icao, ct);
            var sigmetTask = weatherService.GetSigmetsAsync(ct);

            // Nearby METAR fetch requires coordinates, so it is only launched if
            // the airport lookup succeeded and returned valid lat/lon.
            Task<List<Metar>>? nearbyTask = null;
            if (airport?.Latitude.HasValue == true && airport.Longitude.HasValue == true)
                nearbyTask = weatherService.GetNearbyMetarsAsync(airport.Latitude!.Value, airport.Longitude!.Value, 1.0, ct);

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
    private void RemoveFavorite(FavoriteStation fav) => Favorites.Remove(fav);

    // ── Auto-refresh ──────────────────────────────────────────────────────────

    /// <summary>
    /// Partial method called automatically by the source generator whenever AutoRefresh changes.
    /// Creates or destroys a 5-minute repeating timer accordingly.
    /// The timer callback marshals onto the UI thread via Dispatcher.InvokeAsync because
    /// System.Timers.Timer fires on a thread-pool thread.
    /// </summary>
    partial void OnAutoRefreshChanged(bool value)
    {
        _refreshTimer?.Stop();
        _refreshTimer?.Dispose();
        _refreshTimer = null;

        if (value && !string.IsNullOrEmpty(CurrentIcao))
        {
            _refreshTimer = new System.Timers.Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            _refreshTimer.Elapsed += async (_, _) =>
            {
                // System.Timers.Timer fires on a thread-pool thread; WPF UI must be updated on the UI thread.
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(
                    async () => await FetchAllAsync(CurrentIcao));
            };
            _refreshTimer.AutoReset = true;
            _refreshTimer.Start();
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SetError(string msg)
    {
        ErrorMessage = msg;
        HasError = true;
        StatusMessage = msg;
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
