namespace METARLookupWPF.Models;

/// <summary>
/// Represents an AIRMET or SIGMET advisory retrieved from the Aviation Weather Center.
/// SIGMETs (Significant Meteorological Information) warn of severe hazards such as turbulence,
/// icing, volcanic ash, or tropical cyclones. AIRMETs cover less severe but still significant
/// hazards for lighter aircraft (IFR conditions, moderate turbulence/icing, mountain obscuration).
/// Data is sourced from the aviationweather.gov XML API endpoint.
/// </summary>
public class Sigmet
{
    /// <summary>Unique identifier assigned to this advisory by the issuing centre.</summary>
    public string? SigmetId { get; set; }

    /// <summary>Advisory type: "SIGMET" or "AIRMET". Determines the severity level of the hazard.</summary>
    public string? AirSigmetType { get; set; }  // SIGMET, AIRMET

    /// <summary>Primary hazard type: "ICE" (icing), "TURB" (turbulence), "IFR" (instrument conditions), etc.</summary>
    public string? Hazard { get; set; }          // ICE, TURB, IFR, etc.

    /// <summary>Intensity of the hazard (e.g. "MOD" for moderate, "SEV" for severe). May be null if not specified.</summary>
    public string? Severity { get; set; }

    /// <summary>UTC time from which this advisory is active.</summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>UTC time at which this advisory expires.</summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>Lower boundary of the affected altitude block in feet above mean sea level.</summary>
    public int? MinAltFtMsl { get; set; }

    /// <summary>Upper boundary of the affected altitude block in feet above mean sea level.</summary>
    public int? MaxAltFtMsl { get; set; }

    /// <summary>Original unparsed advisory text as issued.</summary>
    public string? RawText { get; set; }

    /// <summary>Direction the hazard area is moving, expressed as a compass bearing string (e.g. "NE").</summary>
    public string? MovementDir { get; set; }

    /// <summary>Speed at which the hazard area is moving in knots.</summary>
    public int? MovementSpeedKt { get; set; }
}
