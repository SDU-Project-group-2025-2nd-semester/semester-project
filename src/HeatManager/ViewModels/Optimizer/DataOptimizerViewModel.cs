using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Models.Schedules;

namespace HeatManager.ViewModels.Optimizer;

internal partial class DataOptimizerViewModel : ViewModelBase, IDataOptimizerViewModel, INotifyPropertyChanged
{

    private readonly IOptimizer _optimizer;
    private readonly ObservableCollection<ObservablePoint> _values = new();
    private DateTimeOffset? _selectedDate;
    private string? _lastLabel;

    private bool _isChartVisible = true;
    private bool _isTableVisible = false;



    /// <summary>
    /// Initializes a new instance of the <see cref="DataOptimizerViewModel"/> class.
    /// </summary>
    /// <param name="optimizer">Injected optimizer service.</param>
    public DataOptimizerViewModel(IOptimizer optimizer)
    {
        _optimizer = optimizer;
        OptimizeData();
    }

    

    /// <summary>
    /// Gets the chart series collection.
    /// </summary>
    public ObservableCollection<ISeries> Series { get; } = new();

    /// <summary>
    /// Gets the tabular schedule data.
    /// </summary>
    public ObservableCollection<ScheduleData> TableData { get; } = new();

    /// <summary>
    /// Gets or sets the selected date for filtering.
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
    /// Gets the minimum date in the dataset.
    /// </summary>
    public DateTimeOffset? MinDate { get; private set; }

    /// <summary>
    /// Gets the maximum date in the dataset.
    /// </summary>
    public DateTimeOffset? MaxDate { get; private set; }

    /// <summary>
    /// Gets the date range text for display.
    /// </summary>
    public string DateRangeText =>
        (MinDate.HasValue && MaxDate.HasValue)
            ? $"Available data: {MinDate.Value:dd MMM yyyy} - {MaxDate.Value:dd MMM yyyy}"
            : "No data range available";

    /// <summary>
    /// Gets the X-axis configuration.
    /// </summary>
    public List<Axis> XAxes { get; private set; } = new();

    /// <summary>
    /// Gets the Y-axis configuration.
    /// </summary>
    public List<Axis> YAxes { get; private set; } = new();

    /// <summary>
    /// Gets or sets the chart margin.
    /// </summary>
    public Margin Margin { get; set; }

    /// <summary>
    /// Gets or sets whether the chart is visible.
    /// </summary>
    public bool IsChartVisible
    {
        get => _isChartVisible;
        set
        {
            if (SetProperty(ref _isChartVisible, value))
            {
                IsTableVisible = !value;
            }
        }
    }

    /// <summary>
    /// Gets or sets whether the table is visible.
    /// </summary>
    public bool IsTableVisible
    {
        get => !_isChartVisible;
        set => SetProperty(ref _isTableVisible, value);
    }

    

    /// <summary>
    /// Toggles between chart and table views.
    /// </summary>
    public void ToggleView()
    {
        IsChartVisible = !IsChartVisible;
    }

    /// <summary>
    /// Handles chart update events.
    /// </summary>
    /// <param name="args">Chart event arguments.</param>
    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (ICartesianChartView)args.Chart;
    }

    

    /// <summary>
    /// Runs the optimization and sets up chart and table data.
    /// </summary>
    private void OptimizeData()
    {
        var schedule = _optimizer.Optimize();
        List<HeatProductionUnitSchedule> schedules = schedule.HeatProductionUnitSchedules.ToList();

        Series.Clear();
        _values.Clear();

        List<DateTime> orderedTimes = OrderTimeSlots(schedules);

        BuildChartSeries(schedules);
        BuildTableData(schedules);
        ConfigureAxes(orderedTimes, schedules);
    }

    private List<DateTime> OrderTimeSlots(List<HeatProductionUnitSchedule> schedules)
    {
        var timeslots = schedules
            .SelectMany(unitSchedule => unitSchedule.DataPoints)
            .ToList();
        var orderedTimes = timeslots
            .Select(t => t.TimeFrom)
            .Distinct()
            .OrderBy(t => t)
            .ToList();
        if (orderedTimes.Count > 0)
        {
            SetDateRange(orderedTimes);
        }

        return orderedTimes;
    }

    private void SetDateRange(List<DateTime> times)
    {
        if (times.Count > 0)
        {
            MinDate = new DateTimeOffset(times.First());
            MaxDate = new DateTimeOffset(times.Last());
        }
    }

    private void BuildChartSeries(List<HeatProductionUnitSchedule> schedules)
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

    private void BuildTableData(List<HeatProductionUnitSchedule> schedules)
    {
        TableData.Clear();

        foreach (var unitSchedule in schedules)
        {
            TableData.Add(new ScheduleData
            {
                Name = unitSchedule.Name,
                HeatProduction = Math.Round(Convert.ToDecimal(unitSchedule.TotalHeatProduction), 3),
                MaxHeatProduction = Math.Round(Convert.ToDecimal(unitSchedule.MaxHeatProduction), 3),
                Emissions = Math.Round(Convert.ToDecimal(unitSchedule.TotalEmissions), 3),
                MaxEmissions = Math.Round(Convert.ToDecimal(unitSchedule.MaxEmissions), 3),
                Cost = Math.Round(unitSchedule.TotalCost, 2),
                MaxCost = Math.Round(unitSchedule.MaxCost, 2),
                ResourceConsumption = Math.Round(Convert.ToDecimal(unitSchedule.TotalResourceConsumption), 3),
                MaxResourceConsumption = Math.Round(Convert.ToDecimal(unitSchedule.MaxResourceConsumption), 3),
                Utilization = Math.Round(Convert.ToDecimal(unitSchedule.TotalUtilization), 3),
                MaxUtilization = Math.Round(Convert.ToDecimal(unitSchedule.MaxUtilization), 3)
            });
        }
    }

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
                    Name = "[ h ]",
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
                    Name = "Heat Production [ MW ]"
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
    /// Sets the X-axis window based on selected date.
    /// </summary>
    private void SetXMinLimit()
    {
        var orderedTimes = _optimizer.Optimize().HeatProductionUnitSchedules
            .SelectMany(unitSchedule => unitSchedule.DataPoints)
            .Select(t => t.TimeFrom)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

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
    /// Predefined chart color palette.
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

/// <summary>
/// Represents a row of schedule data for the UI.
/// </summary>
public class ScheduleData
{
    public required string Name { get; set; }
    public decimal HeatProduction { get; set; }
    public decimal MaxHeatProduction { get; set; }
    public decimal Emissions { get; set; }
    public decimal MaxEmissions { get; set; }
    public decimal Cost { get; set; }
    public decimal MaxCost { get; set; }
    public decimal ResourceConsumption { get; set; }
    public decimal MaxResourceConsumption { get; set; }
    public decimal Utilization { get; set; }
    public decimal MaxUtilization { get; set; }
}
