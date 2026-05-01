using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using METARLookupWPF.Services;
using METARLookupWPF.ViewModels;
using METARLookupWPF.Views;
using Sentry;

namespace METARLookupWPF;

/// <summary>
/// Application entry point. Bootstraps the Microsoft.Extensions.DependencyInjection
/// container, registering all services, view-models, and the main window as singletons,
/// then shows the main window.
/// Using a DI container (rather than manual construction) makes it straightforward to
/// swap service implementations for testing or to add new features.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _services;
    private IDisposable? _sentryDisposable;

    /// <summary>
    /// Builds the DI container and shows the main window. Called by WPF before any UI is displayed.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _sentryDisposable = SentrySdk.Init(options =>
        {
            options.Dsn = "https://46c8ae2fb3fc8bfef5ba38935e3ef9e3@o4511311807512576.ingest.us.sentry.io/4511311810592768";
            options.Release = $"metar-lookup@{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
            options.Environment = "production";
            // Disable auto-capture — we send only when the user clicks "Send Report"
            options.DisableAppDomainUnhandledExceptionCapture();
            options.AutoSessionTracking = false;
        });

        // Set e.Handled = true before anything else so WPF does not re-raise the exception
        // if our handler itself fails.
        DispatcherUnhandledException += (_, args) =>
        {
            args.Handled = true;
            HandleCrash(args.Exception);
            Current.Shutdown(1);
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                HandleCrash(ex);
        };

        var services = new ServiceCollection();

        // A single shared HttpClient instance is used by all services to avoid socket exhaustion.
        // The browser-like User-Agent header is set because some aviation APIs reject requests
        // that don't look like a web browser.
        services.AddSingleton(_ =>
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36");
            client.Timeout = TimeSpan.FromSeconds(15);
            return client;
        });

        // ── Services ──────────────────────────────────────────────────────────
        // All services are singletons because they are stateless (or hold only a cache)
        // and share the single HttpClient instance.
        services.AddSingleton<IAvWeatherService, AvWeatherService>();
        services.AddSingleton<IAirportService, AirportService>();
        services.AddSingleton<IAtisService, AtisService>();
        services.AddSingleton<IAirportSearchService, AirportSearchService>();
        services.AddSingleton<IFaaChartsService, FaaChartsService>();
        services.AddSingleton<IUserSettingsService, UserSettingsService>();
        services.AddSingleton<CrashReportService>();

        // ── ViewModels ────────────────────────────────────────────────────────
        // Child VMs are registered before MainViewModel because MainViewModel
        // declares them as constructor parameters and the DI container resolves them automatically.
        services.AddSingleton<MetarViewModel>();
        services.AddSingleton<TafViewModel>();
        services.AddSingleton<SigmetViewModel>();
        services.AddSingleton<ChartsViewModel>();
        services.AddSingleton<CalculatorsViewModel>();
        services.AddSingleton<MainViewModel>();

        // ── Views ─────────────────────────────────────────────────────────────
        services.AddSingleton<MainWindow>();

        _services = services.BuildServiceProvider();

        var mainWindow = _services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    /// <summary>
    /// Disposes the DI container (and any IDisposable services) when the application exits.
    /// </summary>
    protected override void OnExit(ExitEventArgs e)
    {
        _sentryDisposable?.Dispose();
        _services?.Dispose();
        base.OnExit(e);
    }

    private void HandleCrash(Exception ex)
    {
        try
        {
            var reporter = _services?.GetService<CrashReportService>();
            if (reporter == null)
            {
                // DI not yet ready — show a minimal message box.
                MessageBox.Show(
                    $"METAR Lookup encountered an unexpected error and must close.\n\n{ex.Message}",
                    "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var diagText = reporter.BuildDiagnosticText(ex);
            reporter.WriteLogFile(diagText);

            Dispatcher.Invoke(() =>
            {
                try
                {
                    var window = new CrashReportWindow(reporter, ex, CrashReportMode.Crash);
                    window.ShowDialog();
                }
                catch
                {
                    // Dialog itself failed — fall back to a plain message box.
                    MessageBox.Show(
                        $"METAR Lookup encountered an unexpected error.\n\n{ex.Message}",
                        "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
        catch
        {
            // Last resort — if everything above fails, at least show something.
            try
            {
                MessageBox.Show(
                    $"METAR Lookup encountered an unexpected error and must close.\n\n{ex.Message}",
                    "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { /* nothing more we can do */ }
        }
    }
}
