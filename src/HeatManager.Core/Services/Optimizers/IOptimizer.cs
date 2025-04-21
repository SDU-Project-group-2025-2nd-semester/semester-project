using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services.AssetManagers;

namespace HeatManager.Core.Services.Optimizers;

public interface IOptimizer
{
    /// <summary>
    /// Performs the optimization process
    /// </summary>
    public void Optimize();

    /// <summary>
    /// Changes the optimization settings
    /// </summary>
    /// <param name="optimizerSettings">The new optimization settings</param>
    void ChangeOptimizationSettings(IOptimizerSettings optimizerSettings);

    /// <summary>
    /// Gets the available production units based on the current settings
    /// </summary>
    /// <param name="assetManager">The asset manager containing the units</param>
    /// <param name="settings">The current optimization settings</param>
    /// <returns>A list of available heat production units</returns>
    List<ProductionUnitBase> GetAvailableUnits(IAssetManager assetManager, IOptimizerSettings settings);

    /// <summary>
    /// Generates schedules for heat production units
    /// </summary>
    /// <param name="heatSources">The available heat production units</param>
    /// <returns>A list of heat production unit schedules</returns>
    List<HeatProductionUnitSchedule> GenerateHeatProductionUnitSchedules(IEnumerable<ProductionUnitBase> heatSources);

    /// <summary>
    /// Generates schedules for electricity production units
    /// </summary>
    /// <param name="electricitySources">The available electricity production units</param>
    /// <returns>A list of electricity production unit schedules</returns>
    List<ElectricityProductionUnitSchedule> GenerateElectricityProductionUnitSchedules(IEnumerable<ElectricityProductionUnit> electricitySources);
}