namespace METARLookupWPF.Models;

/// <summary>
/// A single sky-condition layer reported in a METAR or TAF forecast period.
/// SkyCover uses standard ICAO abbreviations: FEW, SCT, BKN, OVC, SKC, CLR, or VV (vertical visibility).
/// CloudBase is in hundreds of feet AGL (e.g. 15 = 1500 ft AGL).
/// </summary>
public class SkyCondition
{
    /// <summary>Cloud coverage abbreviation (e.g. "BKN", "OVC"). "VV" indicates obscured sky with a vertical visibility value.</summary>
    public string? SkyCover { get; set; }

    /// <summary>Cloud base height in hundreds of feet above ground level. Null for SKC/CLR layers.</summary>
    public int? CloudBase { get; set; }
}

/// <summary>
/// Decoded METAR (Meteorological Aerodrome Report) for a single station.
/// A METAR is a routine aviation weather observation issued at regular intervals (usually hourly).
/// Data is sourced from the Aviation Weather Center XML API at aviationweather.gov.
/// </summary>
public class Metar
{
    /// <summary>The original, unparsed METAR string exactly as transmitted (e.g. "KSEA 101553Z 29012KT 10SM FEW030 14/07 A2985").</summary>
    public string? RawText { get; set; }

    /// <summary>ICAO station identifier (4-letter code, e.g. "KSEA").</summary>
    public string? StationId { get; set; }

    /// <summary>UTC time the observation was taken.</summary>
    public DateTime? ObservationTime { get; set; }

    /// <summary>Outside air temperature in degrees Celsius.</summary>
    public double? TempC { get; set; }

    /// <summary>Dew point temperature in degrees Celsius. Used to assess moisture and fog likelihood.</summary>
    public double? DewpointC { get; set; }

    /// <summary>Magnetic wind direction in degrees (0–360). Null or absent if wind is variable.</summary>
    public int? WindDir { get; set; }

    /// <summary>Sustained wind speed in knots.</summary>
    public int? WindSpeedKt { get; set; }

    /// <summary>Wind gust speed in knots. Zero or null means no gusts reported.</summary>
    public int? WindGustsKt { get; set; }

    /// <summary>Prevailing visibility in statute miles.</summary>
    public double? VisibilityStatuteMi { get; set; }

    /// <summary>Altimeter setting in inches of mercury (in Hg), used to set the altimeter datum.</summary>
    public double? AltimeterInHg { get; set; }

    /// <summary>
    /// FAA/NWS flight category: VFR, MVFR, IFR, or LIFR.
    /// Determined by ceiling and visibility: LIFR &lt;500ft/&lt;1SM, IFR &lt;1000ft/&lt;3SM,
    /// MVFR ≤3000ft/≤5SM, VFR above all those thresholds.
    /// </summary>
    public string? FlightCategory { get; set; }

    /// <summary>Airport field elevation in meters above mean sea level.</summary>
    public double? ElevationMeter { get; set; }

    /// <summary>Ordered list of reported sky-condition layers, lowest first.</summary>
    public List<SkyCondition> SkyConditions { get; set; } = [];

    /// <summary>
    /// Altimeter setting converted to QNH (hectopascals / millibars).
    /// Used outside the US where hPa is the standard unit (1 in Hg ≈ 33.8639 hPa).
    /// </summary>
    public double? AltimeterQnh => AltimeterInHg.HasValue ? Math.Round(AltimeterInHg.Value * 33.8639, 1) : null;

    /// <summary>Airport elevation converted from metres to feet for display and pressure-altitude calculations.</summary>
    public double? ElevationFeet => ElevationMeter.HasValue ? Math.Round(ElevationMeter.Value * 3.28084, 0) : null;
}
