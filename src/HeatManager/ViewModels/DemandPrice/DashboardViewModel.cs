using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes;

using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.Services.FileServices;
using System.Threading.Tasks;

namespace HeatManager.ViewModels.DemandPrice;

public partial class GridProductionViewModel : ViewModelBase
{
    [ObservableProperty]
    private string firstViewText = "Source: https://livecharts.dev/docs/Avalonia/2.0.0-rc5.4/samples.general.scrollable";

    [ObservableProperty]
    private string pageTitle = "Summer Data";

    private bool _isDown = false;
    private readonly ObservableCollection<DateTimePoint> _heatValues = [];
    private readonly ObservableCollection<DateTimePoint> _priceValues = [];

    public ISeries[] ChartSeries { get; set; }
    public ICartesianAxis[] ScrollableAxes { get; set; }
    public ICartesianAxis[] YAxes { get; set; }
    public ISeries[] ScrollbarSeries { get; set; }
    public Axis[] InvisibleX { get; set; }
    public Axis[] InvisibleY { get; set; }
    public LiveChartsCore.Measure.Margin Margin { get; set; }
    public RectangularSection[] Thumbs { get; set; }

    public ChartExporter chartExporter = new ChartExporter();

    public GridProductionViewModel(ISourceDataProvider provider)
    {
        var dataPoints = provider.SourceDataCollection?.DataPoints?? throw new InvalidOperationException("Source data need to be imported before their visualization.");



        foreach (SourceDataPoint dataPoint in dataPoints)
        {
            _priceValues.Add(new DateTimePoint(dataPoint.TimeFrom, (double)dataPoint.ElectricityPrice));
            _heatValues.Add(new DateTimePoint(dataPoint.TimeFrom, dataPoint.HeatDemand));
        }

        ChartSeries = [
            new ColumnSeries<DateTimePoint>
            {
                Values = _priceValues,
                Name = "Electricity Price",
                DataPadding = new(0,1),
                ScalesYAt = 1
            },
            new LineSeries<DateTimePoint>
            {
                Values = _heatValues,
                Name = "Heat Demand",
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0,1),
                ScalesYAt = 0
            },
        ];

        ScrollbarSeries = [
            new LineSeries<DateTimePoint>
            {
                Values = _heatValues,
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0, 1)
            }
        ];

        int initialViewSize = Math.Min(50, _heatValues.Count);

        DateTime startDate = _heatValues.First().DateTime;
        DateTime endDate = initialViewSize < _heatValues.Count ? _heatValues[initialViewSize - 1].DateTime : _heatValues.Last().DateTime;

        TimeSpan timeSpan = TimeSpan.FromHours(1);

        Thumbs = [
            new RectangularSection
            {
                Fill = new SolidColorPaint(new SKColor(220, 220, 220, 120)),
                // Stroke = new SolidColorPaint(SKColors.Gray) {StrokeThickness = 1},
                Xi = startDate.Ticks,
                Xj = endDate.Ticks
            }
        ];

        YAxes = [
            new Axis
            {
            Name = "Heat Demand",
            NameTextSize = 14,
            // NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
            // Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 10, 0),
            TextSize = 12,
            },
            new Axis
            {
            Name = "Electricity Price",
            NameTextSize = 14,
            // NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
            // Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 12,
            ShowSeparatorLines = false,
            }
        ];

        ScrollableAxes = [
            new DateTimeAxis(timeSpan, date => date.ToString("MM/dd/yy"))
            {
                MinLimit = startDate.Ticks,
                MaxLimit = endDate.Ticks,
                // Name = "Day",
                Labeler = value =>
                {
                    var date = new DateTime((long)value);

                    if (ScrollableAxes is not { Length: > 0 } ||
                        ScrollableAxes[0] is not DateTimeAxis dateAxis)
                    {
                        return date.ToString("MMM/dd/yy");
                    }

                    var visibleRange = dateAxis.MaxLimit - dateAxis ?.MinLimit ?? throw new InvalidOperationException("Max and/or min limit not set.");
                    var rangeDays = TimeSpan.FromTicks((long)visibleRange).TotalDays;

                    return rangeDays switch
                    {
                        < 1 => date.ToString("HH:mm"),
                        < 7 => date.ToString("MMM/dd HH:mm"),
                        _ => date.ToString("MMM/dd/yy")
                    };
                }
            }
        ];


        InvisibleX = [new Axis { IsVisible = false }];
        InvisibleY = [new Axis { IsVisible = false }];

        // force the left margin to be 100 and the right margin 50 in both charts, this will
        // align the start and end point of the "draw margin",
        // no matter the size of the labels in the Y axis of both chart.
        var auto = LiveChartsCore.Measure.Margin.Auto;
        Margin = new(100, auto, 50, auto);
    }

    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (ICartesianChartView)args.Chart;

        var x = cartesianChart.XAxes.First();

        // update the scroll bar thumb when the chart is updated (zoom/pan)
        // this will let the user know the current visible range
        var thumb = Thumbs[0];

        thumb.Xi = x.MinLimit;
        thumb.Xj = x.MaxLimit;

        OnPropertyChanged(nameof(Thumbs));
    }

    [RelayCommand]
    public void PointerDown(PointerCommandArgs args) =>
        _isDown = true;

    [RelayCommand]
    public void PointerMove(PointerCommandArgs args)
    {
        if (!_isDown) return;

        var chart = (ICartesianChartView)args.Chart;
        var positionInData = chart.ScalePixelsToData(args.PointerPosition);

        var thumb = Thumbs[0];
        var currentRange = thumb.Xj - thumb.Xi;

        // update the scroll bar thumb when the user is dragging the chart
        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;

        // update the chart visible range
        ScrollableAxes[0].MinLimit = thumb.Xi;
        ScrollableAxes[0].MaxLimit = thumb.Xj;
    }

    [RelayCommand]
    public void PointerUp(PointerCommandArgs args) =>
        _isDown = false;

    [RelayCommand]
    public async Task ExportButton(object chartObject)
    {
        var mainChart = chartObject as LiveChartsCore.SkiaSharpView.Avalonia.CartesianChart;

        // Execute Export logic
        if (mainChart == null)
        {
            Console.WriteLine("ChartControl not found");
            return;
        }
        await chartExporter.ExportControl(mainChart, ChartSeries, ScrollableAxes, YAxes, "SummerDataChart");
    }

}
