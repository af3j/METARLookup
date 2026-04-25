using System.IO;
using System.Reflection;
using System.Text;
using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

public interface IAirportSearchService
{
    /// <summary>
    /// Returns up to <paramref name="maxResults"/> airports matching <paramref name="query"/>.
    /// Returns an empty list if the query is fewer than 2 characters.
    /// </summary>
    IReadOnlyList<AirportSuggestion> Search(string query, int maxResults = 10);
}

/// <summary>
/// Loads the bundled OurAirports CSV once at construction time and exposes a fast
/// in-memory search over ~30 K airport records. Registered as a singleton so the
/// parse cost (≈ 50–100 ms) is paid only once on first resolve.
/// </summary>
public sealed class AirportSearchService : IAirportSearchService
{
    private readonly AirportSuggestion[] _airports;

    public AirportSearchService()
    {
        _airports = LoadFromEmbeddedCsv();
    }

    // ── Search ────────────────────────────────────────────────────────────────

    public IReadOnlyList<AirportSuggestion> Search(string query, int maxResults = 10)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return [];

        var q = query.Trim().ToUpperInvariant();

        var scored = new List<(int Score, AirportSuggestion Airport)>(_airports.Length / 10);

        foreach (var airport in _airports)
        {
            int score = ComputeScore(airport, q);
            if (score > 0)
                scored.Add((score, airport));
        }

        return scored
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Airport.Icao)
            .Take(maxResults)
            .Select(x => x.Airport)
            .ToArray();
    }

    private static int ComputeScore(AirportSuggestion a, string upperQuery)
    {
        // Score 4: exact ICAO match
        if (a.Icao == upperQuery) return 4;

        // Score 3: ICAO starts with the query
        if (a.Icao.StartsWith(upperQuery, StringComparison.Ordinal)) return 3;

        // Score 2: IATA exact or prefix match
        if (!string.IsNullOrEmpty(a.Iata))
        {
            var iata = a.Iata.ToUpperInvariant();
            if (iata == upperQuery || iata.StartsWith(upperQuery, StringComparison.Ordinal))
                return 2;
        }

        // Score 1: all query tokens found across city, state, country, and name.
        // Splitting on commas and spaces lets users type "Seattle, WA", "Seattle WA",
        // "Denver CO", "New York", etc. and still get matches.
        var tokens = upperQuery.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);
        if (AllTokensMatch(a, tokens)) return 1;

        return 0;
    }

    /// <summary>
    /// Returns true when every token in <paramref name="tokens"/> appears in at least
    /// one of the airport's searchable fields: city, state abbreviation, country, or name.
    /// This allows multi-word queries like "Seattle WA" or "Los Angeles, CA, US" to work
    /// because each token is checked independently across all fields.
    /// </summary>
    private static bool AllTokensMatch(AirportSuggestion a, string[] tokens)
    {
        // Extract the state/region abbreviation once (e.g. "US-WA" → "WA").
        var state = a.Region.Contains('-') ? a.Region.Split('-', 2)[1] : string.Empty;

        foreach (var token in tokens)
        {
            bool found =
                a.City.Contains(token, StringComparison.OrdinalIgnoreCase) ||
                state.Contains(token, StringComparison.OrdinalIgnoreCase) ||
                a.Country.Contains(token, StringComparison.OrdinalIgnoreCase) ||
                a.Name.Contains(token, StringComparison.OrdinalIgnoreCase);

            if (!found) return false;
        }

        return tokens.Length > 0;
    }

    // ── CSV loading ───────────────────────────────────────────────────────────

    private static AirportSuggestion[] LoadFromEmbeddedCsv()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(
            "METARLookupWPF.Data.airports_filtered.csv")
            ?? throw new InvalidOperationException(
                "Embedded resource 'METARLookupWPF.Data.airports_filtered.csv' not found. " +
                "Ensure the file is marked as EmbeddedResource in the .csproj.");

        using var reader = new StreamReader(stream, Encoding.UTF8);

        var results = new List<AirportSuggestion>(32_000);
        reader.ReadLine(); // skip header row

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            var suggestion = ParseLine(line);
            if (suggestion is not null)
                results.Add(suggestion);
        }

        return results.ToArray();
    }

    /// <summary>
    /// Parses one CSV line produced by Export-Csv (PowerShell).
    /// Column order: ident, iata_code, name, municipality, iso_country, iso_region
    /// PowerShell Export-Csv always quotes every field, so we just need to strip the
    /// surrounding quotes and handle escaped "" inside values.
    /// </summary>
    private static AirportSuggestion? ParseLine(string line)
    {
        var fields = SplitCsvLine(line);
        if (fields.Count < 6) return null;

        var icao = fields[0].Trim();
        if (icao.Length < 3 || icao.Length > 4) return null;

        var iata = fields[1].Trim();

        return new AirportSuggestion(
            Icao:    icao,
            Iata:    string.IsNullOrWhiteSpace(iata) ? null : iata,
            Name:    fields[2].Trim(),
            City:    fields[3].Trim(),
            Country: fields[4].Trim(),
            Region:  fields[5].Trim());
    }

    /// <summary>
    /// Minimal RFC-4180-compliant CSV field splitter.
    /// Handles quoted fields (which may contain commas and escaped double-quotes "").
    /// </summary>
    private static List<string> SplitCsvLine(string line)
    {
        var fields = new List<string>(6);
        var sb = new StringBuilder();
        bool inQuote = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuote && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');  // escaped double-quote inside quoted field
                    i++;
                }
                else
                {
                    inQuote = !inQuote;
                }
            }
            else if (c == ',' && !inQuote)
            {
                fields.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        fields.Add(sb.ToString()); // last field (no trailing comma)
        return fields;
    }
}
