using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

/// <summary>
/// Retrieves static airport metadata (name, location, coordinates, etc.)
/// from the airport-data.com public API by ICAO code.
/// </summary>
public interface IAirportService
{
    /// <summary>
    /// Fetches airport information for <paramref name="icao"/>.
    /// Returns null if the airport is not found or the request fails.
    /// </summary>
    Task<Airport?> GetAirportAsync(string icao, CancellationToken ct = default);
}

/// <summary>
/// Retrieves airport metadata by trying aviationweather.gov first, then falling back to
/// airport-data.com if the primary source returns nothing.
/// </summary>
public class AirportService(HttpClient http) : IAirportService
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    /// <inheritdoc/>
    public async Task<Airport?> GetAirportAsync(string icao, CancellationToken ct = default)
    {
        return await TryAirportDataComAsync(icao, ct)
        ?? await TryAvWeatherAsync(icao, ct);
    }

    // ── Fallback: aviationweather.gov ─────────────────────────────────────────

    private async Task<Airport?> TryAvWeatherAsync(string icao, CancellationToken ct)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            using var response = await http.GetAsync(
                $"https://aviationweather.gov/api/data/airport?ids={icao}&format=json", cts.Token);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(cts.Token);
            var list = JsonSerializer.Deserialize<List<AvwRaw>>(json, JsonOpts);
            var raw = list?.FirstOrDefault();
            if (raw == null) return null;

            return new Airport
            {
                Icao = raw.Id,
                Iata = raw.Iata,
                Name = raw.Name,
                Location = FormatLocation(raw.City, raw.State, raw.Country),
                City = raw.City,
                Country = raw.Country,
                Latitude = raw.Lat,
                Longitude = raw.Lon,
            };
        }
        catch { return null; }
    }

    // ── Primary: airport-data.com ───────────────────────────────────────────

    private async Task<Airport?> TryAirportDataComAsync(string icao, CancellationToken ct)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            using var response = await http.GetAsync(
                $"https://airport-data.com/api/ap_info.json?icao={icao}", cts.Token);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(cts.Token);
            var raw = JsonSerializer.Deserialize<AdcRaw>(json, JsonOpts);
            if (raw == null) return null;

            return new Airport
            {
                Icao = raw.Icao,
                Iata = raw.Iata,
                Name = raw.Name,
                Location = raw.Location,
                City = raw.City,
                Country = raw.Country,
                CountryIso = raw.Country_iso,
                Phone = raw.Phone,
                Website = raw.Website,
                Latitude = TryParseDouble(raw.Latitude),
                Longitude = TryParseDouble(raw.Longitude),
                Uct = TryParseInt(raw.Uct),
            };
        }
        catch { return null; }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string? FormatLocation(string? city, string? state, string? country)
    {
        var parts = new[] { city, state }.Where(s => !string.IsNullOrWhiteSpace(s));
        var result = string.Join(", ", parts);
        return string.IsNullOrEmpty(result) ? country : result;
    }

    private static double? TryParseDouble(string? s) =>
        double.TryParse(s, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;

    private static int? TryParseInt(string? s) =>
        int.TryParse(s, out var v) ? v : null;

    // ── DTOs ─────────────────────────────────────────────────────────────────

    /// <summary>aviationweather.gov airport JSON fields.</summary>
    private class AvwRaw
    {
        public string? Id { get; set; }
        public string? Iata { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
    }

    /// <summary>airport-data.com JSON fields (lat/lon returned as strings).</summary>
    private class AdcRaw
    {
        public string? Icao { get; set; }
        public string? Iata { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Country_iso { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Uct { get; set; }
    }
}
