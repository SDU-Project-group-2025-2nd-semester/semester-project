using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services;

namespace HeatManager.Core.Services.Optimizers;

// 1. Let user choose from where to load data
// 2. Let user set-up heat sources
// 3. we have CSV entries and Heat sources with their resources and electricity production
// 4. call optimizer to create optimal schedule
// 5. Display data to the user
// 6. Export data

// *1 User may change the heat sources and their resources any time, then the schedule should be re-optimized
// *2 User may change the which units should run before optimization 

internal class DefaultOptimizer : IOptimizer
{
    private readonly IHeatSourceManager _heatSourceManager;
    private readonly IResourceManager _resourceManager;
    private readonly ISourceDataProvider _sourceDataProvider;

    public DefaultOptimizer(IHeatSourceManager heatSourceManager, IResourceManager resourceManager, ISourceDataProvider sourceDataProvider)
    {
        _heatSourceManager = heatSourceManager; // TODO: Get all the necessary data from services
        _resourceManager = resourceManager;
        _sourceDataProvider = sourceDataProvider;
    }

    public async Task OptimizeAsync()
    {
        await Task.Run(() => // To offload it to a background thread, TODO: Probably put to separate method
        {
            // Probably load some settings here, or get them as parameters

            // Make heat source priority list

            // Iterate over each entry in the schedule

            // Create heat schedule 

            new Schedule();
        });
    }
}