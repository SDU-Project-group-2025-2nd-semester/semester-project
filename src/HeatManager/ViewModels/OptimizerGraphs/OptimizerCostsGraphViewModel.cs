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

namespace HeatManager.ViewModels.OptimizerGraphs;

/// <summary>
/// ViewModel for displaying the Costs Graph using LiveCharts2
/// </summary>
internal partial class OptimizerCostsGraphViewModel : ViewModelBase, IDataOptimizerViewModel, INotifyPropertyChanged
{
    private DateTimeOffset? _selectedDate;
    private string? _lastLabel;
    private List<DateTime> orderedTimes;

    /// <summary>
    /// Chart Series collection
    /// </summary>
    public ObservableCollection<ISeries> Series { get; } = new();

    /// <summary>
    /// Set the selected date for focusing on chart
    /// </summary>
    public DateTimeOffset? SelectedDate
    {
        get => _selectedDate;
        set
        {
            SetProperty(ref _selectedDate, value);
            OnDateSelected();
        }
    }

    /// <summary>
    /// Get minimum date from dataset
    /// </summary>
    public DateTimeOffset? MinDate { get; private set; }

    /// <summary>
    /// Create X axis
    /// </summary>
    public List<Axis> XAxes { get; private set; } = new();

    /// <summary>
    /// Create Y axis
    /// </summary>
    public List<Axis> YAxes { get; private set; } = new();

    /// <summary>
    /// Create chart's margin
    /// </summary>
    public Margin? Margin { get; set; }

    /// <summary>
    /// Constructor of the <see cref="OptimizerCostsGraphViewModel"/> partial class
    /// </summary>
    /// <param name="schedules">List of the Heat production unit's schedules</param>
    /// <param name="OrderedTimes">The order for time for the chart axis labeling</param>
    /// <param name="minDate">Minimum date in given dataset</param>
    public OptimizerCostsGraphViewModel(List<HeatProductionUnitSchedule> schedules, List<DateTime> OrderedTimes, DateTimeOffset? minDate)
    {
        orderedTimes = OrderedTimes;
        MinDate = minDate;
        BuildChartSeries(schedules);
        ConfigureAxes(OrderedTimes, schedules);
    }

    /// <summary>
    ///  Handles chart's update events.
    /// </summary>
    /// <param name="args"></param>
    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (ICartesianChartView)args.Chart;
    }

    /// <summary>
    /// Create the chart series
    /// </summary>
    /// <param name="schedules">List of Unit Schedules</param>
    private void BuildChartSeries(List<HeatProductionUnitSchedule> schedules)
    {
        Series.Clear();
        int i = 0;

        foreach (var unitSchedule in schedules)
        {
            i++;
            Series.Add(new LineSeries<double>
            {
                Values = unitSchedule.Costs.Select(cost => (double)cost).ToArray(),
                Name = unitSchedule.Name,
                Stroke = new SolidColorPaint(Colors[i]) { StrokeThickness = 4 },
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                LineSmoothness = 1
            });
        }
    }

    /// <summary>
    /// Configures the chart's X and Y axes using provided time and schedule data.
    /// </summary>
    /// <param name="orderedTimes">The list of ordered time slots.</param>
    /// <param name="schedules">The list of schedules.</param>
    private void ConfigureAxes(List<DateTime> orderedTimes, List<HeatProductionUnitSchedule> schedules)
    {
        int i = 0;
        foreach (var unitSchedule in schedules)
        {
            i++;

            XAxes = new List<Axis>
            {
                new Axis
                {
                    Name = " Date [ h ]",
                    MinLimit = 0,
                    MaxLimit = orderedTimes.Count > 0 ? orderedTimes.Count - 1 : 0,
                    Labeler = value =>
                    {
                        int index = (int)Math.Round(value);
                        if (index >= 0 && index < orderedTimes.Count)
                        {
                            var dateAxis = XAxes.FirstOrDefault();
                            var visibleRange = dateAxis?.MaxLimit - dateAxis?.MinLimit;
                            var t = orderedTimes[index];

                            string currentLabel = visibleRange > 45
                                ? t.ToString("dd/MM/yy")
                                : t.ToString("HH:mm dd/MM/yy");

                            if (visibleRange > 45)
                            {
                                if (currentLabel == _lastLabel)
                                    return string.Empty;

                                _lastLabel = currentLabel;
                            }

                            return currentLabel;
                        }

                        return string.Empty;
                    }
                }
            };

            YAxes = new List<Axis>
            {
                new Axis
                {
                    Name = "Cost [ Dkk ]"
                }
            };

            Margin = new Margin(100, Margin.Auto, 50, Margin.Auto);
            SelectedDate = MinDate;
        }
    }

    /// <summary>
    /// Called when a new date is selected.
    /// </summary>
    private void OnDateSelected()
    {
        if (SelectedDate.HasValue)
        {
            SetXMinLimit();
        }
    }

    /// <summary>
    /// Sets the X-axis window range based on the selected date.
    /// </summary>
    private void SetXMinLimit()
    {
        if (SelectedDate.HasValue)
        {
            var index = orderedTimes.FindIndex(t => t >= SelectedDate.Value.DateTime);

            if (index != -1)
            {
                XAxes[0].MinLimit = index;
                XAxes[0].MaxLimit = Math.Min(index + 30, orderedTimes.Count - 1);
            }
            else
            {
                XAxes[0].MinLimit = 0;
            }
        }
        else
        {
            XAxes[0].MinLimit = 0;
        }
    }


    /// <summary>
    /// Predefined color palette for chart series.
    /// </summary>
    private static readonly SKColor[] Colors =
    {
        SKColors.White, SKColors.Maroon, SKColors.Red, SKColors.Magenta, SKColors.Pink,
        SKColors.Green, SKColors.Blue, SKColors.Yellow, SKColors.Orange, SKColors.Purple,
        SKColors.Brown, SKColors.Gray, SKColors.Black, SKColors.Cyan, SKColors.Lime,
        SKColors.Teal, SKColors.Navy, SKColors.Olive, SKColors.Aqua, SKColors.Silver,
        SKColors.Gold
    };

}