using HeatManager.Core.Models.Producers;

namespace HeatManager.Core.Services.AssetManagers;

public class JsonDataStructure
{
    public List<HeatProductionUnit>? HeatProductionUnits { get; set; }
    public List<ElectricityProductionUnit>? ElectricityProductionUnits { get; set; }
}