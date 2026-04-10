using System.Net.Http;
using System.Text.Json;
using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

public interface IAtisService
{
    Task<List<Atis>> GetAtisAsync(string icao, CancellationToken ct = default);
}

public class AtisService(HttpClient http) : IAtisService
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<List<Atis>> GetAtisAsync(string icao, CancellationToken ct = default)
    {
        try
        {
            using var response = await http.GetAsync($"https://datis.clowd.io/api/{icao}", ct);
            if (!response.IsSuccessStatusCode) return [];

            var json = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(json) || json.Contains("\"error\"")) return [];

            var list = JsonSerializer.Deserialize<List<Atis>>(json, JsonOpts);
            return list ?? [];
        }
        catch
        {
            return [];
        }
    }
}
