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

    /// <summary>
    /// Changes the optimization settings
    /// </summary>
    /// <param name="optimizerSettings">The new optimization settings</param>
    public void ChangeOptimizationSettings(IOptimizerSettings optimizerSettings);

    /// <summary>
    /// Updates the production units in the optimizer.
    /// </summary>
    /// <param name="assetManager"></param>
    public void UpdateProductionUnits(IAssetManager assetManager);
}