namespace HeatManager.Core.Models.Schedules;

public interface IHeatProductionUnitSchedule /*: IHeatProductionUnit*/
{
    public bool[] IsOn { get; }

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

    public IResourceConsumptionSchedule ResourceConsumption2 { get; }

    /// <summary>
    /// kg/MWh(th)
    /// </summary>
    public double Emissions { get; }
}