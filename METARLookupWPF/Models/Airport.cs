namespace METARLookupWPF.Models;

/// <summary>
/// Represents metadata about a single airport, populated from the airport-data.com API.
/// </summary>
public class Airport
{
    /// <summary>Internal database identifier returned by airport-data.com.</summary>
    public string? Id { get; set; }

    /// <summary>IATA three-letter code (e.g. "SEA"). May be null for smaller airports.</summary>
    public string? Iata { get; set; }

    /// <summary>ICAO four-letter identifier (e.g. "KSEA") used for weather lookups.</summary>
    public string? Icao { get; set; }

    /// <summary>Official airport name (e.g. "Seattle-Tacoma International Airport").</summary>
    public string? Name { get; set; }

    /// <summary>Human-readable location string, typically city and country.</summary>
    public string? Location { get; set; }

    public string? StreetNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? County { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }

    /// <summary>ISO 3166-1 alpha-2 country code (e.g. "US").</summary>
    public string? CountryIso { get; set; }

    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }

    /// <summary>WGS-84 latitude in decimal degrees. Used to centre the map and fetch nearby METARs.</summary>
    public double? Latitude { get; set; }

    /// <summary>WGS-84 longitude in decimal degrees.</summary>
    public double? Longitude { get; set; }

    /// <summary>UTC offset in whole hours (not accounting for DST).</summary>
    public int? Uct { get; set; }
}
