using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Extensions;
using SkiaSharp;
using CommunityToolkit.Mvvm.Input;

using HeatManager.Core.Models.Schedules;
using HeatManager.ViewModels.Optimizer;


namespace HeatManager.ViewModels.OptimizerGraphs;

internal partial class OptimizerCostsPieGraphViewModel : ViewModelBase
{
    public ObservableCollection<ISeries> MaxCostSeries { get; set; } = new();
    public ObservableCollection<ISeries> TotalCostSeries { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizerCostsPieChartsViewModel"/> class.
    /// </summary>
    /// <param name="schedules">The list of heat production unit schedules.</param>
    /// <param name="OrderedTimes">The ordered time slots for chart axis labeling.</param>
    /// <param name="minDate">The minimum date in the dataset.</param>
    public OptimizerCostsPieGraphViewModel(List<HeatProductionUnitSchedule> schedules)
    {
        BuildPieSeries(schedules);
    }

    public void BuildPieSeries(List<HeatProductionUnitSchedule> schedules)
    {
        MaxCostSeries.Clear();
        TotalCostSeries.Clear();

        decimal SumOfMaxAllUnits = schedules.Sum(schedule => schedule.MaxCost);
        decimal SumOfTotalAllUnits = schedules.Sum(schedule => schedule.TotalCost);

        var MaxCostGaugeItems = new List<GaugeItem>();
        var TotalCostGaugeItems = new List<GaugeItem>();

        var maxItems = new GaugeItem[schedules.Count + 1]; // +1 for background
        maxItems[0] = new GaugeItem(GaugeItem.Background, series => { series.Fill = null; });

        var totalItems = new GaugeItem[schedules.Count + 1]; // +1 for background
        totalItems[0] = new GaugeItem(GaugeItem.Background, series => { series.Fill = null; });

        for (int i = 0; i < schedules.Count; i++)
        {
            var unitSchedule = schedules[i];

            maxItems[i + 1] = new GaugeItem(
                (double)GetProportionalValue(SumOfMaxAllUnits, unitSchedule.MaxCost),
                series => SetStyle($"{unitSchedule.Name}: {unitSchedule.MaxCost} DKK", series));

            totalItems[i + 1] = new GaugeItem(
                (double)GetProportionalValue(SumOfTotalAllUnits, unitSchedule.TotalCost),
                series => SetStyle($"{unitSchedule.Name}: {unitSchedule.TotalCost} DKK", series));
        }

        var maxSeries = GaugeGenerator.BuildSolidGauge(maxItems);
        var totalSeries = GaugeGenerator.BuildSolidGauge(totalItems);

        foreach (var series in maxSeries)
        {
            MaxCostSeries.Add(series);
        }

        foreach (var series in totalSeries)
        {
            TotalCostSeries.Add(series);
        }
    }



    private decimal GetProportionalValue(decimal sumUnits, decimal singleUnit)
    {
        return Math.Round(singleUnit / sumUnits * 100, 2);
    }

    public static void SetStyle(string name, PieSeries<ObservableValue> series)
    {
        series.Name = name;
        series.DataLabelsSize = 20;
        series.DataLabelsPosition = PolarLabelsPosition.End;
        series.DataLabelsFormatter =
                point => point.Coordinate.PrimaryValue.ToString();
        series.InnerRadius = 20;
        series.MaxRadialColumnWidth = 5;
    }
}
