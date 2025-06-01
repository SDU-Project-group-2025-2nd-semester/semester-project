using HeatManager.Core.Models.Schedules;
using HeatManager.ViewModels.Optimizer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HeatManager.ViewModels.OptimizerGraphs;

/// <summary>
/// ViewModel responsible for preparing and exposing summary table data for heat production unit schedules.
/// </summary>
internal partial class OptimizerSummaryTableViewModel : ViewModelBase, IDataOptimizerViewModel, INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptimizerSummaryTableViewModel"/> class.
    /// </summary>
    /// <param name="schedules">The list of heat production unit schedules.</param>
    public OptimizerSummaryTableViewModel(List<HeatProductionUnitSchedule> schedules)
    {
        BuildTableData(schedules);
    }

    /// <summary>
    /// Gets the tabular schedule data to be displayed in the UI.
    /// </summary>
    public ObservableCollection<ScheduleData> TableData { get; } = new();

    /// <summary>
    /// Builds the table data from the provided list of schedules.
    /// </summary>
    /// <param name="schedules">The list of schedules to process.</param>
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
                ResourceConsumption = Math.Round(Convert.ToDecimal(unitSchedule.TotalResourceConsumption.Value), 3),
                MaxResourceConsumption = Math.Round(Convert.ToDecimal(unitSchedule.MaxResourceConsumption.Value), 3),
                Utilization = Math.Round(Convert.ToDecimal(unitSchedule.TotalUtilization), 3),
                MaxUtilization = Math.Round(Convert.ToDecimal(unitSchedule.MaxUtilization), 3)
            });
        }
    }
}

/// <summary>
/// Represents a row of schedule data for display in a summary table.
/// </summary>
public class ScheduleData
{
    /// <summary>
    /// Gets or sets the name of the production unit.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the total heat production.
    /// </summary>
    public decimal HeatProduction { get; set; }

    /// <summary>
    /// Gets or sets the maximum heat production observed.
    /// </summary>
    public decimal MaxHeatProduction { get; set; }

    /// <summary>
    /// Gets or sets the total emissions.
    /// </summary>
    public decimal Emissions { get; set; }

    /// <summary>
    /// Gets or sets the maximum emissions observed.
    /// </summary>
    public decimal MaxEmissions { get; set; }

    /// <summary>
    /// Gets or sets the total cost.
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// Gets or sets the maximum cost observed.
    /// </summary>
    public decimal MaxCost { get; set; }

    /// <summary>
    /// Gets or sets the total resource consumption.
    /// </summary>
    public decimal ResourceConsumption { get; set; }

    /// <summary>
    /// Gets or sets the maximum resource consumption observed.
    /// </summary>
    public decimal MaxResourceConsumption { get; set; }

    /// <summary>
    /// Gets or sets the total utilization.
    /// </summary>
    public decimal Utilization { get; set; }

    /// <summary>
    /// Gets or sets the maximum utilization observed.
    /// </summary>
    public decimal MaxUtilization { get; set; }
}