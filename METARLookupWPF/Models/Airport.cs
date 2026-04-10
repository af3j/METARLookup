namespace METARLookupWPF.Models;

public class Airport
{
    public string? Id { get; set; }
    public string? Iata { get; set; }
    public string? Icao { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? StreetNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? County { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryIso { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? Uct { get; set; }
}
