using System.Collections.Immutable;

namespace HeatManager.Core.Models.Schedules;

internal class Schedule : ISchedule
{
    public ImmutableList<IHeatProductionUnitSchedule> HeatProductionUnitSchedules { get; }
    public ImmutableList<IElectricityProductionUnitSchedule> ElectricityProductionUnitSchedules { get; }
    public TimeSpan Resolution { get; }
    public DateTime Start { get; }
    public DateTime End { get; }
    public double[] Emissions { get; }
    public double[] Costs { get; }
    public double TotalCost { get; }
    public int Length { get; }
    public double TotalEmissions { get; }
    public double[] HeatDemand { get; set; }
    public decimal[] ElectricityPrice { get; set; }
    
}