using System.Windows;
using System.Windows.Controls;
using ModernWpf;
using METARLookupWPF.ViewModels;

namespace METARLookupWPF.Views;

/// <summary>
/// Code-behind for the main application window. Responsibilities are minimal:
/// wiring up events that cannot be expressed purely in XAML bindings (theme switching,
/// tab-lazy-loading, and the search box query event) and delegating everything else
/// to <see cref="MainViewModel"/>.
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    /// <summary>
    /// Constructor receives the view-model via dependency injection (registered in App.xaml.cs).
    /// Sets DataContext so all XAML bindings resolve against MainViewModel.
    /// </summary>
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;

        // Sync the dark-mode toggle with whatever theme is active at startup.
        // Must run after Loaded because ThemeManager.Current is not ready until then.
        Loaded += (_, _) =>
            ThemeSwitch.IsOn = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;

        // When NearbyMetars is updated (the last step of FetchAllAsync), automatically
        // refresh the map if the Map tab is currently visible.
        // NearbyMetars is not an [ObservableProperty] collection so it fires a plain
        // PropertyChanged event, which we listen for here.
        vm.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.NearbyMetars) && MainTabs.SelectedIndex == 2)
                await MapViewControl.ShowAirportAsync(_vm.CurrentLat, _vm.CurrentLon, _vm.NearbyMetars);
        };
    }

    /// <summary>
    /// Handles the AutoSuggestBox QuerySubmitted event (triggered by pressing Enter or selecting a suggestion).
    /// We read the text directly from the event args rather than relying on the two-way binding because
    /// WPF bindings may not have propagated the latest keystroke by the time this handler fires.
    /// </summary>
    private void SearchBox_QuerySubmitted(ModernWpf.Controls.AutoSuggestBox sender,
        ModernWpf.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        _vm.SearchText = args.QueryText ?? sender.Text ?? string.Empty;
        _vm.LookupCommand.Execute(null);
    }

    /// <summary>
    /// Handles the Lookup button click. Reads the search box text directly from the control
    /// for the same binding-flush reason as <see cref="SearchBox_QuerySubmitted"/>.
    /// </summary>
    private void LookupButton_Click(object sender, RoutedEventArgs e)
    {
        _vm.SearchText = SearchBox.Text ?? string.Empty;
        _vm.LookupCommand.Execute(null);
    }

    /// <summary>Applies the ModernWpf application theme immediately when the toggle is switched.</summary>
    private void ThemeSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        ThemeManager.Current.ApplicationTheme =
            ThemeSwitch.IsOn ? ApplicationTheme.Dark : ApplicationTheme.Light;
    }

    /// <summary>
    /// Implements lazy loading for the Map (index 2) and Charts (index 3) tabs.
    /// The map is refreshed every time the tab is shown; charts are loaded on demand
    /// (the expensive metafile download happens only once per ICAO thanks to caching).
    /// Does nothing if no airport has been looked up yet.
    /// </summary>
    private async void MainTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_vm.CurrentIcao))
        {
            if (MainTabs.SelectedIndex == 2)
                await MapViewControl.ShowAirportAsync(_vm.CurrentLat, _vm.CurrentLon, _vm.NearbyMetars);
            else if (MainTabs.SelectedIndex == 3)
                await _vm.ChartsVm.LoadAsync(_vm.CurrentIcao);
        }
    }
}
