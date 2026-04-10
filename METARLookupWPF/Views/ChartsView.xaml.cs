using System.Windows;
using System.Windows.Controls;
using METARLookupWPF.ViewModels;

namespace METARLookupWPF.Views;

/// <summary>
/// Code-behind for the Airport Charts tab. Manages WebView2 initialisation
/// and subscribes to <see cref="ChartsViewModel.NavigateToPdf"/> so the view-model
/// can trigger PDF navigation without holding a reference to the WebView2 control.
/// This keeps the ViewModel testable and free of UI dependencies.
/// </summary>
public partial class ChartsView : UserControl
{
    // Tracks whether EnsureCoreWebView2Async has completed so we know when it is safe to call Navigate().
    private bool    _webViewInitialized;

    // Buffers a URL that arrived before WebView2 was ready; consumed in OnLoaded.
    private string? _pendingUrl;

    public ChartsView()
    {
        InitializeComponent();
        Loaded             += OnLoaded;
        DataContextChanged += OnDataContextChanged;
    }

    /// <summary>
    /// Initialises WebView2 the first time the Charts tab becomes visible.
    /// The flag is set AFTER the await so any NavigateToPdfUrl calls that arrive
    /// concurrently will see the correct value.
    /// </summary>
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_webViewInitialized) return;
        await ChartWebView.EnsureCoreWebView2Async();
        _webViewInitialized = true;                 // set AFTER CoreWebView2 is ready

        // A chart may have been selected before the WebView was ready; navigate now.
        if (_pendingUrl is not null)
        {
            ChartWebView.CoreWebView2.Navigate(_pendingUrl);
            _pendingUrl = null;
        }
    }

    /// <summary>
    /// Subscribes/unsubscribes to <see cref="ChartsViewModel.NavigateToPdf"/> whenever
    /// the DataContext is replaced. This prevents memory leaks (old VM holding a reference
    /// to this view) and ensures the correct ViewModel is wired up after DI re-assignment.
    /// </summary>
    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChartsViewModel old) old.NavigateToPdf -= NavigateToPdfUrl;
        if (e.NewValue is ChartsViewModel vm)  vm.NavigateToPdf  += NavigateToPdfUrl;
    }

    /// <summary>
    /// Navigates WebView2 to the given PDF URL, or stores it for deferred navigation
    /// if WebView2 is not yet initialised.
    /// </summary>
    private void NavigateToPdfUrl(string url)
    {
        if (_webViewInitialized)
            ChartWebView.CoreWebView2.Navigate(url);
        else
            _pendingUrl = url;   // will be consumed once CoreWebView2 is ready in OnLoaded
    }
}
