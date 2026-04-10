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
/// Implements <see cref="IAirportService"/> using the airport-data.com JSON REST API.
/// Coordinates returned by this API are strings that must be parsed to doubles.
/// </summary>
public class AirportService(HttpClient http) : IAirportService
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    /// <inheritdoc/>
    public async Task<Airport?> GetAirportAsync(string icao, CancellationToken ct = default)
    {
        try
        {
            // Add a tighter 5-second timeout so a slow airport lookup doesn't block
            // the UI while the METAR (more important) is waiting.
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            using var response = await http.GetAsync($"https://airport-data.com/api/ap_info.json?icao={icao}", cts.Token);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(cts.Token);
            // Deserialize into the private DTO whose field names match the JSON keys.
            var raw = JsonSerializer.Deserialize<AirportRaw>(json, JsonOpts);
            if (raw == null) return null;

            // Map the raw DTO to the public Airport model, parsing string coordinates to doubles.
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
        catch
        {
            // Swallow all exceptions (network errors, parse errors, cancellation) and
            // return null so the app degrades gracefully when airport data is unavailable.
            return null;
        }
    }

    /// <summary>Parses a coordinate string with invariant culture; returns null if invalid.</summary>
    private static double? TryParseDouble(string? s) =>
        double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;

    private static int? TryParseInt(string? s) =>
        int.TryParse(s, out var v) ? v : null;

    /// <summary>
    /// Private DTO whose property names match the airport-data.com JSON response exactly.
    /// Lat/lon are returned as strings (may include trailing spaces or be empty), so they
    /// are kept as strings here and converted in the mapping step above.
    /// </summary>
    private class AirportRaw
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
