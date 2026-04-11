namespace METARLookupWPF.Models;

/// <summary>
/// Serializable snapshot of a pinned favorite, stored in settings.json.
/// Deliberately separate from FavoriteStation so the on-disk format is
/// decoupled from the ObservableObject runtime model.
/// </summary>
public class SavedFavorite
{
    public string Icao { get; set; } = string.Empty;
    public string FlightCategory { get; set; } = string.Empty;
}

/// <summary>
/// Root settings object persisted to %APPDATA%\METARLookupWPF\settings.json.
/// New fields added here are backward-compatible: missing JSON keys deserialize
/// to the property's default value.
/// </summary>
public class UserSettings
{
    public bool IsDarkTheme { get; set; }
    public List<SavedFavorite> Favorites { get; set; } = [];
}
