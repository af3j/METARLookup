namespace METARLookupWPF.Models;

public class SkyCondition
{
    public string? SkyCover { get; set; }
    public int? CloudBase { get; set; }
}

public class Metar
{
    public string? RawText { get; set; }
    public string? StationId { get; set; }
    public DateTime? ObservationTime { get; set; }
    public double? TempC { get; set; }
    public double? DewpointC { get; set; }
    public int? WindDir { get; set; }
    public int? WindSpeedKt { get; set; }
    public int? WindGustsKt { get; set; }
    public double? VisibilityStatuteMi { get; set; }
    public double? AltimeterInHg { get; set; }
    public string? FlightCategory { get; set; }
    public double? ElevationMeter { get; set; }
    public List<SkyCondition> SkyConditions { get; set; } = [];

    public double? AltimeterQnh => AltimeterInHg.HasValue ? Math.Round(AltimeterInHg.Value * 33.8639, 1) : null;
    public double? ElevationFeet => ElevationMeter.HasValue ? Math.Round(ElevationMeter.Value * 3.28084, 0) : null;
}
