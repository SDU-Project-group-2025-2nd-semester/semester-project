using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Services.Optimizers;

public interface IOptimizer
{
    public Task OptimizeAsync();
    void ChangeOptimizationSettings(IOptimizerSettings optimizerSettings);


}