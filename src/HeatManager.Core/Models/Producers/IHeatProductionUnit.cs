using HeatManager.Core.Models.Resources;

namespace HeatManager.Core.Models.Producers;

public interface IHeatProductionUnit
{
    public string Name { get; set; }

    /// <summary>
    /// DKK/MWh(th)
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// MW
    /// </summary>
    public double MaxHeatProduction { get; set; }

    /// <summary>
    /// MWh(resource)/MWh(th)
    /// </summary>
    public double ResourceConsumption { get; set; }

    public IBasicResource Resource { get; set; }

    /// <summary>
    /// kg/MWh(th)
    /// </summary>
    public double Emissions { get; set; }
    
    public IHeatProductionUnit Clone();
}