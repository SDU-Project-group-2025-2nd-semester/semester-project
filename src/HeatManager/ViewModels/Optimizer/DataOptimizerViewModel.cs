using HeatManager.Core.ResultData;
using HeatManager.Core.Services.Optimizers;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeatManager.ViewModels.Optimizer;

internal class DataOptimizerViewModel : ViewModelBase, IDataOptimizerViewModel
{
    public ObservableCollection<ISeries> Series { get; } = [];
    public List<Axis> XAxes { get; private set; } = [];
    public List<Axis> YAxes { get; private set; } = [];

    private readonly IOptimizer _optimizer;

    private DateTimeOffset? _selectedMinTime;
    public DateTimeOffset? SelectedMinTime
    {
        get => _selectedMinTime;
        set
        {
            _selectedMinTime = value;
            OptimizeData(); // reoptimize whenever time selection changes
        }
    }

    private DateTimeOffset? _selectedMaxTime;
    public DateTimeOffset? SelectedMaxTime
    {
        get => _selectedMaxTime;
        set
        {
            _selectedMaxTime = value;
            OptimizeData(); // reoptimize whenever time selection changes
        }
    }

    private DateTimeOffset? _selectedMinDate;
    public DateTimeOffset? SelectedMinDate
    {
        get => _selectedMinDate;
        set
        {
            if (SetProperty(ref _selectedMinDate, value))
                UpdateSelectedMinTime();
        }
    }

    private TimeSpan? _selectedMinTimeOfDay;
    public TimeSpan? SelectedMinTimeOfDay
    {
        get => _selectedMinTimeOfDay;
        set
        {
            if (SetProperty(ref _selectedMinTimeOfDay, value))
                UpdateSelectedMinTime();
        }
    }

    // Same for Max
    private DateTimeOffset? _selectedMaxDate;
    public DateTimeOffset? SelectedMaxDate
    {
        get => _selectedMaxDate;
        set
        {
            if (SetProperty(ref _selectedMaxDate, value))
                UpdateSelectedMaxTime();
        }
    }

    private TimeSpan? _selectedMaxTimeOfDay;
    public TimeSpan? SelectedMaxTimeOfDay
    {
        get => _selectedMaxTimeOfDay;
        set
        {
            if (SetProperty(ref _selectedMaxTimeOfDay, value))
                UpdateSelectedMaxTime();
        }
    }

    // Combine them
    private void UpdateSelectedMinTime()
    {
        if (SelectedMinDate.HasValue && SelectedMinTimeOfDay.HasValue)
        {
            SelectedMinTime = SelectedMinDate.Value.Date + SelectedMinTimeOfDay.Value;
        }
        else
        {
            SelectedMinTime = null;
        }
    }

    private void UpdateSelectedMaxTime()
    {
        if (SelectedMaxDate.HasValue && SelectedMaxTimeOfDay.HasValue)
        {
            SelectedMaxTime = SelectedMaxDate.Value.Date + SelectedMaxTimeOfDay.Value;
        }
        else
        {
            SelectedMaxTime = null;
        }
    }



    public DataOptimizerViewModel(IOptimizer optimizer)
    {
        _optimizer = optimizer;

        OptimizeData();
    }

    public void OptimizeData()
    {
        var schedule = _optimizer.Optimize();
        var schedules = schedule.HeatProductionUnitSchedules.ToList();

        Series.Clear();

        var timeslots = new List<IHeatProductionUnitResultDataPoint>();

        foreach (var unitSchedule in schedules)
        {
            timeslots.AddRange(unitSchedule.DataPoints);
        }

        // Build a list of unique sorted times
        var orderedTimes = timeslots
            .Select(t => t.TimeFrom)
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        var filteredTimes = orderedTimes.AsEnumerable();

        if (SelectedMinTime.HasValue)
            filteredTimes = filteredTimes.Where(t => t >= SelectedMinTime.Value.DateTime);

        if (SelectedMaxTime.HasValue)
            filteredTimes = filteredTimes.Where(t => t <= SelectedMaxTime.Value.DateTime);

        var filteredTimeslots = timeslots
            .Where(dp =>
                (!SelectedMinTime.HasValue || dp.TimeFrom >= SelectedMinTime.Value) &&
                (!SelectedMaxTime.HasValue || dp.TimeFrom <= SelectedMaxTime.Value))
            .ToList();

        orderedTimes = filteredTimeslots
            .Select(t => t.TimeFrom)
            .Distinct()
            .OrderBy(t => t)
            .ToList();


        // Create a dictionary to map times to indexes
        var timeIndexMap = orderedTimes
            .Select((time, index) => new { time, index })
            .ToDictionary(x => x.time, x => (double)x.index);

        for (int i = 0; i < schedules.Count; i++)
        {
            var unitSchedule = schedules[i];

            var filteredDataPoints = unitSchedule.DataPoints
                .Where(dp =>
                    (!SelectedMinTime.HasValue || dp.TimeFrom >= SelectedMinTime.Value) &&
                    (!SelectedMaxTime.HasValue || dp.TimeFrom <= SelectedMaxTime.Value))
                .OrderBy(dp => dp.TimeFrom)
                .ToList();

            var values = orderedTimes
                .Select(time =>
                {
                    var dataPoint = filteredDataPoints.FirstOrDefault(dp => dp.TimeFrom == time);
                    return dataPoint?.HeatProduction ?? 0;
                })
                .ToList();



            Series.Add(new StackedColumnSeries<double>
            {
                Values = values,
                Name = unitSchedule.Name,
                Fill = new SolidColorPaint(Colors[i % Colors.Length])
            });
        }

        // Update X axis
        XAxes = new List<Axis>
        {
            new Axis
            {
                Name = "Time",
                Labels = orderedTimes.Select(t => t.ToString("HH:mm dd/MM")).ToArray(),
                LabelsRotation = 15,
                MinLimit = 0,
                MaxLimit = orderedTimes.Count > 0 ? orderedTimes.Count - 1 : 0
            }
        };


        YAxes = new List<Axis>
        {
            new Axis
            {
                Name = "Heat Production"
            }
        };
    }



    public static readonly SKColor[] Colors = new[]
    {
        SKColors.Red, SKColors.Green, SKColors.Blue, SKColors.Yellow, SKColors.Orange,
        SKColors.Purple, SKColors.Pink, SKColors.Brown, SKColors.Gray, SKColors.Black,
        SKColors.White, SKColors.Cyan, SKColors.Magenta, SKColors.Lime, SKColors.Teal,
        SKColors.Navy, SKColors.Olive, SKColors.Maroon, SKColors.Aqua, SKColors.Silver,
        SKColors.Gold
    };
}
