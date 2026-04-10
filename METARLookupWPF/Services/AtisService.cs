using System.Net.Http;
using System.Text.Json;
using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

/// <summary>
/// Retrieves D-ATIS (Digital Automatic Terminal Information Service) broadcasts
/// for a given airport from the datis.clowd.io public API.
/// Not all airports have D-ATIS; smaller or international airports may return an empty list.
/// </summary>
public interface IAtisService
{
    /// <summary>
    /// Returns all available ATIS broadcasts for <paramref name="icao"/> (arrival, departure, or combined).
    /// Returns an empty list if no ATIS is available or the request fails.
    /// </summary>
    Task<List<Atis>> GetAtisAsync(string icao, CancellationToken ct = default);
}

/// <summary>
/// Implements <see cref="IAtisService"/> using the datis.clowd.io REST API,
/// which aggregates D-ATIS data from VATSIM and other sources.
/// </summary>
public class AtisService(HttpClient http) : IAtisService
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    /// <inheritdoc/>
    public async Task<List<Atis>> GetAtisAsync(string icao, CancellationToken ct = default)
    {
        try
        {
            using var response = await http.GetAsync($"https://datis.clowd.io/api/{icao}", ct);
            if (!response.IsSuccessStatusCode) return [];

            var json = await response.Content.ReadAsStringAsync(ct);
            // The API returns a JSON error object (containing "error" key) for unknown airports
            // rather than a 4xx status code, so we check the body content as well.
            if (string.IsNullOrWhiteSpace(json) || json.Contains("\"error\"")) return [];

            var list = JsonSerializer.Deserialize<List<Atis>>(json, JsonOpts);
            return list ?? [];
        }
        catch
        {
            // ATIS is supplementary information; fail silently so the METAR tab still loads.
            return [];
        }
    }
}
