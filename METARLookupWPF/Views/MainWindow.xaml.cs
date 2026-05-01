using System.Windows;
using System.Windows.Controls;
using ModernWpf;
using METARLookupWPF.Models;
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
    private readonly CrashReportService _crashReportService;

    /// <summary>
    /// Constructor receives the view-model and services via dependency injection.
    /// Sets DataContext so all XAML bindings resolve against MainViewModel.
    /// </summary>
    public MainWindow(MainViewModel vm, IUserSettingsService settingsService, CrashReportService crashReportService)
    {
        InitializeComponent();
        _vm = vm;
        _settingsService = settingsService;
        _crashReportService = crashReportService;
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
    /// Handles the AutoSuggestBox QuerySubmitted event (triggered by pressing Enter with typed text).
    /// When a suggestion is chosen, SuggestionChosen fires first and handles the lookup — we skip
    /// QuerySubmitted in that case (args.ChosenSuggestion is non-null) to avoid a double fetch.
    /// </summary>
    private void SearchBox_QuerySubmitted(ModernWpf.Controls.AutoSuggestBox sender,
        ModernWpf.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        // SuggestionChosen already fired FetchAllAsync — don't trigger a second lookup.
        if (args.ChosenSuggestion is not null)
            return;

        _vm.SearchText = args.QueryText ?? sender.Text ?? string.Empty;
        _vm.LookupCommand.Execute(null);
    }

    /// <summary>
    /// Fires on every keystroke in the search box (UserInput reason only).
    /// Delegates to the view-model to run an in-memory airport search and
    /// populate the suggestion dropdown.
    /// </summary>
    private void SearchBox_TextChanged(ModernWpf.Controls.AutoSuggestBox sender,
        ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == ModernWpf.Controls.AutoSuggestionBoxTextChangeReason.UserInput)
            _vm.UpdateSuggestions(sender.Text);
    }

    /// <summary>
    /// Fires when the user selects an airport from the suggestion dropdown.
    /// Delegates to the view-model which sets SearchText and triggers the full lookup.
    /// </summary>
    private async void SearchBox_SuggestionChosen(ModernWpf.Controls.AutoSuggestBox sender,
        ModernWpf.Controls.AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is AirportSuggestion suggestion)
            await _vm.SelectSuggestionAsync(suggestion);
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

    /// <summary>Opens the bug report dialog so the user can send a manual report.</summary>
    private void ReportBug_Click(object sender, RoutedEventArgs e)
    {
        //#if DEBUG
        //        // Capture a real exception so the Sentry test event includes a stack trace.
        //        // Remove this block once Sentry integration is verified.
        //        Exception? testEx = null;
        //        try { throw new InvalidOperationException("Sentry integration test — deliberate test exception."); }
        //        catch (Exception ex) { testEx = ex; }
        //        var dlg = new CrashReportWindow(_crashReportService, testEx, CrashReportMode.Manual) { Owner = this };
        //#else
                var dlg = new CrashReportWindow(_crashReportService, null, CrashReportMode.Manual) { Owner = this };
        //#endif
        dlg.ShowDialog();
    }

    /// <summary>Applies the ModernWpf application theme immediately when the toggle is switched and persists the choice.</summary>
    private async void ThemeSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        bool isDark = ThemeSwitch.IsChecked == true;
        ThemeManager.Current.ApplicationTheme =
            isDark ? ApplicationTheme.Dark : ApplicationTheme.Light;

        var settings = _settingsService.Load();
        settings.IsDarkTheme = isDark;
        _settingsService.Save(settings);

        // Switch map tile layer via JS — no page reload, instant, no HTTP request.
        if (MainTabs.SelectedIndex == 2)
            await MapViewControl.SetThemeAsync(isDark);
    }

    /// <summary>
    /// Implements lazy loading for the Map (index 2) and Charts (index 3) tabs.
    /// The map is refreshed every time the tab is shown; charts are loaded on demand
    /// (the expensive metafile download happens only once per ICAO thanks to caching).
    /// Does nothing if no airport has been looked up yet.
    /// </summary>
    private static readonly string[] TabNames = ["METAR", "TAF", "Map", "Airport Charts", "SIGMETs", "Calculators"];

    private async void MainTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var idx = MainTabs.SelectedIndex;
        var tabName = idx >= 0 && idx < TabNames.Length ? TabNames[idx] : $"Tab {idx}";
        ActivityLog.Record($"Opened {tabName} tab" + (string.IsNullOrEmpty(_vm.CurrentIcao) ? "" : $" for {_vm.CurrentIcao}"));

        if (!string.IsNullOrEmpty(_vm.CurrentIcao))
        {
            if (idx == 2)
                await MapViewControl.ShowAirportAsync(_vm.CurrentLat, _vm.CurrentLon, _vm.NearbyMetars, _vm.CurrentIcao);
            else if (idx == 3)
                await _vm.ChartsVm.LoadAsync(_vm.CurrentIcao);
        }
    }
}
