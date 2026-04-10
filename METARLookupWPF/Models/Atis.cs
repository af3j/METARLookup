namespace METARLookupWPF.Models;

/// <summary>
/// Represents a D-ATIS (Digital Automatic Terminal Information Service) broadcast for an airport.
/// ATIS provides pilots with current non-control information such as weather, active runways,
/// and NOTAMs before departure or arrival. Data is sourced from the datis.clowd.io API.
/// </summary>
public class Atis
{
    /// <summary>ICAO identifier of the airport this ATIS belongs to.</summary>
    public string? Airport { get; set; }

    /// <summary>
    /// Broadcast type: "arr" (arrival), "dep" (departure), or "combined".
    /// Busier airports issue separate arrival and departure ATIS; smaller ones use a single combined broadcast.
    /// </summary>
    public string? Type { get; set; }   // arr, dep, combined

    /// <summary>Full text of the ATIS broadcast, including the information letter identifier (e.g. "INFORMATION KILO …").</summary>
    public string? Datis { get; set; }
}
