using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METARLookupWPF.Models;
using METARLookupWPF.Services;

namespace METARLookupWPF.ViewModels;

/// <summary>
/// View-model for the Airport Charts tab. Manages loading FAA d-TPP chart groups
/// for the current airport and coordinating PDF navigation with <see cref="ChartsView"/>
/// via the <see cref="NavigateToPdf"/> event.
/// </summary>
public partial class ChartsViewModel(IFaaChartsService chartsService) : ObservableObject
{
    /// <summary>
    /// Chart groups displayed in the left-panel tree (e.g. "Instrument Approaches", "Departures").
    /// Uses ObservableCollection so the ItemsControl in the view updates incrementally.
    /// </summary>
    [ObservableProperty] private ObservableCollection<ChartGroup> _groups     = [];

    /// <summary>The chart the user most recently clicked; drives PDF navigation.</summary>
    [ObservableProperty] private AirportChart?                    _selectedChart;

    /// <summary>True when at least one chart group is available for the current airport.</summary>
    [ObservableProperty] private bool                             _hasCharts;

    /// <summary>Controls the "select a chart" placeholder overlay in the right panel.</summary>
    [ObservableProperty] private bool                             _hasPdfOpen;

    /// <summary>True while the charts metafile is being fetched; drives the left-panel progress ring.</summary>
    [ObservableProperty] private bool                             _isLoading;

    /// <summary>Status message shown below the progress ring when no charts are loaded yet.</summary>
    [ObservableProperty] private string                           _statusText  = "Select an airport to view charts.";

    /// <summary>
    /// Raised whenever a chart is selected and the PDF viewer should navigate to a new URL.
    /// The view subscribes to this event in <see cref="ChartsView.OnDataContextChanged"/>
    /// so the ViewModel stays unaware of the WebView2 control.
    /// </summary>
    public event Action<string>? NavigateToPdf;

    /// <summary>Bound to the click command of each chart button in the left panel list.</summary>
    [RelayCommand]
    private void SelectChart(AirportChart chart) => SelectedChart = chart;

    /// <summary>
    /// Partial method called automatically when SelectedChart changes.
    /// Fires the NavigateToPdf event so ChartsView can forward the URL to WebView2.
    /// Pattern-matching on PdfUrl length guards against charts with empty URLs.
    /// </summary>
    partial void OnSelectedChartChanged(AirportChart? value)
    {
        HasPdfOpen = value != null;
        if (value?.PdfUrl is { Length: > 0 } url)
            NavigateToPdf?.Invoke(url);
    }

    /// <summary>
    /// Fetches chart groups for <paramref name="icao"/> from the FAA d-TPP metafile.
    /// Called lazily when the user first switches to the Charts tab (not on every lookup),
    /// because downloading the large metafile XML is expensive.
    /// </summary>
    public async Task LoadAsync(string icao, CancellationToken ct = default)
    {
        IsLoading = true;
        StatusText = $"Loading charts for {icao}…";
        HasCharts  = false;
        HasPdfOpen = false;
        Groups.Clear();
        SelectedChart = null;

        try
        {
            var groups = await chartsService.GetAirportChartsAsync(icao, ct);

            // Add groups individually so the ItemsControl animates each one in as it arrives.
            foreach (var g in groups) Groups.Add(g);

            HasCharts  = groups.Count > 0;
            StatusText = HasCharts
                ? $"{groups.Sum(g => g.Charts.Count)} charts for {icao}"
                : $"No FAA charts found for {icao}. (International airports may not be listed.)";
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            StatusText = $"Error loading charts: {ex.Message}";
        }
        finally { IsLoading = false; }
    }
}
