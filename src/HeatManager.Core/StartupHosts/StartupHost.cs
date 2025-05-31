using HeatManager.Core.DataLoader;
using HeatManager.Core.Db;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Projects;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.SourceDataProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HeatManager.Core.StartupHosts;

public class StartupHost(IServiceProvider serviceProvider)
{
    public void Execute(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();

        var applicationDbContext = scope.ServiceProvider.GetRequiredService<HeatManagerDbContext>();

        Console.WriteLine("Migrating to database.");

        applicationDbContext.Database.Migrate();

        MockDatabase(applicationDbContext);
    }

    private void MockDatabase(HeatManagerDbContext dbContext)
    {
        var projectSummerScenario1 = dbContext.Projects.FirstOrDefault(p => p.Name == "Summer Scenario 1");

        if (projectSummerScenario1 is null)
        {
            Console.WriteLine("Creating mock project 'Summer Scenario 1'");
            var project = new Project
            {
                Name = "Summer Scenario 1",
                LastOpened = DateTime.UtcNow,
                ProjectData =
                {
                    SourceData = GetDataForSummer(),
                    ProductionUnits = GetHeatProductionUnitsScenario1()
                }
            };

            dbContext.Projects.Add(project);
            dbContext.SaveChanges();
        }

        var projectSummerScenario2 = dbContext.Projects.FirstOrDefault(p => p.Name == "Summer Scenario 2");

        if (projectSummerScenario2 is null)
        {
            Console.WriteLine("Creating mock project 'Summer Scenario 2'");
            var project = new Project
            {
                Name = "Summer Scenario 2",
                LastOpened = DateTime.UtcNow,
                ProjectData =
                {
                    SourceData = GetDataForSummer(),
                    ProductionUnits = GetHeatProductionUnitsScenario2()
                }
            };
            dbContext.Projects.Add(project);
            dbContext.SaveChanges();
        }

        var projectWinterScenario1 = dbContext.Projects.FirstOrDefault(p => p.Name == "Winter Scenario 1");

        if (projectWinterScenario1 is null)
        {
            Console.WriteLine("Creating mock project 'Winter Scenario 1'");
            var project = new Project
            {
                Name = "Winter Scenario 1",
                LastOpened = DateTime.UtcNow,
                ProjectData =
                {
                    SourceData = GetDataForWinter(),
                    ProductionUnits = GetHeatProductionUnitsScenario1()
                }
            };
            dbContext.Projects.Add(project);
            dbContext.SaveChanges();
        }

        var projectWinterScenario2 = dbContext.Projects.FirstOrDefault(p => p.Name == "Winter Scenario 2");

        if (projectWinterScenario2 is null)
        {
            Console.WriteLine("Creating mock project 'Winter Scenario 2'");
            var project = new Project
            {
                Name = "Winter Scenario 2",
                LastOpened = DateTime.UtcNow,
                ProjectData =
                {
                    SourceData = GetDataForWinter(),
                    ProductionUnits = GetHeatProductionUnitsScenario2()
                }
            };
            dbContext.Projects.Add(project);
            dbContext.SaveChanges();
        }
    }

    private SourceDataCollection GetDataForSummer()
    {
        return GetData("./source-data-csv/summer.csv");
    }

    private SourceDataCollection GetDataForWinter()
    {
        return GetData("./source-data-csv/winter.csv");
    }

    private List<ProductionUnitBase> GetHeatProductionUnitsScenario1() => 
        GetHeatProductionUnitsScenario2()
        .Where( u => u.Name switch
        {
            "HP1" or "GM1" => false,
            _ => true
        }).ToList();

    private List<ProductionUnitBase> GetHeatProductionUnitsScenario2()
    {
        var assetManager = new AssetManager();

        assetManager.LoadUnits(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Producers",
            "ProductionUnits.json"));

        return assetManager.ProductionUnits.ToList();
    }

    private SourceDataCollection GetData(string path)
    {
        var sourceDataProvider = new SourceDataProvider();
        var csvDataLoader = new CsvDataLoader(sourceDataProvider);

        csvDataLoader.LoadData(path);

        // ReSharper disable once NullableWarningSuppressionIsUsed
        return sourceDataProvider.SourceDataCollection!;
    }
}