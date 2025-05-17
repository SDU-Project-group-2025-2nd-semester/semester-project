using HeatManager.Core.DataLoader;
using HeatManager.Core.Db;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.ResourceManagers;
using HeatManager.Core.Services.SourceDataProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HeatManager.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        #region Services

        services
            .AddSingleton<ISourceDataProvider, SourceDataProvider>()
            .AddSingleton<IOptimizerSettings, OptimizerSettings>()
            .AddSingleton<IOptimizerStrategy, OptimizerStrategy>()
            .AddSingleton<IAssetManager, AssetManager>()
            .AddSingleton<IResourceManager, ResourceManager>()
            .AddSingleton<IOptimizer, DefaultOptimizer>()
            .AddTransient<IDataLoader, CsvDataLoader>()
            .AddSingleton<IProjectManager, ProjectManager>();

        #endregion

        #region Database

        services.AddDbContext<HeatManagerDbContext>(options =>
        {
            options
                .UseNpgsql("Host=localhost;Database=heatManager;Username=postgres;Password=postgres", o =>
                {
                    o.ConfigureDataSource(o => o.EnableDynamicJson());
                })
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        });

        #endregion

        return services;
    }
}