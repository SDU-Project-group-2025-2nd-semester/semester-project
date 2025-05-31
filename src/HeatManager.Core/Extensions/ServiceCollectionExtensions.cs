using HeatManager.Core.DataLoader;
using HeatManager.Core.Db;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.Core.StartupHosts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

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
            //.AddSingleton<IResourceManager, ResourceManager>()
            .AddSingleton<IOptimizer, DefaultOptimizer>()
            .AddSingleton<IDataLoader, CsvDataLoader>()
            .AddSingleton<IProjectManager, ProjectManager>();

        services.AddScoped<StartupHost>();

        #endregion

        #region Database

        services.AddDbContext<HeatManagerDbContext>(options =>
        {
            options
                .UseNpgsql("Host=localhost;Database=heatManager;Username=postgres;Password=postgres", o =>
                {
                    o.ConfigureDataSource(o =>
                    {
                        o.EnableDynamicJson();
                        o.ConfigureJsonOptions(new JsonSerializerOptions()
                        {
                            IncludeFields = true,
                        });
                    });
                })
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        });

        #endregion

        return services;
    }
}