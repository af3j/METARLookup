using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

public class AvWeatherService(HttpClient http) : IAvWeatherService
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<Metar?> GetMetarAsync(string icao, CancellationToken ct = default)
    {
        var results = await FetchMetarsXmlAsync(
            $"https://aviationweather.gov/api/data/metar?ids={icao}&hoursBeforeNow=3&format=xml&mostRecent=true", ct);
        return results.FirstOrDefault();
    }

    public async Task<List<Metar>> GetMetarHistoryAsync(string icao, int hoursBeforeNow = 24, CancellationToken ct = default)
    {
        return await FetchMetarsXmlAsync(
            $"https://aviationweather.gov/api/data/metar?ids={icao}&hoursBeforeNow={hoursBeforeNow}&format=xml", ct);
    }

    public async Task<List<Metar>> GetNearbyMetarsAsync(double lat, double lon, double radiusDeg = 1.0, CancellationToken ct = default)
    {
        double minLat = lat - radiusDeg, maxLat = lat + radiusDeg;
        double minLon = lon - radiusDeg, maxLon = lon + radiusDeg;
        string bbox = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", minLon, minLat, maxLon, maxLat);
        return await FetchMetarsXmlAsync(
            $"https://aviationweather.gov/api/data/metar?bbox={bbox}&hoursBeforeNow=2&format=xml&mostRecent=true", ct);
    }

    public async Task<Taf?> GetTafAsync(string icao, CancellationToken ct = default)
    {
        var url = $"https://aviationweather.gov/api/data/taf?ids={icao}&format=xml";
        using var response = await http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode) return null;

        var xml = await response.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(xml)) return null;

        var doc = XDocument.Parse(xml);
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

        foreach (var fc in tafEl.Elements("forecast"))
        {
            var period = new TafPeriod
            {
                From = ParseDateTime(fc.Element("fcst_time_from")?.Value),
                To = ParseDateTime(fc.Element("fcst_time_to")?.Value),
                ChangeIndicator = fc.Element("change_indicator")?.Value,
                Wx = fc.Element("wx_string")?.Value,
            };

            if (int.TryParse(fc.Element("wind_dir_degrees")?.Value, out int wdir)) period.WindDir = wdir;
            if (int.TryParse(fc.Element("wind_speed_kt")?.Value, out int wspd)) period.WindSpeedKt = wspd;
            if (int.TryParse(fc.Element("wind_gust_kt")?.Value, out int wgust)) period.WindGustsKt = wgust;
            if (double.TryParse(fc.Element("visibility_statute_mi")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double vis)) period.VisibilityStatuteMi = vis;

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

    public async Task<List<Sigmet>> GetSigmetsAsync(CancellationToken ct = default)
    {
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

            var hazard = el.Element("hazard");
            if (hazard != null)
            {
                s.Hazard = hazard.Attribute("type")?.Value;
                s.Severity = hazard.Attribute("severity")?.Value;
            }

            if (int.TryParse(el.Element("altitude")?.Attribute("min_ft_msl")?.Value, out int minAlt)) s.MinAltFtMsl = minAlt;
            if (int.TryParse(el.Element("altitude")?.Attribute("max_ft_msl")?.Value, out int maxAlt)) s.MaxAltFtMsl = maxAlt;

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
                FlightCategory = el.Element("flight_category")?.Value,
            };

            if (double.TryParse(el.Element("temp_c")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double temp)) m.TempC = temp;
            if (double.TryParse(el.Element("dewpoint_c")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double dew)) m.DewpointC = dew;
            if (int.TryParse(el.Element("wind_dir_degrees")?.Value, out int wdir)) m.WindDir = wdir;
            if (int.TryParse(el.Element("wind_speed_kt")?.Value, out int wspd)) m.WindSpeedKt = wspd;
            if (int.TryParse(el.Element("wind_gust_kt")?.Value, out int wgust)) m.WindGustsKt = wgust;
            if (double.TryParse(el.Element("visibility_statute_mi")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double vis)) m.VisibilityStatuteMi = vis;
            if (double.TryParse(el.Element("altim_in_hg")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double alt)) m.AltimeterInHg = alt;
            if (double.TryParse(el.Element("elevation_m")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double elev)) m.ElevationMeter = elev;

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

    private static DateTime? ParseDateTime(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt) ? dt : null;
    }
}
