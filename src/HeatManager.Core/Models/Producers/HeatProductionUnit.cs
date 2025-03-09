using HeatManager.Core.Models.Resources;

namespace HeatManager.Core.Models.Producers;

internal class HeatProductionUnit : IHeatProductionUnit
{
    public string Name { get; set; }

    public decimal Cost { get; set; }

    public double MaxHeatProduction { get; set; }

    public double ResourceConsumption { get; set; }

    public IBasicResource Resource { get; set; }

    public double Emissions { get; set; }
}