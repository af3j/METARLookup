namespace METARLookupWPF.Models;

public class Sigmet
{
    public string? SigmetId { get; set; }
    public string? AirSigmetType { get; set; }  // SIGMET, AIRMET
    public string? Hazard { get; set; }          // ICE, TURB, IFR, etc.
    public string? Severity { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public int? MinAltFtMsl { get; set; }
    public int? MaxAltFtMsl { get; set; }
    public string? RawText { get; set; }
    public string? MovementDir { get; set; }
    public int? MovementSpeedKt { get; set; }
}
