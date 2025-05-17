using HeatManager.Core.Db;
using HeatManager.Core.Models.Projects;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.ResourceManagers;
using HeatManager.Core.Services.SourceDataProviders;
using Microsoft.EntityFrameworkCore;

namespace HeatManager.Core.Services.ProjectManagers;

public class ProjectManager(
    HeatManagerDbContext dbContext, 
    IAssetManager assetManager, 
    IResourceManager resourceManager, 
    ISourceDataProvider sourceDataProvider) : IProjectManager
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

        //projectData.HeatProductionUnits = assetManager.HeatProductionUnits.ToList();

        projectData.ProductionUnits = assetManager.ProductionUnits.ToList();

        projectData.Resources = resourceManager.Resources.ToList();

        projectData.SourceData = sourceDataProvider.SourceDataCollection;

        var project = await dbContext.Projects.FindAsync(CurrentProject.Id);

        if (project is null)
        {
            dbContext.Projects.Add(CurrentProject);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task NewProjectAsync(string name)
    {
        CurrentProject = new Project { Name = name };

        await LoadAsync();
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
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Name == projectName);

        CurrentProject = project ?? throw new ArgumentException($"Project with name: \"{projectName}\" not found!");

        await LoadAsync();
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
        assetManager.ProductionUnits.Clear();
        resourceManager.Resources.Clear();

        var projectData = CurrentProject?.ProjectData ?? throw new InvalidOperationException("Project needs to be retrieved from db before it's loading.");

        projectData.ProductionUnits.ForEach(assetManager.ProductionUnits.Add);
        projectData.Resources.ForEach(resourceManager.Resources.Add);

        sourceDataProvider.SourceDataCollection = projectData.SourceData;

        return Task.CompletedTask;
    }
}