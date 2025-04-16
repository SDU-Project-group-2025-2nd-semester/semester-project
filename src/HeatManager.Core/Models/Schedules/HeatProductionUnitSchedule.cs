namespace HeatManager.Core.Models.Schedules;

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