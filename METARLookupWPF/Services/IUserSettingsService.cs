using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

/// <summary>
/// Provides load/save operations for user preferences persisted between sessions.
/// Registered as a singleton so the same instance is shared by MainViewModel and MainWindow.
/// </summary>
public interface IUserSettingsService
{
    /// <summary>
    /// Returns the current settings from disk. Returns a default UserSettings instance
    /// (dark=false, no favorites) if the file is absent or unreadable. Never throws.
    /// </summary>
    UserSettings Load();

    /// <summary>
    /// Persists <paramref name="settings"/> to disk. Silently swallows I/O errors
    /// so a permissions issue never crashes the app.
    /// </summary>
    void Save(UserSettings settings);
}
