using System.Net.Http;
using System.Xml.Linq;
using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

/// <summary>
/// Fetches FAA Terminal Procedures Publication (d-TPP) charts for a given airport
/// by downloading and caching the AIRAC cycle metafile XML from aeronav.faa.gov.
/// The metafile lists every chart PDF for every US airport in the current 28-day AIRAC cycle.
/// </summary>
public class FaaChartsService(HttpClient http) : IFaaChartsService
{
    // The metafile is several MB; cache the parsed XDocument in memory so subsequent
    // chart lookups (different airports in the same session) don't re-download it.
    private string?    _cachedCycle;
    private XDocument? _cachedMetafile;

    // SemaphoreSlim(1,1) used as an async-safe mutex to prevent concurrent metafile fetches.
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>Maps FAA chart-code abbreviations to user-friendly display names.</summary>
    private static readonly Dictionary<string, string> CategoryNames = new()
    {
        ["APD"]  = "Airport Diagram",
        ["IAP"]  = "Instrument Approaches",
        ["DP"]   = "Departures (DP)",
        ["STAR"] = "Arrivals (STAR)",
        ["MIN"]  = "Minimums",
        ["HOT"]  = "Hot Spots",
        ["LAH"]  = "Land & Hold Short",
    };

    /// <inheritdoc/>
    public async Task<List<ChartGroup>> GetAirportChartsAsync(string icao, CancellationToken ct = default)
    {
        var (cycle, metafile) = await GetMetafileAsync(ct);
        if (metafile == null) return [];

        // The metafile uses both "icao_ident" (4-letter ICAO) and "apt_ident" (FAA identifier,
        // which omits the leading 'K' or 'P' prefix for US/Pacific airports).
        // Try ICAO first; fall back to the stripped FAA identifier for broader compatibility.
        var airportEl =
            metafile.Descendants("airport_name")
                    .FirstOrDefault(el => string.Equals(
                        el.Attribute("icao_ident")?.Value, icao, StringComparison.OrdinalIgnoreCase))
            ?? metafile.Descendants("airport_name")
                    .FirstOrDefault(el => string.Equals(
                        el.Attribute("apt_ident")?.Value,
                        icao.TrimStart('K', 'P'), StringComparison.OrdinalIgnoreCase));

        if (airportEl == null) return [];

        // PDF files live under the cycle-versioned directory on aeronav.faa.gov.
        var baseUrl = $"https://aeronav.faa.gov/d-tpp/{cycle}/";

        // Group chart records by chart_code, apply display-name mapping and sort order,
        // then project each group into a ChartGroup with its list of AirportChart objects.
        return airportEl.Elements("record")
            .GroupBy(r => r.Element("chart_code")?.Value ?? "OTHER")
            .OrderBy(g => ChartOrder(g.Key))
            .Select(g => new ChartGroup
            {
                Category = CategoryNames.TryGetValue(g.Key, out var n) ? n : g.Key,
                Charts   = g.Select(r => new AirportChart
                {
                    ChartCode = g.Key,
                    Name      = r.Element("chart_name")?.Value ?? "",
                    PdfUrl    = baseUrl + r.Element("pdf_name")?.Value
                }).ToList()
            })
            .Where(g => g.Charts.Count > 0)
            .ToList();
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Returns the cached metafile for the current AIRAC cycle, fetching it if necessary.
    /// The SemaphoreSlim prevents a thundering-herd if multiple tabs request charts simultaneously.
    /// </summary>
    private async Task<(string cycle, XDocument? doc)> GetMetafileAsync(CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var cycle = _cachedCycle ?? CalculateCurrentCycle();

            if (_cachedMetafile == null || _cachedCycle != cycle)
            {
                // Try current cycle; fall back one if the file isn't published yet.
                // The FAA typically publishes the new metafile on the cycle effective date,
                // but there is sometimes a brief window where the previous cycle is still current.
                _cachedMetafile = await TryFetchMetafileAsync(cycle, ct)
                               ?? await TryFetchMetafileAsync(PreviousCycle(cycle), ct);
                _cachedCycle = cycle;
            }

            return (_cachedCycle!, _cachedMetafile);
        }
        finally { _lock.Release(); }
    }

    /// <summary>
    /// Attempts to fetch and parse the d-TPP metafile XML for the given AIRAC cycle string.
    /// Returns null on any failure so the caller can try the previous cycle.
    /// </summary>
    private async Task<XDocument?> TryFetchMetafileAsync(string cycle, CancellationToken ct)
    {
        try
        {
            var url = $"https://aeronav.faa.gov/d-tpp/{cycle}/xml_data/d-TPP_Metafile.xml";
            using var resp = await http.GetAsync(url, ct);
            if (!resp.IsSuccessStatusCode) return null;
            var xml = await resp.Content.ReadAsStringAsync(ct);
            return XDocument.Parse(xml);
        }
        catch { return null; }
    }

    /// <summary>
    /// Calculates the current AIRAC cycle string (e.g. "2603").
    /// Reference: cycle 2401 effective January 25, 2024.  Each cycle = 28 days.
    /// AIRAC cycles are numbered sequentially within each calendar year (01–13),
    /// then reset to 01 at the start of the next year's first cycle.
    /// Note: the shared HttpClient has a 15-second timeout; for large metafile XML (several MB)
    /// this may be tight on slow connections, but we rely on the cached result after the first fetch.
    /// </summary>
    private static string CalculateCurrentCycle()
    {
        var refDate = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Utc);
        var days    = Math.Max(0, (DateTime.UtcNow - refDate).Days);
        var total   = days / 28;         // total cycles elapsed since 2401

        // Walk forward year-by-year (13 cycles per year) to find the current year and cycle index.
        int year = 2024, idx = total;
        while (idx >= 13) { idx -= 13; year++; }
        return $"{year % 100:D2}{idx + 1:D2}";
    }

    /// <summary>
    /// Returns the cycle string immediately before <paramref name="cycle"/>.
    /// Handles year rollover: cycle "XX01" rolls back to "(XX-1)13".
    /// </summary>
    private static string PreviousCycle(string cycle)
    {
        if (!int.TryParse(cycle, out var n)) return cycle;
        int yr = n / 100, c = n % 100;
        return c > 1 ? $"{yr:D2}{c - 1:D2}" : $"{yr - 1:D2}13";
    }

    /// <summary>
    /// Returns a numeric sort key for chart-code categories so groups appear
    /// in a logical pilot-workflow order (diagram first, then approaches, departures, arrivals).
    /// </summary>
    private static int ChartOrder(string code) => code switch
    {
        "APD"  => 0,
        "IAP"  => 1,
        "DP"   => 2,
        "STAR" => 3,
        "MIN"  => 4,
        "HOT"  => 5,
        _      => 99
    };
}
