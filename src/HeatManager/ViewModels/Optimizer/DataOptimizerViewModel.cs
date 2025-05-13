using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Models.Schedules;
using HeatManager.ViewModels.OptimizerGraphs;
using HeatManager.Views.OptimizerGraphs;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HeatManager.ViewModels.Optimizer;

internal partial class DataOptimizerViewModel : ViewModelBase, IDataOptimizerViewModel, INotifyPropertyChanged
{

    private readonly IOptimizer _optimizer;
    private List<HeatProductionUnitSchedule> schedules;
    private List<DateTime> orderedTimes;

    [ObservableProperty]
    private UserControl? currentView;

    [ObservableProperty]
    private OptimizerViewType? selectedView;

    [ObservableProperty]
    private ViewOption? selectedViewOption;



    public DataOptimizerViewModel(IOptimizer optimizer)
    {
        _optimizer = optimizer;
        var schedule = _optimizer.Optimize();
        schedules = schedule.HeatProductionUnitSchedules.ToList();
        orderedTimes = OrderTimeSlots(schedules);

        SelectedViewOption = ViewOptions.First(v => v.ViewType == OptimizerViewType.HeatProductionGraph);
        SetHeatProductionGraphView();
    }

    /// <summary>
    /// Gets the minimum date in the dataset.
    /// </summary>
    public DateTimeOffset? MinDate { get; private set; }

    /// <summary>
    /// Gets the maximum date in the dataset.
    /// </summary>
    public DateTimeOffset? MaxDate { get; private set; }

    public record ViewOption(string DisplayName, OptimizerViewType ViewType);

    public IEnumerable<ViewOption> ViewOptions => new[]
    {
    new ViewOption("Heat Production", OptimizerViewType.HeatProductionGraph),
    new ViewOption("Total and Maximum Values", OptimizerViewType.SummaryTable)
    };

    /// <summary>
    /// Gets the date range text for display.
    /// </summary>
    public string DateRangeText =>
        (MinDate.HasValue && MaxDate.HasValue)
            ? $"Available data: {MinDate.Value:dd MMM yyyy} - {MaxDate.Value:dd MMM yyyy}"
            : "No data range available";



    [RelayCommand]
    private void SetSummaryTableView()
    {
        CurrentView = new OptimizerSummaryTableView { DataContext = new OptimizerSummaryTableViewModel(schedules) };
    }

    [RelayCommand]
    private void SetHeatProductionGraphView()
    {
        CurrentView = new OptimizerHeatProductionGraphView { DataContext = new OptimizerHeatProductionGraphViewModel(schedules, orderedTimes, MinDate) };
    }

    partial void OnSelectedViewOptionChanged(ViewOption? value)
    {
        if (value is not null)
        {
            SelectedView = value.ViewType;
        }
    }

    partial void OnSelectedViewChanged(OptimizerViewType? value)
    {
        switch (value)
        {
            case OptimizerViewType.HeatProductionGraph:
                SetHeatProductionGraphView();
                break;
            case OptimizerViewType.SummaryTable:
                SetSummaryTableView();
                break;
        }
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

}

public enum OptimizerViewType
{
    HeatProductionGraph,
    SummaryTable
}
