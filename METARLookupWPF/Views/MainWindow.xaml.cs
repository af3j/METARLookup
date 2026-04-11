using System.Windows;
using System.Windows.Controls;
using ModernWpf;
using METARLookupWPF.Services;
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
    private readonly IUserSettingsService _settingsService;

    /// <summary>
    /// Constructor receives the view-model and settings service via dependency injection.
    /// Sets DataContext so all XAML bindings resolve against MainViewModel.
    /// </summary>
    public MainWindow(MainViewModel vm, IUserSettingsService settingsService)
    {
        InitializeComponent();
        _vm = vm;
        _settingsService = settingsService;
        DataContext = vm;

        // Restore the persisted theme on startup. Unsubscribe/resubscribe around the
        // programmatic IsOn set to prevent ThemeSwitch_Toggled from firing during init.
        Loaded += (_, _) =>
        {
            var settings = _settingsService.Load();
            ThemeManager.Current.ApplicationTheme =
                settings.IsDarkTheme ? ApplicationTheme.Dark : ApplicationTheme.Light;
            ThemeSwitch.Checked -= ThemeSwitch_Toggled;
            ThemeSwitch.Unchecked -= ThemeSwitch_Toggled;
            ThemeSwitch.IsChecked = settings.IsDarkTheme;
            ThemeSwitch.Checked += ThemeSwitch_Toggled;
            ThemeSwitch.Unchecked += ThemeSwitch_Toggled;
        };

        // When NearbyMetars is updated (the last step of FetchAllAsync), automatically
        // refresh the map if the Map tab is currently visible.
        vm.PropertyChanged += async (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.NearbyMetars) && MainTabs.SelectedIndex == 2)
                await MapViewControl.ShowAirportAsync(_vm.CurrentLat, _vm.CurrentLon, _vm.NearbyMetars, _vm.CurrentIcao);
        };

        // When the user clicks "Look Up" on a map marker, run a full lookup for that station.
        MapViewControl.StationSelected += async icao =>
        {
            _vm.SearchText = icao;
            await _vm.FetchAllAsync(icao);
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

    /// <summary>Opens the API Status dialog and runs connectivity checks for all external endpoints.</summary>
    private void ApiStatus_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new ApiStatusWindow { Owner = this };
        dlg.ShowDialog();
    }

    /// <summary>Applies the ModernWpf application theme immediately when the toggle is switched and persists the choice.</summary>
    private void ThemeSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        bool isDark = ThemeSwitch.IsChecked == true;
        ThemeManager.Current.ApplicationTheme =
            isDark ? ApplicationTheme.Dark : ApplicationTheme.Light;

        var settings = _settingsService.Load();
        settings.IsDarkTheme = isDark;
        _settingsService.Save(settings);
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
                await MapViewControl.ShowAirportAsync(_vm.CurrentLat, _vm.CurrentLon, _vm.NearbyMetars, _vm.CurrentIcao);
            else if (MainTabs.SelectedIndex == 3)
                await _vm.ChartsVm.LoadAsync(_vm.CurrentIcao);
        }
    }
}
