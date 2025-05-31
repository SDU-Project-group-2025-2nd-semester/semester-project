using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services.AssetManagers;

namespace HeatManager.Core.Services.Optimizers;

public interface IOptimizer
{
    /// <summary>
    /// Performs the optimization process
    /// </summary>
    public Schedule Optimize();
    public IOptimizerSettings OptimizerSettings { get; }

    /// <summary>
    /// Changes the optimization settings
    /// </summary>
    /// <param name="optimizerSettings">The new optimization settings</param>
    public void ChangeOptimizationSettings(IOptimizerSettings optimizerSettings);

    /// <summary>
    /// Updates the optimizer with the latest active/offline states of the production units
    /// </summary>
    /// <param name="units">A dictionary containing the names of the units and their states.</param>
    void UpdateUnits(Dictionary<string, bool> units);

    /// <summary>
    /// Gets the available production units based on the current settings
    /// Updates the production units in the optimizer.
    /// </summary>
    /// <param name="assetManager"></param>
    public void UpdateProductionUnits(IAssetManager assetManager);
}