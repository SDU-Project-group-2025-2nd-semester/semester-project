using HeatManager.Core.Models.Schedules;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;

namespace HeatManager.ViewModels.OptimizerGraphs;

/// <summary>
/// ViewModel for displaying a heat production graph using LiveCharts.
/// </summary>
internal partial class OptimizerHeatProductionGraphViewModel : BaseOptimizerGraphViewModel
{

    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizerHeatProductionGraphViewModel"/> class.
    /// </summary>
    /// <param name="schedules">The list of heat production unit schedules.</param>
    /// <param name="OrderedTimes">The ordered time slots for chart axis labeling.</param>
    /// <param name="minDate">The minimum date in the dataset.</param>
    public OptimizerHeatProductionGraphViewModel(List<HeatProductionUnitSchedule> schedules, List<DateTime> OrderedTimes, DateTimeOffset? minDate) : base(schedules, OrderedTimes, minDate)
    {
    }

    /// <summary>
    /// Gets the filename prefix used when exporting the chart to an image file.
    /// </summary>
    protected override string FilenamePrefixOnExport => "HeatProductionChart";

    /// <summary>
    /// Builds the chart series based on provided schedule data.
    /// </summary>
    /// <param name="schedules">The list of schedules.</param>
    protected override void BuildChartSeries(List<HeatProductionUnitSchedule> schedules)
    {
        Series.Clear();
        int i = 0;

        foreach (var unitSchedule in schedules)
        {
            i++;
            Series.Add(new StackedColumnSeries<double>
            {
                Values = unitSchedule.HeatProduction,
                Name = unitSchedule.Name,
                Fill = new SolidColorPaint(Colors[i])
            });
        }
    }

    protected override void BuildChartSeries(Schedule schedule)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the Y-axis title - each subclass must implement this
    /// </summary>
    protected override string GetYAxisTitle() => "Heat Production [ MW ]";
}
