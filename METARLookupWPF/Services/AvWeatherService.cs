using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

/// <summary>
/// Fetches aviation weather data (METAR, TAF, SIGMET/AIRMET) from the
/// FAA/NOAA Aviation Weather Center REST API at aviationweather.gov.
/// All endpoints return XML, which is parsed with LINQ-to-XML.
/// A single shared <see cref="HttpClient"/> instance is injected via the constructor.
/// </summary>
public class AvWeatherService(HttpClient http) : IAvWeatherService
{
    // Reused options instance avoids allocating per-request when deserializing JSON (used by helper methods below).
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    /// <inheritdoc/>
    public async Task<Metar?> GetMetarAsync(string icao, CancellationToken ct = default)
    {
        // hours=3 ensures we get a result even if the station is slightly late reporting.
        // The API returns observations most-recent-first, so FirstOrDefault below picks the latest.
        var results = await FetchMetarsXmlAsync(
            $"https://aviationweather.gov/api/data/metar?ids={icao}&hours=3&format=xml", ct);
        return results.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task<List<Metar>> GetNearbyMetarsAsync(double lat, double lon, double radiusDeg = 1.0, CancellationToken ct = default)
    {
        // Build an axis-aligned bounding box from the degree radius.
        // At typical latitudes 1° ≈ 60–70 nm, so radiusDeg=1 captures airports within ~70 nm.
        double minLat = lat - radiusDeg, maxLat = lat + radiusDeg;
        double minLon = lon - radiusDeg, maxLon = lon + radiusDeg;

        // AWC bbox format is minLat,minLon,maxLat,maxLon (lat-first).
        // InvariantCulture ensures decimal separators are always '.' regardless of OS locale.
        string bbox = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", minLat, minLon, maxLat, maxLon);
        return await FetchMetarsXmlAsync(
            $"https://aviationweather.gov/api/data/metar?bbox={bbox}&hours=2&format=xml", ct);
    }

    /// <inheritdoc/>
    public async Task<Taf?> GetTafAsync(string icao, CancellationToken ct = default)
    {
        var url = $"https://aviationweather.gov/api/data/taf?ids={icao}&format=xml";
        using var response = await http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode) return null;

        var xml = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(xml)) return null;

        var doc = XDocument.Parse(xml);
        // The AWC XML response wraps TAFs in <TAF> elements under a <data> root.
        var tafEl = doc.Descendants("TAF").FirstOrDefault();
        if (tafEl == null) return null;

        var taf = new Taf
        {
            StationId = tafEl.Element("station_id")?.Value,
            RawText = tafEl.Element("raw_text")?.Value,
            IssueTime = ParseDateTime(tafEl.Element("issue_time")?.Value),
            ValidFrom = ParseDateTime(tafEl.Element("valid_time_from")?.Value),
            ValidTo = ParseDateTime(tafEl.Element("valid_time_to")?.Value),
        };

        // Each <forecast> child element is one TAF period (base or change group).
        foreach (var fc in tafEl.Elements("forecast"))
        {
            var period = new TafPeriod
            {
                From = ParseDateTime(fc.Element("fcst_time_from")?.Value),
                To = ParseDateTime(fc.Element("fcst_time_to")?.Value),
                ChangeIndicator = fc.Element("change_indicator")?.Value,
                Wx = fc.Element("wx_string")?.Value,
            };

            // Numeric fields use TryParse to silently ignore missing or malformed XML values.
            if (int.TryParse(fc.Element("wind_dir_degrees")?.Value, out int wdir)) period.WindDir = wdir;
            if (int.TryParse(fc.Element("wind_speed_kt")?.Value, out int wspd)) period.WindSpeedKt = wspd;
            if (int.TryParse(fc.Element("wind_gust_kt")?.Value, out int wgust)) period.WindGustsKt = wgust;
            if (double.TryParse(fc.Element("visibility_statute_mi")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double vis)) period.VisibilityStatuteMi = vis;

            // Sky conditions are XML attributes on <sky_condition> child elements.
            foreach (var sky in fc.Elements("sky_condition"))
            {
                var sc = new SkyCondition { SkyCover = sky.Attribute("sky_cover")?.Value };
                if (int.TryParse(sky.Attribute("cloud_base_ft_agl")?.Value, out int cb)) sc.CloudBase = cb;
                period.SkyConditions.Add(sc);
            }

            taf.Periods.Add(period);
        }

        return taf;
    }

    /// <inheritdoc/>
    public async Task<List<Sigmet>> GetSigmetsAsync(CancellationToken ct = default)
    {
        // The AWC SIGMET endpoint returns both SIGMETs and AIRMETs as <AIRSIGMET> elements.
        var url = "https://aviationweather.gov/api/data/sigmet?format=xml";
        using var response = await http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode) return [];

        var xml = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(xml)) return [];

        var doc = XDocument.Parse(xml);
        var list = new List<Sigmet>();

        foreach (var el in doc.Descendants("AIRSIGMET"))
        {
            var s = new Sigmet
            {
                SigmetId = el.Element("airsigmet_id")?.Value,
                AirSigmetType = el.Element("airsigmet_type")?.Value,
                RawText = el.Element("raw_text")?.Value,
                ValidFrom = ParseDateTime(el.Element("valid_time_from")?.Value),
                ValidTo = ParseDateTime(el.Element("valid_time_to")?.Value),
            };

            // Hazard type and severity are XML attributes on the <hazard> child element.
            var hazard = el.Element("hazard");
            if (hazard != null)
            {
                s.Hazard = hazard.Attribute("type")?.Value;
                s.Severity = hazard.Attribute("severity")?.Value;
            }

            // Altitude bounds are attributes on the <altitude> element (feet MSL).
            if (int.TryParse(el.Element("altitude")?.Attribute("min_ft_msl")?.Value, out int minAlt)) s.MinAltFtMsl = minAlt;
            if (int.TryParse(el.Element("altitude")?.Attribute("max_ft_msl")?.Value, out int maxAlt)) s.MaxAltFtMsl = maxAlt;

            // Movement direction and speed are optional; some advisories are quasi-stationary.
            var movement = el.Element("movement");
            if (movement != null)
            {
                s.MovementDir = movement.Attribute("direction")?.Value;
                if (int.TryParse(movement.Attribute("speed_kts")?.Value, out int spd)) s.MovementSpeedKt = spd;
            }

            list.Add(s);
        }

        return list;
    }

    /// <summary>
    /// Shared helper that fetches an AWC METAR XML URL and returns parsed <see cref="Metar"/> objects.
    /// Used by both single-station and bounding-box queries to avoid code duplication.
    /// </summary>
    private async Task<List<Metar>> FetchMetarsXmlAsync(string url, CancellationToken ct)
    {
        using var response = await http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode) return [];

        var xml = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(xml)) return [];

        var doc = XDocument.Parse(xml);
        var list = new List<Metar>();

        foreach (var el in doc.Descendants("METAR"))
        {
            var m = new Metar
            {
                RawText = el.Element("raw_text")?.Value,
                StationId = el.Element("station_id")?.Value,
                ObservationTime = ParseDateTime(el.Element("observation_time")?.Value),
                // FlightCategory is pre-computed by the AWC API; we store it directly.
                FlightCategory = el.Element("flight_category")?.Value,
            };

            // All numeric fields use InvariantCulture to handle decimal points correctly
            // across all OS locale settings.
            if (double.TryParse(el.Element("temp_c")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double temp)) m.TempC = temp;
            if (double.TryParse(el.Element("dewpoint_c")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double dew)) m.DewpointC = dew;
            if (int.TryParse(el.Element("wind_dir_degrees")?.Value, out int wdir)) m.WindDir = wdir;
            if (int.TryParse(el.Element("wind_speed_kt")?.Value, out int wspd)) m.WindSpeedKt = wspd;
            if (int.TryParse(el.Element("wind_gust_kt")?.Value, out int wgust)) m.WindGustsKt = wgust;
            if (double.TryParse(el.Element("visibility_statute_mi")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double vis)) m.VisibilityStatuteMi = vis;
            if (double.TryParse(el.Element("altim_in_hg")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double alt)) m.AltimeterInHg = alt;
            if (double.TryParse(el.Element("elevation_m")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double elev)) m.ElevationMeter = elev;
            // Try both old ADDS element names ("latitude"/"longitude") and newer API names ("lat"/"lon").
            var latStr = el.Element("latitude")?.Value ?? el.Element("lat")?.Value;
            var lonStr = el.Element("longitude")?.Value ?? el.Element("lon")?.Value;
            if (double.TryParse(latStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double mlat)) m.Latitude  = mlat;
            if (double.TryParse(lonStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double mlon)) m.Longitude = mlon;

            // Sky conditions are XML attributes on each <sky_condition> element.
            foreach (var sky in el.Elements("sky_condition"))
            {
                var sc = new SkyCondition { SkyCover = sky.Attribute("sky_cover")?.Value };
                if (int.TryParse(sky.Attribute("cloud_base_ft_agl")?.Value, out int cb)) sc.CloudBase = cb;
                m.SkyConditions.Add(sc);
            }

            list.Add(m);
        }

        return list;
    }

    /// <summary>
    /// Parses an ISO 8601 date-time string from the AWC XML API into a UTC <see cref="DateTime"/>.
    /// Returns null for missing or unparseable values rather than throwing.
    /// </summary>
    private static DateTime? ParseDateTime(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt) ? dt : null;
    }

    // ── JSON helpers ─────────────────────────────────────────────────────────
    // These are retained for potential future use with JSON-format AWC endpoints.

    private static string? JsonStr(JsonElement el, string key) =>
        el.TryGetProperty(key, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;

    private static bool JsonDouble(JsonElement el, string key, out double value)
    {
        value = 0;
        return el.TryGetProperty(key, out var p)
            && p.ValueKind == JsonValueKind.Number
            && p.TryGetDouble(out value);
    }

    private static bool JsonInt(JsonElement el, string key, out int value)
    {
        value = 0;
        return el.TryGetProperty(key, out var p)
            && p.ValueKind == JsonValueKind.Number
            && p.TryGetInt32(out value);
    }

    /// <summary>
    /// Parses visibility strings returned by the JSON API:
    /// "10+", "7", "1 1/2", "3/4", "1/4", etc.
    /// </summary>
    private static double? ParseVisibility(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        // The trailing '+' on "10+" means "10 or more"; strip it and treat as the numeric value.
        s = s.Replace("+", "").Trim();

        // Whole number with fractional part, e.g. "1 1/2"
        var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2 &&
            double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var whole) &&
            TryParseFraction(parts[1], out var frac))
            return whole + frac;

        // Pure fraction, e.g. "3/4"
        if (parts.Length == 1 && TryParseFraction(parts[0], out var f)) return f;

        // Plain decimal/integer
        return double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null;
    }

    /// <summary>
    /// Attempts to parse a fraction string such as "3/4" into a double.
    /// Returns false if the string is not in numerator/denominator form.
    /// </summary>
    private static bool TryParseFraction(string s, out double value)
    {
        value = 0;
        var slash = s.IndexOf('/');
        if (slash < 1) return false;
        if (!double.TryParse(s[..slash],    NumberStyles.Any, CultureInfo.InvariantCulture, out var num)) return false;
        if (!double.TryParse(s[(slash+1)..], NumberStyles.Any, CultureInfo.InvariantCulture, out var den) || den == 0) return false;
        value = num / den;
        return true;
    }
}
