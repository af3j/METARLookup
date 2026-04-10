using CommunityToolkit.Mvvm.ComponentModel;
using METARLookupWPF.Models;

namespace METARLookupWPF.ViewModels;

/// <summary>
/// Flattened, display-ready representation of a single TAF forecast period.
/// All properties are pre-formatted strings so no converters are needed in XAML.
/// Instances are immutable after construction (all properties use <c>init</c>).
/// </summary>
public partial class TafPeriodVm : ObservableObject
{
    /// <summary>UTC time range for this period (e.g. "18:00 – 00:00 Z").</summary>
    public string TimeRange { get; init; } = string.Empty;

    /// <summary>TAF change group type (e.g. "TEMPO", "BECMG", "FM"). Defaults to "BECMG" if absent.</summary>
    public string ChangeIndicator { get; init; } = string.Empty;

    /// <summary>Formatted wind string (e.g. "270° @ 12kt G20kt").</summary>
    public string Wind { get; init; } = string.Empty;

    /// <summary>Forecast visibility in statute miles (e.g. "6.0 SM").</summary>
    public string Visibility { get; init; } = string.Empty;

    /// <summary>
    /// Comma-separated sky conditions (e.g. "FEW 01500ft, BKN 05000ft").
    /// Displays "SKC" when no layers are forecast.
    /// </summary>
    public string Sky { get; init; } = string.Empty;

    /// <summary>Weather phenomena string as transmitted in the TAF (e.g. "-RA", "TSRA").</summary>
    public string Wx { get; init; } = string.Empty;

    /// <summary>
    /// Derived flight category for this period (VFR / MVFR / IFR / LIFR).
    /// Computed by <see cref="TafPeriod.FlightCategory"/> from visibility and ceiling.
    /// </summary>
    public string FlightCategory { get; init; } = string.Empty;
}

/// <summary>
/// View-model for the TAF tab. Converts the raw <see cref="Taf"/> model into
/// display strings and a list of <see cref="TafPeriodVm"/> items for the periods grid.
/// </summary>
public partial class TafViewModel : ObservableObject
{
    /// <summary>The original unparsed TAF text, displayed in the "Raw" text box.</summary>
    [ObservableProperty] private string _rawTaf = string.Empty;

    /// <summary>Formatted issue time string, e.g. "2024-03-01 12:00 Z".</summary>
    [ObservableProperty] private string _issueTime = string.Empty;

    /// <summary>Formatted valid-period string combining start/end times and date.</summary>
    [ObservableProperty] private string _validPeriod = string.Empty;

    /// <summary>Flat list of display-ready forecast period objects for the TAF periods grid.</summary>
    [ObservableProperty] private List<TafPeriodVm> _periods = [];

    /// <summary>False when no TAF was returned for the station; hides the TAF content and shows a placeholder.</summary>
    [ObservableProperty] private bool _hasTaf;

    /// <summary>
    /// Populates all TAF display properties from the given model object.
    /// Passing null clears all fields (airport has no TAF service).
    /// </summary>
    public void Load(Taf? taf)
    {
        if (taf == null)
        {
            HasTaf = false;
            RawTaf = string.Empty;
            IssueTime = string.Empty;
            ValidPeriod = string.Empty;
            Periods = [];
            return;
        }

        HasTaf = true;
        RawTaf = taf.RawText ?? string.Empty;
        IssueTime = taf.IssueTime?.ToString("yyyy-MM-dd HH:mm") + " Z" ?? string.Empty;
        ValidPeriod = $"{taf.ValidFrom:HH:mm} - {taf.ValidTo:HH:mm} Z ({taf.ValidFrom:yyyy-MM-dd})";

        Periods = taf.Periods.Select(p =>
        {
            // Wind is "—" when direction is absent (variable or calm with no direction given).
            string wind = p.WindDir.HasValue
                ? $"{p.WindDir:D3}° @ {p.WindSpeedKt}kt" + (p.WindGustsKt > 0 ? $" G{p.WindGustsKt}kt" : "")
                : "—";

            // CloudBase is stored in hundreds of feet; multiply by 100 by appending "00" in the format string.
            string sky = string.Join(", ", p.SkyConditions.Select(s =>
                s.CloudBase.HasValue ? $"{s.SkyCover} {s.CloudBase:D3}00ft" : s.SkyCover ?? ""));

            return new TafPeriodVm
            {
                TimeRange = $"{p.From:HH:mm} – {p.To:HH:mm} Z",
                ChangeIndicator = p.ChangeIndicator ?? "BECMG",
                Wind = wind,
                Visibility = p.VisibilityStatuteMi.HasValue ? $"{p.VisibilityStatuteMi:F1} SM" : "—",
                Sky = string.IsNullOrEmpty(sky) ? "SKC" : sky,
                Wx = p.Wx ?? string.Empty,
                FlightCategory = p.FlightCategory,
            };
        }).ToList();
    }
}
