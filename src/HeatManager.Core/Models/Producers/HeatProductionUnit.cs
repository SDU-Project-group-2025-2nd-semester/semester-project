using HeatManager.Core.Models.Resources;
using HeatManager.Core.Services;
using System.Text.Json.Serialization;

namespace HeatManager.Core.Models.Producers;

public class HeatProductionUnit : IHeatProductionUnit
{
    public required string Name { get; set; }

    public decimal Cost { get; set; }

    public double MaxHeatProduction { get; set; }

    public double ResourceConsumption { get; set; }

    [JsonConverter(typeof(BasicResourceConverter))]
    public required IBasicResource Resource { get; set; }

    public double Emissions { get; set; }
}