using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using METARLookupWPF.Models;

namespace METARLookupWPF.ViewModels;

public partial class MetarViewModel : ObservableObject
{
    [ObservableProperty] private Metar? _metar;
    [ObservableProperty] private Airport? _airport;
    [ObservableProperty] private string _atisText = string.Empty;
    [ObservableProperty] private string _selectedAtisType = "arr";

    private List<Atis> _atisList = [];

    public string FlightCategoryText => Metar?.FlightCategory ?? "—";
    public string StationId => Metar?.StationId ?? "—";
    public string AirportName => Airport?.Name ?? "—";
    public string AirportLocation => Airport?.Location ?? "—";
    public string RawMetar => Metar?.RawText ?? string.Empty;
    public string ObsDate => Metar?.ObservationTime?.ToString("yyyy-MM-dd") ?? "—";
    public string ObsTime => Metar?.ObservationTime?.ToString("HH:mm:ss") + " Z" ?? "—";
    public string TempC => Metar?.TempC?.ToString("F1") ?? "—";
    public string DewC => Metar?.DewpointC?.ToString("F1") ?? "—";
    public string WindDir => Metar?.WindDir.HasValue == true ? $"{Metar.WindDir:D3}°" : "VRB";
    public string WindSpeed => Metar?.WindSpeedKt?.ToString() ?? "—";
    public string WindGusts => (Metar?.WindGustsKt ?? 0) > 0 ? $"G{Metar!.WindGustsKt}kt" : string.Empty;
    public string Visibility => Metar?.VisibilityStatuteMi?.ToString("F1") ?? "—";
    public string AltInHg => Metar?.AltimeterInHg?.ToString("F2") ?? "—";
    public string AltQnh => Metar?.AltimeterQnh?.ToString("F1") ?? "—";
    public string ElevMeters => Metar?.ElevationMeter?.ToString("F0") ?? "—";
    public string ElevFeet => Metar?.ElevationFeet?.ToString("F0") ?? "—";
    public string SkyConditions => Metar == null ? string.Empty :
        string.Join("\n", Metar.SkyConditions.Select(s =>
            s.CloudBase.HasValue ? $"{s.SkyCover} {s.CloudBase:D3}00 ft AGL" : s.SkyCover ?? string.Empty));

    public void Load(Metar? metar, Airport? airport, List<Atis> atis)
    {
        Metar = metar;
        Airport = airport;
        _atisList = atis;
        ShowAtis(SelectedAtisType);
        NotifyAll();
    }

    [RelayCommand]
    private void ShowAtis(string type)
    {
        SelectedAtisType = type;
        var match = _atisList.FirstOrDefault(a => a.Type == type)
                    ?? _atisList.FirstOrDefault(a => a.Type == "combined")
                    ?? _atisList.FirstOrDefault();

        AtisText = match?.Datis ?? (type == "arr" ? "Arrival ATIS not available." : "Departure ATIS not available.");
    }

    private void NotifyAll()
    {
        OnPropertyChanged(nameof(FlightCategoryText));
        OnPropertyChanged(nameof(StationId));
        OnPropertyChanged(nameof(AirportName));
        OnPropertyChanged(nameof(AirportLocation));
        OnPropertyChanged(nameof(RawMetar));
        OnPropertyChanged(nameof(ObsDate));
        OnPropertyChanged(nameof(ObsTime));
        OnPropertyChanged(nameof(TempC));
        OnPropertyChanged(nameof(DewC));
        OnPropertyChanged(nameof(WindDir));
        OnPropertyChanged(nameof(WindSpeed));
        OnPropertyChanged(nameof(WindGusts));
        OnPropertyChanged(nameof(Visibility));
        OnPropertyChanged(nameof(AltInHg));
        OnPropertyChanged(nameof(AltQnh));
        OnPropertyChanged(nameof(ElevMeters));
        OnPropertyChanged(nameof(ElevFeet));
        OnPropertyChanged(nameof(SkyConditions));
    }
}
