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
using HeatManager.Core.ResultData;
using HeatManager.Core.Services.Optimizers;

namespace HeatManager.ViewModels.Optimizer;

public partial class DataOptimizerViewModel : ViewModelBase, INotifyPropertyChanged
{
    private readonly IOptimizer _optimizer;
    private readonly ObservableCollection<ObservablePoint> _values = new();
    private DateTimeOffset? _selectedDate;
    private string? _lastLabel;

    public DataOptimizerViewModel(IOptimizer optimizer)
    {
        _optimizer = optimizer;
        OptimizeData(); // Initialize chart data
    }

    // Currently selected date in the UI
    public DateTimeOffset? SelectedDate
    {
        get => _selectedDate;
        set
        {
            SetProperty(ref _selectedDate, value);
            OnDateSelected();
        }
    }

    // Earliest and latest available dates in the dataset
    public DateTimeOffset? MinDate { get; private set; }
    public DateTimeOffset? MaxDate { get; private set; }

    // Displays the available date range in the UI
    public string DateRangeText =>
        (MinDate.HasValue && MaxDate.HasValue)
            ? $"Available data: {MinDate.Value:dd MMM yyyy} - {MaxDate.Value:dd MMM yyyy}"
            : "No data range available";

    public ObservableCollection<ISeries> Series { get; } = new();
    public List<Axis> XAxes { get; private set; } = new();
    public List<Axis> YAxes { get; private set; } = new();
    public Margin Margin { get; set; }


    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (ICartesianChartView)args.Chart;
    }

    // Runs optimization and populates chart data and axes
    private void OptimizeData()
    {
        var schedule = _optimizer.Optimize();
        var schedules = schedule.HeatProductionUnitSchedules.ToList();

        Series.Clear();
        _values.Clear();

        // Gather all time points from all unit schedules
        var timeslots = new List<IHeatProductionUnitResultDataPoint>();
        foreach (var unitSchedule in schedules)
        {
            timeslots.AddRange(unitSchedule.DataPoints);
        }

        // Extract unique, sorted time values
        var orderedTimes = timeslots
            .Select(t => t.TimeFrom)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        // Set available date range
        if (orderedTimes.Any())
        {
            MinDate = new DateTimeOffset(orderedTimes.First());
            MaxDate = new DateTimeOffset(orderedTimes.Last());
        }

        // Map time points to index values for chart X-axis
        var timeIndexMap = orderedTimes
            .Select((time, index) => new { time, index })
            .ToDictionary(x => x.time, x => (double)x.index);

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

        // Define dynamic X-axis with formatted labels and scroll range
        XAxes = new List<Axis>
        {
            new Axis
            {
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

                        // Avoid repeating the same date label when zoomed out
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

        // Y-axis configuration
        YAxes = new List<Axis>
        {
            new Axis
            {
                Name = "Heat Production [ MW ]"
            }
        };

        // Apply chart margin for layout alignment
        Margin = new Margin(100, Margin.Auto, 50, Margin.Auto);

        // Set initial selected date
        SelectedDate = MinDate;
    }

    // Triggered when a new date is selected
    private void OnDateSelected()
    {
        if (SelectedDate.HasValue)
        {
            SetXMinLimit();
        }
    }

    // Scrolls X-axis to the index corresponding to the selected date
    private void SetXMinLimit()
    {
        var orderedTimes = _optimizer.Optimize().HeatProductionUnitSchedules
            .SelectMany(unitSchedule => unitSchedule.DataPoints)
            .Select(t => t.TimeFrom)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

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

    // Predefined chart colors
    private static readonly SKColor[] Colors =
    {
        SKColors.White,
        SKColors.Maroon,
        SKColors.Red,
        SKColors.Magenta,
        SKColors.Pink,
        SKColors.Green,
        SKColors.Blue,
        SKColors.Yellow,
        SKColors.Orange,
        SKColors.Purple,
        SKColors.Brown,
        SKColors.Gray,
        SKColors.Black,
        SKColors.Cyan,
        SKColors.Lime,
        SKColors.Teal,
        SKColors.Navy,
        SKColors.Olive,
        SKColors.Aqua,
        SKColors.Silver,
        SKColors.Gold
    };
}
