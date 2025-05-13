using HeatManager.Core.Models;
using HeatManager.Core.Models.Producers;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace HeatManager.Core.Services.AssetManagers;

public class AssetManager : IAssetManager
{
    private readonly string DataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Producers", "ProductionUnits.json");

    public ObservableCollection<ProductionUnitBase> ProductionUnits { get; set; } = [];
    // This is just for parity, as the ProjectManager requires it.
    // Created after removing HeatSourceManager
    public ObservableCollection<HeatProductionUnit> HeatProductionUnits =>
        new(ProductionUnits.OfType<HeatProductionUnit>()); 

    public AssetManager()
    {
        LoadUnits(DataFilePath);
    }

    public void LoadUnits(string filepath)
    {
        if (!File.Exists(filepath))
        {
            throw new FileNotFoundException($"Data file not found at {filepath}");
        }

        var json = File.ReadAllText(filepath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new BasicResourceConverter() }
        };

        var jsonData = JsonSerializer.Deserialize<JsonDataStructure>(json, options) ?? new JsonDataStructure();

        var allUnits = (jsonData.HeatProductionUnits ?? Enumerable.Empty<ProductionUnitBase>())
            .Concat(jsonData.ElectricityProductionUnits ?? Enumerable.Empty<ProductionUnitBase>())
            .ToList();

        ProductionUnits = new ObservableCollection<ProductionUnitBase>(allUnits);

    }

    // New method to combine data for the UI (AssetManager view)
    public ObservableCollection<CombinedProductionUnit> GetCombinedUnits()
    {
        var combinedUnits = new ObservableCollection<CombinedProductionUnit>();

        foreach (var unit in ProductionUnits)
        {
            if (ProductionUnitData.Units.AllUnits.TryGetValue(unit.Name, out var isActive))
            {
                var status = isActive ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline;

                combinedUnits.Add(new CombinedProductionUnit
                {
                    Name = unit.Name,
                    IsActive = isActive, 
                    Status = isActive ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline,
                    Cost = unit.Cost,
                    MaxHeatProduction = unit.MaxHeatProduction,
                    Emissions = unit.Emissions,
                    ResourceConsumption = unit.ResourceConsumption,
                    Resource = unit.Resource
                });
            }
        }

        return combinedUnits;
    }
}