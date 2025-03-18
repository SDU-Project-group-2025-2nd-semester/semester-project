using HeatManager.Core.DataLoader;
using HeatManager.Core.Services;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace HeatManager.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection collection)
    {
        // TODO: Implement basic services

        #region Views

        collection
            .AddSingleton<IAssetManagerViewModel,AssetManagerViewModel>()
            .AddSingleton<IDataOptimizerViewModel, DataOptimizerViewModel>();

        #endregion

        #region Services

        collection
            .AddSingleton<ISourceDataProvider, SourceDataProvider>()
            .AddSingleton<IHeatSourceManager,HeatSourceManager>()
            .AddSingleton<IResourceManager, ResourceManager>()
            .AddSingleton<IOptimizer, DefaultOptimizer>();
            
            

        #endregion
        
        //TODO: Sort where to add this
        #region DontKnowWhereToPutThis
        
        collection.AddTransient<IDataLoader, CsvDataLoader>();
        
        #endregion

        return collection;
    }
}