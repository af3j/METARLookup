using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METARLookupWPF.Models;

namespace METARLookupWPF.ViewModels;

/// <summary>
/// View-model for the METAR tab. Holds the raw model objects and exposes
/// formatted string properties that the XAML bindings consume directly,
/// eliminating the need for value converters for most display logic.
/// Also manages ATIS selection (arrival vs. departure vs. combined).
/// </summary>
public partial class MetarViewModel : ObservableObject
{
    /// <summary>The current METAR observation. Null until a successful lookup.</summary>
    [ObservableProperty] private Metar? _metar;

    /// <summary>Airport metadata returned alongside the METAR. Null if the airport lookup failed.</summary>
    [ObservableProperty] private Airport? _airport;

    /// <summary>Full text of the currently displayed ATIS broadcast.</summary>
    [ObservableProperty] private string _atisText = string.Empty;

    /// <summary>Which ATIS type is selected: "arr" (arrival) or "dep" (departure).</summary>
    [ObservableProperty] private string _selectedAtisType = "arr";

    // Stored so ShowAtis can switch between arrival and departure without a new API call.
    private List<Atis> _atisList = [];

    // ── Formatted display properties ──────────────────────────────────────────
    // These are computed properties; they do not raise their own PropertyChanged events.
    // Instead, NotifyAll() raises them all at once after every Load() call.

    /// <summary>Flight category string (VFR / MVFR / IFR / LIFR / "—" if no data).</summary>
    public string FlightCategoryText => Metar?.FlightCategory ?? "—";

    public string StationId => Metar?.StationId ?? "—";
    public string AirportName => Airport?.Name ?? "—";
    public string AirportLocation => Airport?.Location ?? "—";

    /// <summary>The full raw METAR text string as received from the AWC API.</summary>
    public string RawMetar => Metar?.RawText ?? string.Empty;

    public string ObsDate => Metar?.ObservationTime?.ToString("yyyy-MM-dd") ?? "—";

    /// <summary>Observation time formatted as HH:mm:ss Z (Zulu/UTC), matching how pilots read METARs.</summary>
    public string ObsTime => Metar?.ObservationTime?.ToString("HH:mm:ss") + " Z" ?? "—";

    public string TempC => Metar?.TempC?.ToString("F1") ?? "—";
    public string DewC => Metar?.DewpointC?.ToString("F1") ?? "—";

    /// <summary>
    /// Wind direction formatted as a zero-padded 3-digit bearing (e.g. "270°").
    /// Displays "VRB" when WindDir is null, indicating variable winds.
    /// </summary>
    public string WindDir => Metar?.WindDir.HasValue == true ? $"{Metar.WindDir:D3}°" : "VRB";

    public string WindSpeed => Metar?.WindSpeedKt?.ToString() ?? "—";

    /// <summary>Formatted gust component (e.g. "G25kt"). Returns empty string when no gusts are reported.</summary>
    public string WindGusts => (Metar?.WindGustsKt ?? 0) > 0 ? $"G{Metar!.WindGustsKt}kt" : string.Empty;

    public string Visibility => Metar?.VisibilityStatuteMi?.ToString("F1") ?? "—";

    /// <summary>Altimeter setting in inches of mercury (US standard, e.g. "29.92").</summary>
    public string AltInHg => Metar?.AltimeterInHg?.ToString("F2") ?? "—";

    /// <summary>Altimeter setting converted to QNH in hectopascals (used outside the US).</summary>
    public string AltQnh => Metar?.AltimeterQnh?.ToString("F1") ?? "—";

    public string ElevMeters => Metar?.ElevationMeter?.ToString("F0") ?? "—";
    public string ElevFeet => Metar?.ElevationFeet?.ToString("F0") ?? "—";

    /// <summary>
    /// Multiline string of sky-condition layers (one per line), e.g. "FEW 02500 ft AGL\nBKN 05000 ft AGL".
    /// Cloud base values are stored in hundreds of feet, so the D3 format pads to 3 digits before
    /// appending "00" to reconstruct the actual altitude (e.g. CloudBase=25 → "02500 ft AGL").
    /// </summary>
    public string SkyConditions => Metar == null ? string.Empty :
        string.Join("\n", Metar.SkyConditions.Select(s =>
            s.CloudBase.HasValue ? $"{s.SkyCover} {s.CloudBase:D3}00 ft AGL" : s.SkyCover ?? string.Empty));

    // ── Data loading ──────────────────────────────────────────────────────────

    /// <summary>
    /// Called by <see cref="MainViewModel.FetchAllAsync"/> after every successful lookup.
    /// Replaces all model data and triggers a batch property-change notification.
    /// </summary>
    public void Load(Metar? metar, Airport? airport, List<Atis> atis)
    {
        Metar = metar;
        Airport = airport;
        _atisList = atis;
        ShowAtis(SelectedAtisType);
        NotifyAll();
    }

    // ── ATIS handling ─────────────────────────────────────────────────────────

    /// <summary>
    /// Selects which ATIS type to display. Falls back to "combined" if the requested
    /// type isn't available, then to any available ATIS as a last resort.
    /// Bound to the Arrival/Departure toggle buttons in MetarView.
    /// </summary>
    [RelayCommand]
    private void ShowAtis(string type)
    {
        SelectedAtisType = type;
        var match = _atisList.FirstOrDefault(a => a.Type == type)
                    ?? _atisList.FirstOrDefault(a => a.Type == "combined")
                    ?? _atisList.FirstOrDefault();

        AtisText = match?.Datis ?? (type == "arr" ? "Arrival ATIS not available." : "Departure ATIS not available.");
    }

    // ── Property change notification ──────────────────────────────────────────

    /// <summary>
    /// Raises PropertyChanged for all computed display properties at once.
    /// Because these properties are plain getters (not [ObservableProperty] fields),
    /// the source generator cannot track them automatically.
    /// </summary>
    private void NotifyAll()
    {
        OnPropertyChanged(nameof(FlightCategoryText));
        OnPropertyChanged(nameof(StationId));
        OnPropertyChanged(nameof(AirportName));
        OnPropertyChanged(nameof(AirportLocation));
        OnPropertyChanged(nameof(RawMetar));
        OnPropertyChanged(nameof(ObsDate));
        OnPropertyChanged(nameof(ObsTime));
        OnPropertyChanged(nameof(TempC));
        OnPropertyChanged(nameof(DewC));
        OnPropertyChanged(nameof(WindDir));
        OnPropertyChanged(nameof(WindSpeed));
        OnPropertyChanged(nameof(WindGusts));
        OnPropertyChanged(nameof(Visibility));
        OnPropertyChanged(nameof(AltInHg));
        OnPropertyChanged(nameof(AltQnh));
        OnPropertyChanged(nameof(ElevMeters));
        OnPropertyChanged(nameof(ElevFeet));
        OnPropertyChanged(nameof(SkyConditions));
    }
}
