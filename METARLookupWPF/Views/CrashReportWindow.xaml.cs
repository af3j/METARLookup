using System.Diagnostics;
using System.IO;
using System.Windows;
using METARLookupWPF.Services;

namespace METARLookupWPF.Views;

public enum CrashReportMode { Crash, Manual }

public partial class CrashReportWindow : Window
{
    private readonly CrashReportService _reporter;
    private readonly Exception? _exception;
    private readonly CrashReportMode _mode;

    private static string LogPath => CrashReportService.LogPath;

    public CrashReportWindow(CrashReportService reporter, Exception? exception, CrashReportMode mode)
    {
        InitializeComponent();
        _reporter = reporter;
        _exception = exception;
        _mode = mode;

        try
        {
            Title = mode == CrashReportMode.Crash ? "Application Error" : "Report a Bug";

            HeadingText.Text = mode == CrashReportMode.Crash
                ? "METAR Lookup has encountered an unexpected error."
                : "Report a Bug";

            SubText.Text = mode == CrashReportMode.Crash
                ? "The application will close after you dismiss this dialog. Click 'Send Report' to send a crash report with diagnostics to the developer."
                : "Describe the issue below, then click 'Send Report' to submit directly to the developer.";

            CloseButton.Content = mode == CrashReportMode.Crash ? "Close App" : "Cancel";

            OpenLogFolderButton.Visibility = File.Exists(LogPath)
                ? Visibility.Visible
                : Visibility.Collapsed;

            DiagnosticTextBox.Text = _reporter.BuildDiagnosticText(_exception);
        }
        catch (Exception initEx)
        {
            try
            {
                // Minimal fallback if the window fails to fully initialize.
                DiagnosticTextBox.Text =
                    $"Could not build full diagnostic report.\nError: {initEx.Message}\n" +
                    $"Original error: {exception?.Message}";
            }
            catch { /* nothing more we can do */ }
        }
    }

    private void SendReport_Click(object sender, RoutedEventArgs e)
    {
        _reporter.CaptureSentryReport(_exception, UserDescriptionBox.Text);

        // Show confirmation — user should see feedback before the window closes or they dismiss.
        SendButton.IsEnabled = false;
        SendButton.Content = "✓ Sent";
        SubText.Text = _mode == CrashReportMode.Crash
            ? "Report submitted — thank you. Click 'Close App' to exit."
            : "Report submitted — thank you!";
        SubText.Opacity = 1.0;
        UserDescriptionBox.IsEnabled = false;
    }

    private void OpenLogFolder_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Select the file in Explorer so the user can see it immediately.
            Process.Start("explorer.exe", $"/select,\"{LogPath}\"");
        }
        catch { /* swallow — not critical */ }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
