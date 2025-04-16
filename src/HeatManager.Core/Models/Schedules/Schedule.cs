using System.Collections.Immutable;

namespace HeatManager.Core.Models.Schedules;

public class Schedule
{
    public ImmutableList<HeatProductionUnitSchedule> HeatProductionUnitSchedules { get; }

    public ImmutableList<ElectricityProductionUnitSchedule> ElectricityProductionUnitSchedules { get; }

    /// <summary>
    /// Equals to the time between two entries in the schedule
    /// </summary>
    public TimeSpan Resolution { get; }

    public DateTime Start { get; }

    public DateTime End { get; }

    public double[] Emissions { get; }

    public double[] Costs { get; }

    // TODO: Add comments with corresponding units
    public double TotalCost { get; }

    public int Length { get; }

    public double TotalEmissions { get; }

    /// <summary>
    /// MWh
    /// </summary>
    public double[] HeatDemand { get; set; }

    /// <summary>
    /// DKK/MWh(el)
    /// </summary>
    public decimal[] ElectricityPrice { get; set; }
}