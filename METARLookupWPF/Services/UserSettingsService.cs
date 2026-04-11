using System.IO;
using System.Text.Json;
using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

/// <summary>
/// Reads and writes user settings as UTF-8 JSON to
/// %APPDATA%\METARLookupWPF\settings.json.
/// All I/O is wrapped in try/catch so the app always starts even if the
/// settings file is missing, corrupted, or in a read-only location.
/// </summary>
public class UserSettingsService : IUserSettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "METARLookupWPF",
        "settings.json");

    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    /// <inheritdoc/>
    public UserSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
                return new UserSettings();

            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
        }
        catch
        {
            return new UserSettings();
        }
    }

    /// <inheritdoc/>
    public void Save(UserSettings settings)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings, JsonOpts));
        }
        catch
        {
            // Silently ignore — a settings write failure must never crash the app.
        }
    }
}
