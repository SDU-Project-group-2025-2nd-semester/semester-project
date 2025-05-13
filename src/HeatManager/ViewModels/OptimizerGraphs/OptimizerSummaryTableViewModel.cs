using HeatManager.Core.Models.Schedules;
using HeatManager.ViewModels.Optimizer;
using LiveChartsCore.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatManager.ViewModels.OptimizerGraphs;
internal partial class OptimizerSummaryTableViewModel : ViewModelBase, IDataOptimizerViewModel, INotifyPropertyChanged
{

    private readonly ObservableCollection<ObservablePoint> _values = new();

    public OptimizerSummaryTableViewModel(List<HeatProductionUnitSchedule> schedules)
    {
        BuildTableData(schedules);
    }


    /// <summary>
    /// Gets the tabular schedule data.
    /// </summary>
    public ObservableCollection<ScheduleData> TableData { get; } = new();

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

