namespace METARLookupWPF.Models;

/// <summary>
/// A single forecast period within a TAF (Terminal Aerodrome Forecast).
/// Each period covers a contiguous time window and may represent the base forecast
/// or a change group (BECMG, TEMPO, FM, PROB).
/// </summary>
public class TafPeriod
{
    /// <summary>Start of the forecast period in UTC.</summary>
    public DateTime? From { get; set; }

    /// <summary>End of the forecast period in UTC.</summary>
    public DateTime? To { get; set; }

    /// <summary>
    /// TAF change group type: BECMG (becoming), TEMPO (temporary), FM (from), PROB (probability), etc.
    /// Null on the initial base forecast period.
    /// </summary>
    public string? ChangeIndicator { get; set; }

    /// <summary>Forecast wind direction in degrees magnetic.</summary>
    public int? WindDir { get; set; }

    /// <summary>Forecast sustained wind speed in knots.</summary>
    public int? WindSpeedKt { get; set; }

    /// <summary>Forecast wind gust speed in knots. Zero or null means no gusts forecast.</summary>
    public int? WindGustsKt { get; set; }

    /// <summary>Forecast prevailing visibility in statute miles.</summary>
    public double? VisibilityStatuteMi { get; set; }

    /// <summary>Weather phenomena string (e.g. "-RA", "TSRA", "+SN"). May be null for clear conditions.</summary>
    public string? Wx { get; set; }

    /// <summary>Forecast sky condition layers, lowest first.</summary>
    public List<SkyCondition> SkyConditions { get; set; } = [];

    /// <summary>
    /// Derives the FAA flight category for this forecast period from visibility and ceiling.
    /// The ceiling is the lowest BKN, OVC, or VV layer. Thresholds:
    /// LIFR: vis &lt;1SM or ceiling &lt;500ft; IFR: vis &lt;3SM or ceiling &lt;1000ft;
    /// MVFR: vis ≤5SM or ceiling ≤3000ft; VFR: above all thresholds.
    /// </summary>
    public string FlightCategory
    {
        get
        {
            if (!VisibilityStatuteMi.HasValue) return "Unknown";
            double vis = VisibilityStatuteMi.Value;

            // The operational ceiling is the lowest broken, overcast, or vertical-visibility layer.
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

/// <summary>
/// A Terminal Aerodrome Forecast (TAF): an aviation weather forecast issued for a specific airport,
/// valid for 24 or 30 hours and updated every 6 hours by the NWS.
/// Data is sourced from the Aviation Weather Center XML API.
/// </summary>
public class Taf
{
    /// <summary>ICAO station identifier the forecast was issued for.</summary>
    public string? StationId { get; set; }

    /// <summary>The original unparsed TAF text string as transmitted.</summary>
    public string? RawText { get; set; }

    /// <summary>UTC time this TAF was issued by the forecasting office.</summary>
    public DateTime? IssueTime { get; set; }

    /// <summary>Start of the TAF validity window in UTC.</summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>End of the TAF validity window in UTC.</summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// Ordered list of forecast periods. The first entry is the initial (base) forecast;
    /// subsequent entries are change groups in chronological order.
    /// </summary>
    public List<TafPeriod> Periods { get; set; } = [];
}
