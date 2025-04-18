using HeatManager.Core.Models.Resources;
using HeatManager.Core.Services.AssetManagers;
using System.Text.Json.Serialization;

namespace HeatManager.Core.Models.Producers;

public class HeatProductionUnit : IHeatProductionUnit
{
    public required string Name { get; set; }

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

    [JsonConverter(typeof(BasicResourceConverter))]
    public required Resource Resource { get; set; }

    /// <summary>
    /// kg/MWh(th)
    /// </summary>
    public double Emissions { get; set; }
    
    public IHeatProductionUnit Clone()
    {
        return new HeatProductionUnit
        {
            Name = Name,
            Cost = Cost,
            MaxHeatProduction = MaxHeatProduction,
            ResourceConsumption = ResourceConsumption,
            Resource = Resource,
            Emissions = Emissions
        };
    }
}