namespace METARLookupWPF.Models;

/// <summary>
/// Represents a single FAA instrument/approach/departure chart for an airport,
/// as listed in the FAA d-TPP (digital Terminal Procedures Publication) metafile.
/// </summary>
public class AirportChart
{
    /// <summary>
    /// FAA chart-code category: "APD" (Airport Diagram), "IAP" (Instrument Approach Procedure),
    /// "DP" (Departure Procedure), "STAR" (Standard Terminal Arrival Route), "MIN" (Minimums),
    /// "HOT" (Hot Spots), "LAH" (Land and Hold Short Operations).
    /// </summary>
    public string ChartCode { get; set; } = "";   // APD, IAP, DP, STAR, etc.

    /// <summary>Human-readable chart name as listed in the FAA d-TPP (e.g. "ILS OR LOC RWY 16L").</summary>
    public string Name      { get; set; } = "";

    /// <summary>
    /// Full URL to the chart PDF on aeronav.faa.gov. Constructed by combining the
    /// current AIRAC cycle base URL with the PDF filename from the metafile.
    /// </summary>
    public string PdfUrl    { get; set; } = "";
}

/// <summary>
/// A named collection of <see cref="AirportChart"/> objects sharing the same chart-code category.
/// Used to populate the expandable groups in the Charts tab.
/// </summary>
public class ChartGroup
{
    /// <summary>Friendly display name for the category (e.g. "Instrument Approaches").</summary>
    public string           Category { get; set; } = "";

    /// <summary>Charts belonging to this category, in the order returned by the FAA metafile.</summary>
    public List<AirportChart> Charts { get; set; } = [];
}
