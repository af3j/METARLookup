using METARLookupWPF.Models;

namespace METARLookupWPF.Services;

/// <summary>
/// Retrieves FAA Terminal Procedures Publication (d-TPP) charts for a given airport.
/// Charts are grouped by type (approach, departure, arrival, etc.) and linked to PDFs
/// hosted on aeronav.faa.gov under the current AIRAC cycle directory.
/// </summary>
public interface IFaaChartsService
{
    /// <summary>
    /// Returns all available FAA d-TPP chart groups for <paramref name="icao"/>,
    /// ordered by chart type (Airport Diagram first, then approaches, departures, arrivals, etc.).
    /// Returns an empty list if the airport is not found in the current AIRAC metafile.
    /// </summary>
    Task<List<ChartGroup>> GetAirportChartsAsync(string icao, CancellationToken ct = default);
}
