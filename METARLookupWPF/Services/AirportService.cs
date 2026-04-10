using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

public interface IAirportService
{
    Task<Airport?> GetAirportAsync(string icao, CancellationToken ct = default);
}

public class AirportService(HttpClient http) : IAirportService
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<Airport?> GetAirportAsync(string icao, CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            using var response = await http.GetAsync($"https://airport-data.com/api/ap_info.json?icao={icao}", cts.Token);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(cts.Token);
            var raw = JsonSerializer.Deserialize<AirportRaw>(json, JsonOpts);
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
        catch
        {
            return null;
        }
    }

    private static double? TryParseDouble(string? s) =>
        double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;

    private static int? TryParseInt(string? s) =>
        int.TryParse(s, out var v) ? v : null;

    // Raw DTO that matches the airport-data.com JSON field names
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
