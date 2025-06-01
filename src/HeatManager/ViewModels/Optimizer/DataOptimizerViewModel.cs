using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services.Optimizers;
using HeatManager.ViewModels.OptimizerGraphs;
using HeatManager.Views.OptimizerGraphs;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HeatManager.ViewModels.Optimizer;

internal partial class DataOptimizerViewModel : ViewModelBase, IDataOptimizerViewModel, INotifyPropertyChanged
{
    private readonly IOptimizer _optimizer;

    private DateTimeOffset? _selectedDate;
    private string? _lastLabel;
    private Schedule _schedule;
    private List<HeatProductionUnitSchedule> schedules;
    private List<DateTime> orderedTimes;
    
    private OptimizerSettings _settings;

    /// <summary>
    /// Gets or sets the current view displayed in the UI.
    /// </summary>
    [ObservableProperty]
    private UserControl? currentView;

    /// <summary>
    /// Gets or sets the currently selected view type.
    /// </summary>
    [ObservableProperty]
    private OptimizerViewType? selectedView;

    /// <summary>
    /// Gets or sets the currently selected view option.
    /// </summary>
    [ObservableProperty]
    private ViewOption? selectedViewOption;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataOptimizerViewModel"/> class.
    /// </summary>
    /// <param name="optimizer">The optimizer service.</param>
    public DataOptimizerViewModel(IOptimizer optimizer)
    {
        _optimizer = optimizer;
        _schedule = _optimizer.Optimize();
        schedules = _schedule.HeatProductionUnitSchedules.ToList();
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

    /// <summary>
    /// Represents a selectable view option for the optimizer display.
    /// </summary>
    /// <param name="DisplayName">The display name of the view.</param>
    /// <param name="ViewType">The type of the view.</param>
    public record ViewOption(string DisplayName, OptimizerViewType ViewType);

    /// <summary>
    /// Gets the list of available view options.
    /// </summary>
    public IEnumerable<ViewOption> ViewOptions => new[]
    {
        new ViewOption("Heat Production", OptimizerViewType.HeatProductionGraph),
        new ViewOption("Total and Maximum Values", OptimizerViewType.SummaryTable),
        new ViewOption("Costs", OptimizerViewType.CostsGraph),
        new ViewOption("Co2 Emissions", OptimizerViewType.Co2Graph),
        new ViewOption("Resource Consumption", OptimizerViewType.ResourceConsumptionGraph)
    };

    /// <summary>
    /// Sets the current view to the summary table view.
    /// </summary>
    [RelayCommand]
    private void SetSummaryTableView()
    {
        CurrentView = new OptimizerSummaryTableView { DataContext = new OptimizerSummaryTableViewModel(schedules) };
    }

    /// <summary>
    /// Sets the current view to the heat production graph view.
    /// </summary>
    [RelayCommand]
    private void SetHeatProductionGraphView()
    {
        CurrentView = new OptimizerHeatProductionGraphView { DataContext = new OptimizerHeatProductionGraphViewModel(schedules, orderedTimes, MinDate) };
    }

    /// <summary>
    /// Sets the current view to the costs production graph view.
    /// </summary>
    [RelayCommand]
    private void SetCostsGraphView()
    {
        CurrentView = new OptimizerCostsGraphView { DataContext = new OptimizerCostsGraphViewModel(schedules, orderedTimes, MinDate) };
    }
    
    private void SetCo2GraphView()
    {
        CurrentView = new OptimizerCo2GraphView() { DataContext = new OptimizerCo2GraphViewModel(schedules, orderedTimes, MinDate) };
    }
    
    private void SetResourceConsumptionGraphView()
    {
        _schedule = _optimizer.Optimize();
        CurrentView = new OptimizerResourceConsumptionView() { DataContext = new OptimizerResourceConsumptionViewModel(_schedule, orderedTimes, MinDate) };
    }


    /// <summary>
    /// Handles changes to the selected view option and updates the selected view type.
    /// </summary>
    /// <param name="value">The newly selected view option.</param>
    partial void OnSelectedViewOptionChanged(ViewOption? value)
    {
        if (value is not null)
        {
            SelectedView = value.ViewType;
        }
    }

    /// <summary>
    /// Handles changes to the selected view type and updates the current view accordingly.
    /// </summary>
    /// <param name="value">The newly selected view type.</param>
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
            case OptimizerViewType.CostsGraph:
                SetCostsGraphView();
                break;
            case OptimizerViewType.Co2Graph:
                SetCo2GraphView(); 
                break;
            case OptimizerViewType.ResourceConsumptionGraph:
                SetResourceConsumptionGraphView();
                break;
        }
    }

    /// <summary>
    /// Orders all time slots from the schedules and sets the date range.
    /// </summary>
    /// <param name="schedules">The list of heat production unit schedules.</param>
    /// <returns>A list of distinct, ordered DateTime values.</returns>
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

    /// <summary>
    /// Sets the minimum and maximum date values based on the provided list of time points.
    /// </summary>
    /// <param name="times">A list of DateTime objects.</param>
    private void SetDateRange(List<DateTime> times)
    {
        if (times.Count > 0)
        {
            MinDate = new DateTimeOffset(times.First());
            MaxDate = new DateTimeOffset(times.Last());
        }
    }
}

/// <summary>
/// Represents the types of views available in the optimizer.
/// </summary>
public enum OptimizerViewType
{
    HeatProductionGraph,
    SummaryTable,
    CostsGraph, 
    Co2Graph, 
    ResourceConsumptionGraph
}
