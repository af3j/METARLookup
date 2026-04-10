using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using METARLookupWPF.Models;
using SkiaSharp;

namespace METARLookupWPF.ViewModels;

public partial class ChartsViewModel : ObservableObject
{
    [ObservableProperty] private ISeries[] _windSeries = [];
    [ObservableProperty] private ISeries[] _visSeries = [];
    [ObservableProperty] private ISeries[] _tempSeries = [];
    [ObservableProperty] private Axis[] _windXAxes = [new Axis { Labels = [] }];
    [ObservableProperty] private Axis[] _visXAxes = [new Axis { Labels = [] }];
    [ObservableProperty] private Axis[] _tempXAxes = [new Axis { Labels = [] }];
    [ObservableProperty] private bool _hasData;

    public void Load(List<Metar> history)
    {
        if (history.Count == 0)
        {
            HasData = false;
            WindSeries = [];
            VisSeries = [];
            TempSeries = [];
            return;
        }

        var ordered = history.OrderBy(m => m.ObservationTime).ToList();
        var labels = ordered.Select(m => m.ObservationTime?.ToString("HH:mm") ?? "").ToArray();

        var windValues = ordered.Select(m => m.WindSpeedKt.HasValue ? new ObservableValue(m.WindSpeedKt.Value) : new ObservableValue(null)).ToList();
        var gustValues = ordered.Select(m => m.WindGustsKt is > 0 ? new ObservableValue(m.WindGustsKt.Value) : new ObservableValue(null)).ToList();
        var visValues = ordered.Select(m => m.VisibilityStatuteMi.HasValue ? new ObservableValue(m.VisibilityStatuteMi.Value) : new ObservableValue(null)).ToList();
        var tempValues = ordered.Select(m => m.TempC.HasValue ? new ObservableValue(m.TempC.Value) : new ObservableValue(null)).ToList();
        var dewValues = ordered.Select(m => m.DewpointC.HasValue ? new ObservableValue(m.DewpointC.Value) : new ObservableValue(null)).ToList();

        WindSeries =
        [
            new LineSeries<ObservableValue>
            {
                Name = "Wind (kt)",
                Values = windValues,
                Stroke = new SolidColorPaint(SKColors.DeepSkyBlue, 2),
                Fill = null,
                GeometrySize = 4,
                GeometryStroke = new SolidColorPaint(SKColors.DeepSkyBlue, 2),
            },
            new LineSeries<ObservableValue>
            {
                Name = "Gusts (kt)",
                Values = gustValues,
                Stroke = new SolidColorPaint(SKColors.OrangeRed, 2),
                Fill = null,
                GeometrySize = 4,
                GeometryStroke = new SolidColorPaint(SKColors.OrangeRed, 2),
            }
        ];

        VisSeries =
        [
            new LineSeries<ObservableValue>
            {
                Name = "Visibility (SM)",
                Values = visValues,
                Stroke = new SolidColorPaint(SKColors.LimeGreen, 2),
                Fill = new SolidColorPaint(SKColors.LimeGreen.WithAlpha(40)),
                GeometrySize = 4,
                GeometryStroke = new SolidColorPaint(SKColors.LimeGreen, 2),
            }
        ];

        TempSeries =
        [
            new LineSeries<ObservableValue>
            {
                Name = "Temp (°C)",
                Values = tempValues,
                Stroke = new SolidColorPaint(SKColors.Tomato, 2),
                Fill = null,
                GeometrySize = 4,
                GeometryStroke = new SolidColorPaint(SKColors.Tomato, 2),
            },
            new LineSeries<ObservableValue>
            {
                Name = "Dewpoint (°C)",
                Values = dewValues,
                Stroke = new SolidColorPaint(SKColors.CornflowerBlue, 2),
                Fill = null,
                GeometrySize = 4,
                GeometryStroke = new SolidColorPaint(SKColors.CornflowerBlue, 2),
            }
        ];

        var labelAxis = new Axis { Labels = labels, LabelsRotation = -30 };
        WindXAxes = [labelAxis];
        VisXAxes = [new Axis { Labels = labels, LabelsRotation = -30 }];
        TempXAxes = [new Axis { Labels = labels, LabelsRotation = -30 }];

        HasData = true;
    }
}
