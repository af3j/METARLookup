namespace METARLookupWPF.Models;

/// <summary>
/// A single airport entry shown in the AutoSuggestBox dropdown.
/// Populated from the bundled OurAirports CSV at startup.
/// </summary>
public sealed record AirportSuggestion(
    string Icao,        // 3–4 char ICAO identifier, e.g. "KSEA"
    string? Iata,       // 3-char IATA code, e.g. "SEA" — may be null/empty
    string Name,        // Official airport name
    string City,        // Municipality
    string Country,     // ISO 3166-1 alpha-2, e.g. "US"
    string Region)      // ISO region, e.g. "US-WA"
{
    /// <summary>
    /// Formatted location line for the dropdown subtitle, e.g.:
    ///   "Seattle, WA, US"  (when a state/region code is present)
    ///   "London, GB"       (when there is no sub-national region)
    /// Region codes follow ISO 3166-2 format ("US-WA") — we strip the country prefix.
    /// </summary>
    public string LocationDisplay
    {
        get
        {
            var state = Region.Contains('-')
                ? Region.Split('-', 2)[1]
                : string.Empty;

            return string.IsNullOrEmpty(state)
                ? $"{City}, {Country}"
                : $"{City}, {state}, {Country}";
        }
    }

    /// <summary>
    /// Returns the ICAO code so the AutoSuggestBox populates the text box
    /// with a clean identifier after the user selects a suggestion.
    /// </summary>
    public override string ToString() => Icao;
}
