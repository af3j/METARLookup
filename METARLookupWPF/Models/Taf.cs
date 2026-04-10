namespace METARLookupWPF.Models;

public class TafPeriod
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? ChangeIndicator { get; set; }
    public int? WindDir { get; set; }
    public int? WindSpeedKt { get; set; }
    public int? WindGustsKt { get; set; }
    public double? VisibilityStatuteMi { get; set; }
    public string? Wx { get; set; }
    public List<SkyCondition> SkyConditions { get; set; } = [];

    public string FlightCategory
    {
        get
        {
            if (!VisibilityStatuteMi.HasValue) return "Unknown";
            double vis = VisibilityStatuteMi.Value;
            int? ceiling = SkyConditions
                .Where(s => s.SkyCover is "BKN" or "OVC" or "VV")
                .Min(s => s.CloudBase);

            if (vis < 1 || (ceiling.HasValue && ceiling < 500)) return "LIFR";
            if (vis < 3 || (ceiling.HasValue && ceiling < 1000)) return "IFR";
            if (vis <= 5 || (ceiling.HasValue && ceiling <= 3000)) return "MVFR";
            return "VFR";
        }
    }
}

public class Taf
{
    public string? StationId { get; set; }
    public string? RawText { get; set; }
    public DateTime? IssueTime { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public List<TafPeriod> Periods { get; set; } = [];
}
