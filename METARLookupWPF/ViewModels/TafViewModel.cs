using CommunityToolkit.Mvvm.ComponentModel;
using METARLookupWPF.Models;

namespace METARLookupWPF.ViewModels;

public partial class TafPeriodVm : ObservableObject
{
    public string TimeRange { get; init; } = string.Empty;
    public string ChangeIndicator { get; init; } = string.Empty;
    public string Wind { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
    public string Sky { get; init; } = string.Empty;
    public string Wx { get; init; } = string.Empty;
    public string FlightCategory { get; init; } = string.Empty;
}

public partial class TafViewModel : ObservableObject
{
    [ObservableProperty] private string _rawTaf = string.Empty;
    [ObservableProperty] private string _issueTime = string.Empty;
    [ObservableProperty] private string _validPeriod = string.Empty;
    [ObservableProperty] private List<TafPeriodVm> _periods = [];
    [ObservableProperty] private bool _hasTaf;

    public void Load(Taf? taf)
    {
        if (taf == null)
        {
            HasTaf = false;
            RawTaf = string.Empty;
            IssueTime = string.Empty;
            ValidPeriod = string.Empty;
            Periods = [];
            return;
        }

        HasTaf = true;
        RawTaf = taf.RawText ?? string.Empty;
        IssueTime = taf.IssueTime?.ToString("yyyy-MM-dd HH:mm") + " Z" ?? string.Empty;
        ValidPeriod = $"{taf.ValidFrom:HH:mm} - {taf.ValidTo:HH:mm} Z ({taf.ValidFrom:yyyy-MM-dd})";

        Periods = taf.Periods.Select(p =>
        {
            string wind = p.WindDir.HasValue
                ? $"{p.WindDir:D3}° @ {p.WindSpeedKt}kt" + (p.WindGustsKt > 0 ? $" G{p.WindGustsKt}kt" : "")
                : "—";

            string sky = string.Join(", ", p.SkyConditions.Select(s =>
                s.CloudBase.HasValue ? $"{s.SkyCover} {s.CloudBase:D3}00ft" : s.SkyCover ?? ""));

            return new TafPeriodVm
            {
                TimeRange = $"{p.From:HH:mm} – {p.To:HH:mm} Z",
                ChangeIndicator = p.ChangeIndicator ?? "BECMG",
                Wind = wind,
                Visibility = p.VisibilityStatuteMi.HasValue ? $"{p.VisibilityStatuteMi:F1} SM" : "—",
                Sky = string.IsNullOrEmpty(sky) ? "SKC" : sky,
                Wx = p.Wx ?? string.Empty,
                FlightCategory = p.FlightCategory,
            };
        }).ToList();
    }
}
