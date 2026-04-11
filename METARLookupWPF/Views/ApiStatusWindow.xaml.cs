using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;

namespace METARLookupWPF.Views;

/// <summary>
/// Shows a live connectivity check for every external API used by the application.
/// Each endpoint is pinged concurrently when the window opens; the Refresh button re-runs all checks.
/// </summary>
public partial class ApiStatusWindow : Window
{
    // Dedicated client with a shorter timeout than the app-wide one so checks don't hang.
    private static readonly HttpClient _http = new()
    {
        Timeout = TimeSpan.FromSeconds(8),
    };

    public ObservableCollection<ApiEndpoint> Endpoints { get; } =
    [
        new() { Name = "Av. Weather Center — METAR / TAF / SIGMET",
                Url  = "https://aviationweather.gov/api/data/metar?ids=KSEA&format=xml&mostRecent=true" },
        new() { Name = "Av. Weather Center — Airport Info",
                Url  = "https://aviationweather.gov/api/data/airport?ids=KSEA&format=json" },
        new() { Name = "Airport Data (airport-data.com)",
                Url  = "https://airport-data.com/api/ap_info.json?icao=KSEA" },
        new() { Name = "D-ATIS (datis.clowd.io)",
                Url  = "https://datis.clowd.io/api/KSEA" },
        new() { Name = "RainViewer Radar",
                Url  = "https://api.rainviewer.com/public/weather-maps.json" },
        new() { Name = "FAA Charts (aeronav.faa.gov)",
                Url  = "https://aeronav.faa.gov/d-tpp/" },
    ];

    public ApiStatusWindow()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += async (_, _) => await CheckAllAsync();
    }

    private async Task CheckAllAsync()
    {
        foreach (var ep in Endpoints)
            ep.SetChecking();

        await Task.WhenAll(Endpoints.Select(CheckAsync));
    }

    /// <summary>
    /// Sends a GET request (headers only) to the endpoint and records latency and HTTP status.
    /// UI updates are dispatched back to the UI thread because the continuation runs on the thread pool.
    /// </summary>
    private async Task CheckAsync(ApiEndpoint ep)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            using var request  = new HttpRequestMessage(HttpMethod.Get, ep.Url);
            using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            sw.Stop();

            var msg = response.IsSuccessStatusCode
                ? $"OK  ({sw.ElapsedMilliseconds} ms)"
                : $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}";

            Dispatcher.Invoke(() => ep.SetResult(response.IsSuccessStatusCode, msg));
        }
        catch (Exception ex)
        {
            sw.Stop();
            var msg = ex is TaskCanceledException ? "Timeout" : ex.Message;
            Dispatcher.Invoke(() => ep.SetResult(false, msg));
        }
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e) => await CheckAllAsync();
    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}

/// <summary>
/// Represents a single API endpoint in the status list.
/// Implements <see cref="INotifyPropertyChanged"/> so the DataTemplate updates automatically.
/// </summary>
public class ApiEndpoint : INotifyPropertyChanged
{
    public string Name { get; init; } = string.Empty;
    public string Url  { get; init; } = string.Empty;

    private string _status     = "Pending";
    private bool   _isChecking = false;
    private bool?  _isOk       = null;

    public string Status    { get => _status;     private set { _status     = value; OnPropertyChanged(); } }
    public bool   IsChecking { get => _isChecking; private set { _isChecking = value; OnPropertyChanged(); } }
    public bool?  IsOk       { get => _isOk;       private set { _isOk       = value; OnPropertyChanged(); } }

    public void SetChecking()
    {
        IsChecking = true;
        IsOk       = null;
        Status     = "Checking…";
    }

    public void SetResult(bool ok, string message)
    {
        IsChecking = false;
        IsOk       = ok;
        Status     = message;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
