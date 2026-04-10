using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

/// <summary>
/// Provides access to aviation weather data from the FAA/NOAA Aviation Weather Center
/// at aviationweather.gov. All methods are asynchronous and support cancellation.
/// </summary>
public interface IAvWeatherService
{
    /// <summary>
    /// Retrieves the most recent METAR observation for the given ICAO station,
    /// looking back up to 3 hours. Returns null if no observation is available.
    /// </summary>
    Task<Metar?> GetMetarAsync(string icao, CancellationToken ct = default);

    /// <summary>
    /// Retrieves the current TAF (Terminal Aerodrome Forecast) for the given ICAO station.
    /// Returns null if no TAF is available (e.g. the airport does not have TAF service).
    /// </summary>
    Task<Taf?> GetTafAsync(string icao, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all currently active AIRMET and SIGMET advisories worldwide.
    /// Returns an empty list on failure.
    /// </summary>
    Task<List<Sigmet>> GetSigmetsAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves METARs for all stations within a bounding box of ±<paramref name="radiusDeg"/>
    /// degrees around the given coordinates. Used to populate the map with nearby flight categories.
    /// </summary>
    Task<List<Metar>> GetNearbyMetarsAsync(double lat, double lon, double radiusDeg = 1.0, CancellationToken ct = default);
}
