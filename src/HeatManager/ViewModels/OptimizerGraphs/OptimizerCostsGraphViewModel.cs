using System;
using System.ComponentModel;
using System.Collections.Generic;

using HeatManager.Core.Models.Schedules;
using HeatManager.ViewModels.Optimizer;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Measure;
using LiveChartsCore.Kernel.Events;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Kernel.Sketches;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Linq;
using LiveChartsCore.SkiaSharpView.Painting.Effects;

namespace HeatManager.ViewModels.OptimizerGraphs;

/// <summary>
/// ViewModel for displaying the Costs Graph using LiveCharts2
/// </summary>
internal partial class OptimizerCostsGraphViewModel : BaseOptimizerGraphViewModel
{
    /// <summary>
    /// Constructor of the <see cref="OptimizerCostsGraphViewModel"/> partial class
    /// </summary>
    /// <param name="schedules">List of the Heat production unit's schedules</param>
    /// <param name="OrderedTimes">The order for time for the chart axis labeling</param>
    /// <param name="minDate">Minimum date in given dataset</param>
    public OptimizerCostsGraphViewModel(List<HeatProductionUnitSchedule> schedules, List<DateTime> OrderedTimes, DateTimeOffset? minDate) : base(schedules, OrderedTimes, minDate)
    {
    }

    /// <summary>
    /// Create the chart series
    /// </summary>
    /// <param name="schedules">List of Unit Schedules</param>
    protected override void BuildChartSeries(List<HeatProductionUnitSchedule> schedules)
    {
        Series.Clear();
        int i = 0;

        var strokeThickness = 3;
        var strokeDashArray = new float[] { 3 * strokeThickness, 2 * strokeThickness };
        var effect = new DashEffect(strokeDashArray);

        List<decimal> totalCostsPerHour = new List<decimal>(new decimal[orderedTimes.Count]);

        foreach (var unitSchedule in schedules)
        {
            i++;
            for (int j = 0; j < unitSchedule.Costs.Length; j++)
            {

                totalCostsPerHour[j] += unitSchedule.Costs[j];
            }

            Series.Add(new LineSeries<double>
            {
                Values = unitSchedule.Costs.Select(cost => (double)cost).ToArray(),
                Name = unitSchedule.Name,
                Stroke = new SolidColorPaint
                {
                    Color = Colors[i],
                    StrokeCap = SKStrokeCap.Square,
                    StrokeThickness = strokeThickness,
                    PathEffect = effect
                },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 1
            });
        }
        Series.Add(new LineSeries<double>
        {
            Values = totalCostsPerHour.Select(cost => (double)cost).ToArray(),
            Name = "Accumulative cost",
            Stroke = new SolidColorPaint(Colors[i + 1]) { StrokeThickness = 5 },
            Fill = null,
            GeometryFill = null,
            GeometryStroke = null,
            LineSmoothness = 1
        });
    }


    /// <summary>
    /// Gets the Y-axis title - each subclass must implement this
    /// </summary>
    protected override string GetYAxisTitle() => "Cost [ Dkk ]";

}