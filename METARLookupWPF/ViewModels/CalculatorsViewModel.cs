using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace METARLookupWPF.ViewModels;

public partial class CalculatorsViewModel : ObservableObject
{
    // --- Density Altitude ---
    [ObservableProperty][NotifyPropertyChangedFor(nameof(DensityAltResult))] private double _pressureAltFt;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(DensityAltResult))] private double _oatCelsius;

    public string DensityAltResult
    {
        get
        {
            // Standard formula: DA = PA + 118.8 * (OAT - ISA_temp)
            // ISA temp at altitude = 15 - (PA / 1000 * 2)
            double isaTemp = 15.0 - (PressureAltFt / 1000.0 * 2.0);
            double da = PressureAltFt + 118.8 * (OatCelsius - isaTemp);
            return $"{da:F0} ft";
        }
    }

    // --- Pressure Altitude ---
    [ObservableProperty][NotifyPropertyChangedFor(nameof(PressureAltResult))] private double _fieldElevationFt;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(PressureAltResult))] private double _altimeterInHg = 29.92;

    public string PressureAltResult
    {
        get
        {
            // PA = field elevation + (29.92 - altimeter) * 1000
            double pa = FieldElevationFt + (29.92 - AltimeterInHg) * 1000.0;
            return $"{pa:F0} ft";
        }
    }

    // --- Crosswind / Headwind ---
    [ObservableProperty][NotifyPropertyChangedFor(nameof(HeadwindResult))][NotifyPropertyChangedFor(nameof(CrosswindResult))]
    private double _windDirectionDeg;

    [ObservableProperty][NotifyPropertyChangedFor(nameof(HeadwindResult))][NotifyPropertyChangedFor(nameof(CrosswindResult))]
    private double _windSpeedKt;

    [ObservableProperty][NotifyPropertyChangedFor(nameof(HeadwindResult))][NotifyPropertyChangedFor(nameof(CrosswindResult))]
    private double _runwayHeadingDeg;

    public string HeadwindResult
    {
        get
        {
            double angle = (WindDirectionDeg - RunwayHeadingDeg) * Math.PI / 180.0;
            double hw = WindSpeedKt * Math.Cos(angle);
            return hw >= 0 ? $"{hw:F1} kt headwind" : $"{-hw:F1} kt tailwind";
        }
    }

    public string CrosswindResult
    {
        get
        {
            double angle = (WindDirectionDeg - RunwayHeadingDeg) * Math.PI / 180.0;
            double xw = Math.Abs(WindSpeedKt * Math.Sin(angle));
            return $"{xw:F1} kt crosswind";
        }
    }

    // Pre-fill from current METAR
    public void PreFillFromMetar(double? tempC, double? altInHg, double? elevFt, int? windDir, int? windSpeedKt)
    {
        if (tempC.HasValue) OatCelsius = tempC.Value;
        if (altInHg.HasValue)
        {
            AltimeterInHg = altInHg.Value;
            PressureAltFt = (elevFt ?? 0) + (29.92 - altInHg.Value) * 1000.0;
        }
        if (elevFt.HasValue) FieldElevationFt = elevFt.Value;
        if (windDir.HasValue) WindDirectionDeg = windDir.Value;
        if (windSpeedKt.HasValue) WindSpeedKt = windSpeedKt.Value;
    }
}
