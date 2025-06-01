using HeatManager.Core.Models.Producers;
using System.Collections.ObjectModel;

namespace HeatManager.Core.Services.AssetManagers;

/// <summary>
/// Interface for managing production units in the application.
/// </summary>
public interface IAssetManager
{
    /// <summary>
    /// Collection of all production units (heat and electricity).
    /// </summary>
    ObservableCollection<ProductionUnitBase> ProductionUnits { get; set; }

    /// <summary>
    /// Subset of ProductionUnits that includes only HeatProductionUnits.
    /// Used for parity with the ProjectManager.
    /// </summary>
    ObservableCollection<HeatProductionUnit> HeatProductionUnits { get; }

    /// <summary>
    /// Loads production units from a JSON file at the specified path.
    /// </summary>
    /// <param name="filepath">The full path to the JSON file.</param>
    void LoadUnits(string filepath);

    /// <summary>
    /// Adds a production unit to the collection.
    /// </summary>
    /// <param name="unit">The production unit to add.</param>
    void AddUnit(ProductionUnitBase unit);

    /// <summary>
    /// Removes a production unit from the collection.
    /// </summary>
    /// <param name="unit">The production unit to remove.</param>
    void RemoveUnit(ProductionUnitBase unit);
}