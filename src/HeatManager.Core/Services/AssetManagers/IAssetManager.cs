using HeatManager.Core.Models.Producers;
using System.Collections.ObjectModel;

namespace HeatManager.Core.Services.AssetManagers;

/// <summary>
/// Defines functionality for managing asset units in the system,
/// including loading, adding, and removing production units.
/// </summary>
public interface IAssetManager
{
    /// <summary>
    /// Gets the collection of all production units.
    /// </summary>
    public ObservableCollection<ProductionUnitBase> ProductionUnits { get; }

    /// <summary>
    /// Gets the collection of heat production units.
    /// </summary>
    public ObservableCollection<HeatProductionUnit> HeatProductionUnits { get; }

    /// <summary>
    /// Loads production units from a specified file.
    /// </summary>
    /// <param name="filepath">The path to the file containing unit data.</param>
    public void LoadUnits(string filepath);

    /// <summary>
    /// Adds a new production unit to the collection.
    /// </summary>
    /// <param name="unit">The production unit to add.</param>
    public void AddUnit(ProductionUnitBase unit);

    /// <summary>
    /// Removes an existing production unit from the collection.
    /// </summary>
    /// <param name="unit">The production unit to remove.</param>
    public void RemoveUnit(ProductionUnitBase unit);
}
