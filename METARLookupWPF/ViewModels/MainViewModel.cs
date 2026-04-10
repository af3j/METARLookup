using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METARLookupWPF.Models;
using METARLookupWPF.Services;

namespace METARLookupWPF.ViewModels;

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
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _statusMessage = "Enter an ICAO code and press Lookup.";
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private bool _autoRefresh;
    [ObservableProperty] private int _selectedTabIndex;
    [ObservableProperty] private string _currentIcao = string.Empty;
    [ObservableProperty] private double? _currentLat;
    [ObservableProperty] private double? _currentLon;

    public MetarViewModel MetarVm => metarVm;
    public TafViewModel TafVm => tafVm;
    public SigmetViewModel SigmetVm => sigmetVm;
    public ChartsViewModel ChartsVm => chartsVm;
    public CalculatorsViewModel CalculatorsVm => calculatorsVm;

    public ObservableCollection<FavoriteStation> Favorites { get; } = [];

    private System.Timers.Timer? _refreshTimer;
    private CancellationTokenSource? _cts;

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

    public async Task FetchAllAsync(string icao)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        IsBusy = true;
        HasError = false;
        StatusMessage = $"Loading data for {icao}…";

        try
        {
            // Fetch METAR, airport info, and ATIS in parallel
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

            // Fetch TAF, SIGMET, and history in parallel (non-critical)
            var tafTask = weatherService.GetTafAsync(icao, ct);
            var sigmetTask = weatherService.GetSigmetsAsync(ct);
            var historyTask = weatherService.GetMetarHistoryAsync(icao, 24, ct);

            // Map task only if we have coords
            Task<List<Metar>>? nearbyTask = null;
            if (airport?.Latitude.HasValue == true && airport.Longitude.HasValue == true)
                nearbyTask = weatherService.GetNearbyMetarsAsync(airport.Latitude!.Value, airport.Longitude!.Value, 1.0, ct);

            await Task.WhenAll([tafTask, sigmetTask, historyTask, .. (nearbyTask != null ? new[] { nearbyTask } : [])]);

            tafVm.Load(tafTask.Result);
            sigmetVm.Load(sigmetTask.Result);
            chartsVm.Load(historyTask.Result);

            // Pre-fill calculators from live METAR
            if (metar != null)
                calculatorsVm.PreFillFromMetar(metar.TempC, metar.AltimeterInHg, metar.ElevationFeet, metar.WindDir, metar.WindSpeedKt);

            // Update map
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

    public List<Metar> NearbyMetars { get; private set; } = [];

    [RelayCommand]
    private void AddFavorite()
    {
        if (string.IsNullOrEmpty(CurrentIcao)) return;
        if (Favorites.Any(f => f.Icao == CurrentIcao)) return;
        if (Favorites.Count >= 8) Favorites.RemoveAt(0);

        Favorites.Add(new FavoriteStation
        {
            Icao = CurrentIcao,
            FlightCategory = MetarVm.FlightCategoryText
        });
    }

    [RelayCommand]
    private async Task SelectFavoriteAsync(FavoriteStation fav)
    {
        SearchText = fav.Icao;
        await LookupAsync();
    }

    [RelayCommand]
    private void RemoveFavorite(FavoriteStation fav) => Favorites.Remove(fav);

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
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(
                    async () => await FetchAllAsync(CurrentIcao));
            };
            _refreshTimer.AutoReset = true;
            _refreshTimer.Start();
        }
    }

    private void SetError(string msg)
    {
        ErrorMessage = msg;
        HasError = true;
        StatusMessage = msg;
    }
}

public partial class FavoriteStation : ObservableObject
{
    [ObservableProperty] private string _icao = string.Empty;
    [ObservableProperty] private string _flightCategory = string.Empty;
}
