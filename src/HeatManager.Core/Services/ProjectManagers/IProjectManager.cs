using HeatManager.Core.Models.Projects;

namespace HeatManager.Core.Services.ProjectManagers;

public interface IProjectManager
{
    public Project? CurrentProject { get; }

    public Task NewProjectAsync(string name);

    public Task<List<ProjectDisplay>> GetProjectsFromDatabaseDisplaysAsync();

    public Task LoadProjectFromDb(string projectName);

    public Task SaveProjectAsync();
}