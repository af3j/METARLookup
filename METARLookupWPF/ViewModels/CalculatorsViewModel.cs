using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace METARLookupWPF.ViewModels;

/// <summary>
/// View-model for the Calculators tab. Provides three standard pilot calculations:
/// Density Altitude, Pressure Altitude, and Headwind/Crosswind components.
/// All result properties are automatically recomputed whenever their inputs change
/// via the [NotifyPropertyChangedFor] attribute on each input field.
/// </summary>
public partial class CalculatorsViewModel : ObservableObject
{
    // ── Density Altitude ─────────────────────────────────────────────────────

    /// <summary>Pressure altitude in feet, used as the base for density altitude calculation.</summary>
    [ObservableProperty][NotifyPropertyChangedFor(nameof(DensityAltResult))] private double _pressureAltFt;

    /// <summary>Outside air temperature in degrees Celsius at the field elevation.</summary>
    [ObservableProperty][NotifyPropertyChangedFor(nameof(DensityAltResult))] private double _oatCelsius;

    /// <summary>
    /// Computed density altitude in feet.
    /// Density altitude represents the pressure altitude corrected for non-standard temperature
    /// and directly affects aircraft performance (lift, engine power, propeller efficiency).
    /// Formula: DA = PA + 118.8 × (OAT − ISA_temp), where ISA_temp = 15°C − (PA/1000 × 2°C/1000ft).
    /// </summary>
    public string DensityAltResult
    {
        get
        {
            // ISA (International Standard Atmosphere) temperature decreases by ~2°C per 1000 ft.
            // The 118.8 factor converts the temperature deviation from ISA into a feet-of-altitude offset.
            double isaTemp = 15.0 - (PressureAltFt / 1000.0 * 2.0);
            double da = PressureAltFt + 118.8 * (OatCelsius - isaTemp);
            return $"{da:F0} ft";
        }
    }

    // ── Pressure Altitude ────────────────────────────────────────────────────

    /// <summary>Airport field elevation above mean sea level in feet.</summary>
    [ObservableProperty][NotifyPropertyChangedFor(nameof(PressureAltResult))] private double _fieldElevationFt;

    /// <summary>Current altimeter setting in inches of mercury. Defaults to ISA standard (29.92 in Hg).</summary>
    [ObservableProperty][NotifyPropertyChangedFor(nameof(PressureAltResult))] private double _altimeterInHg = 29.92;

    /// <summary>
    /// Computed pressure altitude in feet.
    /// Pressure altitude is what an altimeter reads when set to the standard datum of 29.92 in Hg.
    /// It is used for density altitude, engine performance charts, and transponder altitude.
    /// Formula: PA = field elevation + (29.92 − altimeter) × 1000.
    /// </summary>
    public string PressureAltResult
    {
        get
        {
            // Each 0.01 in Hg deviation from standard equals approximately 10 ft of pressure altitude.
            double pa = FieldElevationFt + (29.92 - AltimeterInHg) * 1000.0;
            return $"{pa:F0} ft";
        }
    }

    // ── Crosswind / Headwind ─────────────────────────────────────────────────

    /// <summary>Reported wind direction in degrees magnetic.</summary>
    [ObservableProperty][NotifyPropertyChangedFor(nameof(HeadwindResult))][NotifyPropertyChangedFor(nameof(CrosswindResult))]
    private double _windDirectionDeg;

    /// <summary>Reported wind speed in knots.</summary>
    [ObservableProperty][NotifyPropertyChangedFor(nameof(HeadwindResult))][NotifyPropertyChangedFor(nameof(CrosswindResult))]
    private double _windSpeedKt;

    /// <summary>Runway heading in degrees magnetic (e.g. 270 for Runway 27).</summary>
    [ObservableProperty][NotifyPropertyChangedFor(nameof(HeadwindResult))][NotifyPropertyChangedFor(nameof(CrosswindResult))]
    private double _runwayHeadingDeg;

    /// <summary>
    /// Component of wind acting directly along the runway (positive = headwind, negative = tailwind).
    /// Uses the cosine of the angle between wind direction and runway heading.
    /// A negative result indicates a tailwind, which increases ground roll distances.
    /// </summary>
    public string HeadwindResult
    {
        get
        {
            double angle = (WindDirectionDeg - RunwayHeadingDeg) * Math.PI / 180.0;
            double hw = WindSpeedKt * Math.Cos(angle);
            return hw >= 0 ? $"{hw:F1} kt headwind" : $"{-hw:F1} kt tailwind";
        }
    }

    /// <summary>
    /// Component of wind acting perpendicular to the runway (always positive).
    /// Uses the sine of the angle between wind direction and runway heading.
    /// Exceeding an aircraft's demonstrated crosswind limit requires careful technique or a different runway.
    /// </summary>
    public string CrosswindResult
    {
        get
        {
            double angle = (WindDirectionDeg - RunwayHeadingDeg) * Math.PI / 180.0;
            double xw = Math.Abs(WindSpeedKt * Math.Sin(angle));
            return $"{xw:F1} kt crosswind";
        }
    }

    // ── Auto-population from METAR ────────────────────────────────────────────

    /// <summary>
    /// Populates calculator inputs from a live METAR so the pilot doesn't need to
    /// re-enter the current conditions manually. Only fields with non-null METAR values
    /// are updated, preserving any manual entries the user may have made.
    /// Note: pressure altitude is pre-computed here because it depends on both
    /// altimeter and elevation — both of which come from the same METAR.
    /// </summary>
    public void PreFillFromMetar(double? tempC, double? altInHg, double? elevFt, int? windDir, int? windSpeedKt)
    {
        if (tempC.HasValue) OatCelsius = tempC.Value;
        if (altInHg.HasValue)
        {
            AltimeterInHg = altInHg.Value;
            // Pre-calculate pressure altitude so the density altitude calculator is ready immediately.
            PressureAltFt = (elevFt ?? 0) + (29.92 - altInHg.Value) * 1000.0;
        }
        if (elevFt.HasValue) FieldElevationFt = elevFt.Value;
        if (windDir.HasValue) WindDirectionDeg = windDir.Value;
        if (windSpeedKt.HasValue) WindSpeedKt = windSpeedKt.Value;
    }
}
