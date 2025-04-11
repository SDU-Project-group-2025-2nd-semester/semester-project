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

public class HeatProductionUnitSchedule
{
    public double[] Utilization { get; }

    public string Name { get; }

    /// <summary>
    /// DKK/MWh(th)
    /// </summary>
    public decimal Cost { get; }

    /// <summary>
    /// MW
    /// </summary>
    public double MaxHeatProduction { get; }

    /// <summary>
    /// MWh(resource)/MWh(th)
    /// </summary>
    public double ResourceConsumption { get; }

    public ResourceConsumptionSchedule ResourceConsumption2 { get; }

    /// <summary>
    /// kg/MWh(th)
    /// </summary>
    public double Emissions { get; }
}

public class ElectricityProductionUnitSchedule : HeatProductionUnitSchedule
{
    public double MaxElectricity { get; set; }
}

public class ResourceConsumptionSchedule
{
    public string Name { get; }

    public double[] Consumption { get; }

    public double TotalConsumption { get; }
}