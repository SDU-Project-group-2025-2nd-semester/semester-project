﻿using HeatManager.Core.Models.Resources;
using HeatManager.Core.Services;
using System.Text.Json.Serialization;

namespace HeatManager.Core.Models.Producers;

internal class HeatProductionUnit : IHeatProductionUnit
{
    public required string Name { get; set; }

    public decimal Cost { get; set; }

    public double MaxHeatProduction { get; set; }

    public double ResourceConsumption { get; set; }

    [JsonConverter(typeof(BasicResourceConverter))]
    public required IBasicResource Resource { get; set; }

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