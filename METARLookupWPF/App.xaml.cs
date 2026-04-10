using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using METARLookupWPF.Services;
using METARLookupWPF.ViewModels;
using METARLookupWPF.Views;

namespace METARLookupWPF;

public partial class App : Application
{
    private ServiceProvider? _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // HttpClient (shared singleton)
        services.AddSingleton(_ =>
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(15);
            return client;
        });

        // Services
        services.AddSingleton<IAvWeatherService, AvWeatherService>();
        services.AddSingleton<IAirportService, AirportService>();
        services.AddSingleton<IAtisService, AtisService>();

        // ViewModels
        services.AddSingleton<MetarViewModel>();
        services.AddSingleton<TafViewModel>();
        services.AddSingleton<SigmetViewModel>();
        services.AddSingleton<ChartsViewModel>();
        services.AddSingleton<CalculatorsViewModel>();
        services.AddSingleton<MainViewModel>();

        // Views
        services.AddSingleton<MainWindow>();

        _services = services.BuildServiceProvider();

        var mainWindow = _services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _services?.Dispose();
        base.OnExit(e);
    }
}
