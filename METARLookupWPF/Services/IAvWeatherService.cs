using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

public interface IAvWeatherService
{
    Task<Metar?> GetMetarAsync(string icao, CancellationToken ct = default);
    Task<List<Metar>> GetMetarHistoryAsync(string icao, int hoursBeforeNow = 24, CancellationToken ct = default);
    Task<Taf?> GetTafAsync(string icao, CancellationToken ct = default);
    Task<List<Sigmet>> GetSigmetsAsync(CancellationToken ct = default);
    Task<List<Metar>> GetNearbyMetarsAsync(double lat, double lon, double radiusDeg = 1.0, CancellationToken ct = default);
}
