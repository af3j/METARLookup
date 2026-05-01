using System.IO;
using System.Reflection;
using System.Text;
using Sentry;

namespace METARLookupWPF.Services;

public class CrashReportService
{
    internal static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "METARLookup",
        "crash_log.txt");

    public string BuildDiagnosticText(Exception? ex, string? userDescription = null)
    {
        try
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
            var sb = new StringBuilder();
            sb.AppendLine("=== METAR Lookup Bug Report ===");
            sb.AppendLine($"Timestamp  : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine($"App Version: v{version}");
            sb.AppendLine($"OS         : {Environment.OSVersion}");
            sb.AppendLine($".NET       : {Environment.Version}");

            if (!string.IsNullOrWhiteSpace(userDescription))
            {
                sb.AppendLine();
                sb.AppendLine("User description:");
                sb.AppendLine(userDescription.Trim());
            }

            var activity = ActivityLog.GetEntries();
            if (activity.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Recent activity (oldest → newest):");
                foreach (var entry in activity)
                    sb.AppendLine($"  {entry}");
            }

            if (ex != null)
            {
                sb.AppendLine();
                sb.AppendLine($"Exception  : {ex.GetType().FullName}");
                sb.AppendLine($"Message    : {ex.Message}");
                sb.AppendLine();
                sb.AppendLine("Stack trace:");
                sb.AppendLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    sb.AppendLine();
                    sb.AppendLine($"Inner      : {ex.InnerException.GetType().FullName}");
                    sb.AppendLine($"Inner msg  : {ex.InnerException.Message}");
                    sb.AppendLine(ex.InnerException.StackTrace);
                }
            }

            return sb.ToString();
        }
        catch (Exception buildEx)
        {
            return $"METAR Lookup Bug Report\n[Could not build full report: {buildEx.Message}]\nOriginal error: {ex?.Message}";
        }
    }

    public void WriteLogFile(string diagnosticText)
    {
        try
        {
            var dir = Path.GetDirectoryName(LogPath)!;
            Directory.CreateDirectory(dir);
            File.AppendAllText(LogPath, diagnosticText + "\n---\n\n", Encoding.UTF8);
        }
        catch { /* swallow — a logging failure must never prevent the crash dialog */ }
    }

    public void CaptureSentryReport(Exception? ex, string? userDescription)
    {
        try
        {
            void ConfigureScope(Scope scope)
            {
                foreach (var entry in ActivityLog.GetEntries())
                    scope.AddBreadcrumb(entry, "user-activity", level: BreadcrumbLevel.Info);

                if (!string.IsNullOrWhiteSpace(userDescription))
                    scope.SetExtra("user-description", userDescription.Trim());
            }

            if (ex != null)
                SentrySdk.CaptureException(ex, ConfigureScope);
            else
                SentrySdk.CaptureMessage(
                    string.IsNullOrWhiteSpace(userDescription) ? "Manual bug report" : userDescription.Trim(),
                    ConfigureScope,
                    SentryLevel.Info);
        }
        catch { /* sending failure must never crash the app */ }
    }
}
