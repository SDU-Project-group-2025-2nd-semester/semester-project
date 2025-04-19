using HeatManager.Core.Models.Producers;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace HeatManager.Core.Services.AssetManagers;

internal class AssetManager : IAssetManager
{
    private readonly string DataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Producers", "ProductionUnits.json");

    public ObservableCollection<HeatProductionUnitBase> ProductionUnits { get; set; } = [];

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

        var allUnits = (jsonData.HeatProductionUnits ?? Enumerable.Empty<HeatProductionUnitBase>())
            .Concat(jsonData.ElectricityProductionUnits ?? Enumerable.Empty<HeatProductionUnitBase>())
            .ToList();

        ProductionUnits = new ObservableCollection<HeatProductionUnitBase>(allUnits);

    }
}