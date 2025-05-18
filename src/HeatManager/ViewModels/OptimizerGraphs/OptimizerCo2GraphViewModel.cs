using HeatManager.Core.Models.Schedules;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeatManager.ViewModels.OptimizerGraphs;

internal partial class OptimizerCo2GraphViewModel : BaseOptimizerGraphViewModel
{
    public OptimizerCo2GraphViewModel(List<HeatProductionUnitSchedule> schedules, List<DateTime> OrderedTimes, DateTimeOffset? minDate)
        : base(schedules, OrderedTimes, minDate) { }


    protected override void BuildChartSeries(List<HeatProductionUnitSchedule> schedules)
    {
        Series.Clear();
        int i = 0;
        List<double> totalEmissionsPerHour = new List<double>(new double[orderedTimes.Count]);

        foreach (var unitSchedule in schedules)
        {
            i++;
            for (int j = 0; j < unitSchedule.Emissions.Length; j++)
            {
                totalEmissionsPerHour[j] += unitSchedule.Emissions[j];
            }

            Series.Add(new LineSeries<double>
            {
                Values = unitSchedule.Emissions,
                Name = unitSchedule.Name,
                Stroke = new SolidColorPaint(Colors[i]) { StrokeThickness = 3 },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 1
            });
        }

        Series.Add(new LineSeries<double>
        {
            Values = totalEmissionsPerHour.ToArray(),
            Name = "Accumulative CO₂",
            Stroke = new SolidColorPaint(Colors[i + 1]) { StrokeThickness = 5 },
            Fill = null,
            GeometryFill = null,
            GeometryStroke = null,
            LineSmoothness = 1
        });
    }

    protected override string GetYAxisTitle() => "CO₂ Emissions [kg]";
}