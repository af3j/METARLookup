using System.Windows;
using System.Windows.Controls;
using ModernWpf;
using METARLookupWPF.ViewModels;

namespace METARLookupWPF.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
    }

    private void SearchBox_QuerySubmitted(ModernWpf.Controls.AutoSuggestBox sender,
        ModernWpf.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        // Use the query text directly from the event — binding may not have flushed yet
        _vm.SearchText = args.QueryText ?? sender.Text ?? string.Empty;
        _vm.LookupCommand.Execute(null);
    }

    private void LookupButton_Click(object sender, RoutedEventArgs e)
    {
        // Read directly from the control to avoid stale binding
        _vm.SearchText = SearchBox.Text ?? string.Empty;
        _vm.LookupCommand.Execute(null);
    }

    private void ThemeSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        ThemeManager.Current.ApplicationTheme =
            ThemeSwitch.IsOn ? ApplicationTheme.Dark : ApplicationTheme.Light;
    }

    private async void MainTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // When Map tab selected (index 2), refresh the map
        if (MainTabs.SelectedIndex == 2 && !string.IsNullOrEmpty(_vm.CurrentIcao))
        {
            await MapViewControl.ShowAirportAsync(_vm.CurrentLat, _vm.CurrentLon, _vm.NearbyMetars);
        }
    }
}
