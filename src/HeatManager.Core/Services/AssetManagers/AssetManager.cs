using HeatManager.Core.Models.Producers;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace HeatManager.Core.Services.AssetManagers;

/// <summary>
/// Manages the collection of production units used in the application,
/// including loading from file, adding, and removing units.
/// </summary>
public class AssetManager : IAssetManager
{
    /// <summary>
    /// Path to the JSON file containing production unit data.
    /// </summary>
    private readonly string DataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Producers", "ProductionUnits.json");

    /// <summary>
    /// Collection of all production units (heat and electricity).
    /// </summary>
    public ObservableCollection<ProductionUnitBase> ProductionUnits { get; set; } = [];

    /// <summary>
    /// Subset of ProductionUnits that includes only HeatProductionUnits.
    /// Used for parity with the ProjectManager.
    /// </summary>
    public ObservableCollection<HeatProductionUnit> HeatProductionUnits =>
        new(ProductionUnits.OfType<HeatProductionUnit>());

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetManager"/> class and loads production units from file.
    /// </summary>
    public AssetManager()
    {
        LoadUnits(DataFilePath);
    }

    /// <summary>
    /// Loads production units from a JSON file at the specified path.
    /// </summary>
    /// <param name="filepath">The full path to the JSON file.</param>
    /// <exception cref="FileNotFoundException">Thrown if the file is not found at the specified path.</exception>
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

    /// <summary>
    /// Adds a production unit to the collection.
    /// </summary>
    /// <param name="unit">The production unit to add.</param>
    public void AddUnit(ProductionUnitBase unit)
    {
        ProductionUnits.Add(unit);
    }

    /// <summary>
    /// Removes a production unit from the collection.
    /// </summary>
    /// <param name="unit">The production unit to remove.</param>
    public void RemoveUnit(ProductionUnitBase unit)
    {
        ProductionUnits.Remove(unit);
    }
}