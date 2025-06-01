using HeatManager.Core.Db;
using HeatManager.Core.Models.Projects;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.SourceDataProviders;
using Microsoft.EntityFrameworkCore;

namespace HeatManager.Core.Services.ProjectManagers;

public class ProjectManager(
    HeatManagerDbContext dbContext, 
    IAssetManager assetManager, 
    //IResourceManager resourceManager, 
    ISourceDataProvider sourceDataProvider,
    IOptimizer optimizer) : IProjectManager
{
    public Project? CurrentProject { get; private set; }

    public async Task SaveProjectAsync()
    {
        if (CurrentProject is null)
        {
            throw new InvalidOperationException("Current project is null. Cannot save.");
        }

        CurrentProject.LastOpened = DateTime.UtcNow;

        var projectData = CurrentProject.ProjectData;

        // Clear existing units and add all current units
        projectData.ProductionUnits.Clear();
        foreach (var unit in assetManager.ProductionUnits)
        {
            var clonedUnit = unit.Clone();
            projectData.ProductionUnits.Add(clonedUnit);
        }

        Console.WriteLine($"Saving {projectData.ProductionUnits.Count} production units to project");
        foreach (var unit in projectData.ProductionUnits)
        {
            Console.WriteLine($"  - Unit: {unit.Name}, Status: {unit.IsActive}");
        }

        //projectData.Resources = resourceManager.Resources.ToList();
        //Console.WriteLine($"Saving {projectData.Resources.Count} resources to project");

        projectData.SourceData = sourceDataProvider.SourceDataCollection;

        var project = await dbContext.Projects.FindAsync(CurrentProject.Id);

        if (project is null)
        {
            dbContext.Projects.Add(CurrentProject);
            Console.WriteLine("Creating new project in database");
        }
        else
        {
            // Update the existing project's data
            project.LastOpened = CurrentProject.LastOpened;
            dbContext.Projects.Update(project);
            Console.WriteLine("Updating existing project in database");
        }

        await dbContext.SaveChangesAsync();
        Console.WriteLine("Project saved successfully");
    }

    public async Task NewProjectAsync(string name)
    {
        Console.WriteLine($"Creating new project: {name}");
        CurrentProject = new Project { Name = name };

        await LoadAsync();

        assetManager.LoadUnits(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Producers", "ProductionUnits.json"));
        Console.WriteLine($"Loaded {assetManager.ProductionUnits.Count} production units");
        foreach (var unit in assetManager.ProductionUnits)
        {
            Console.WriteLine($"  - Unit: {unit.Name}, Status: {unit.IsActive}");
        }

        optimizer.ChangeOptimizationSettings(new OptimizerSettings
        {
            AllUnits = assetManager.ProductionUnits.ToDictionary(x => x.Name, x => x.IsActive),
        });
        Console.WriteLine("Updated optimizer settings with loaded units");
    }

    public List<ProjectDisplay> GetProjectsFromDatabaseDisplays()
    {
        return  (from projects in dbContext.Projects
            orderby projects.LastOpened descending
            select new ProjectDisplay
            {
                Name = projects.Name,
                CreatedAt = projects.CreatedAt,
                LastOpened = projects.LastOpened
            }
        ).ToList();
    }

    public async Task LoadProjectFromDb(string projectName)
    {
        Console.WriteLine($"Loading project from database: {projectName}");
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Name == projectName);

        CurrentProject = project ?? throw new ArgumentException($"Project with name: \"{projectName}\" not found!");

        await LoadAsync();
        Console.WriteLine("Project loaded successfully");
    }

    public async Task<List<ProjectDisplay>> GetProjectsFromDatabaseDisplaysAsync()
    {
        return await (from projects in dbContext.Projects
                orderby projects.LastOpened descending
                select new ProjectDisplay
                {
                    Name = projects.Name,
                    CreatedAt = projects.CreatedAt,
                    LastOpened = projects.LastOpened
                }
            ).ToListAsync();
    }

    private Task LoadAsync()
    {
        Console.WriteLine("Starting to load project data");
        assetManager.ProductionUnits.Clear();
        //resourceManager.Resources.Clear();

        var projectData = CurrentProject?.ProjectData ?? throw new InvalidOperationException("Project needs to be retrieved from db before it's loading.");

        projectData.ProductionUnits.ForEach(assetManager.ProductionUnits.Add);
        Console.WriteLine($"Loaded {projectData.ProductionUnits.Count} production units from project data");
        foreach (var unit in projectData.ProductionUnits)
        {
            Console.WriteLine($"  - Unit: {unit.Name}, Status: {unit.IsActive}");
        }

        //projectData.Resources.ForEach(resourceManager.Resources.Add);
        //Console.WriteLine($"Loaded {projectData.Resources.Count} resources from project data");

        sourceDataProvider.SourceDataCollection = projectData.SourceData;
        Console.WriteLine("Loaded source data collection");

        optimizer.ChangeOptimizationSettings(new OptimizerSettings
        {
            AllUnits = assetManager.ProductionUnits.ToDictionary(x => x.Name, x => x.IsActive),
        });
        Console.WriteLine("Updated optimizer settings with loaded units");

        return Task.CompletedTask;
    }
}