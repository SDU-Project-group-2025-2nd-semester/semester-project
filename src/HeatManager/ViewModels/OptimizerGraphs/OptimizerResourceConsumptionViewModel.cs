using HeatManager.Core.Models.Schedules;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System;
using System.Collections.Generic;


namespace HeatManager.ViewModels.OptimizerGraphs;

internal class OptimizerResourceConsumptionViewModel : BaseOptimizerGraphViewModel
{
    public OptimizerResourceConsumptionViewModel(Schedule schedule, List<DateTime> OrderedTimes, DateTimeOffset? minDate)
        : base(schedule, OrderedTimes, minDate) { }

    protected override void BuildChartSeries(List<HeatProductionUnitSchedule> schedules)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the filename prefix used when exporting the chart to an image file.
    /// </summary>
    protected override string FilenamePrefixOnExport => "ResourceConsumptionChart";

    protected override void BuildChartSeries(Schedule schedule)
    {
        Series.Clear();

        var resourceConsumption = schedule.ResourceConsumption;

        int i = 0;
        foreach (var resource in resourceConsumption)
        {
            i++;
            var resourceType = resource.Key;
            var consumptionData = resource.Value;

            // Create a series for the line chart
            var series = new LineSeries<double>()
            {
                Values = consumptionData,
                Name = resourceType.ToString(),
                Stroke = new SolidColorPaint(ColorGenerator.SetColor(resourceType.ToString())) { StrokeThickness = 3 },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 1
            };

            // Add the series to the chart
            Series.Add(series);
        }
    }

    protected override string GetYAxisTitle() => "Resource Consumption";
}