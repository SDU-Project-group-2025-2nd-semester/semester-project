using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Models.Projects;
using HeatManager.Core.Services.ProjectManagers;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HeatManager.ViewModels.ProjectManager;

internal partial class ProjectSelectionViewModel : ViewModelBase
{
    public ObservableCollection<ProjectDisplay> Projects { get; } = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isCreatingProject;

    [ObservableProperty] 
    private string _newProjectName;

    [ObservableProperty] 
    private ProjectDisplay? _selectedProject;


    private readonly IProjectManager _projectManager;
    private readonly Window _hostWindow;

    /// <inheritdoc/>
    public ProjectSelectionViewModel(IProjectManager projectManager, Window hostWindow)
    {
        projectManager.GetProjectsFromDatabaseDisplays().ForEach(Projects.Add);

        _projectManager = projectManager;
        _hostWindow = hostWindow;
    }

    [RelayCommand]
    private void StartProjectCreation()
    {
        IsCreatingProject = true;
        NewProjectName = string.Empty;
    }
    

    [RelayCommand]
    private void CancelProjectCreation()
    {
        IsCreatingProject = false;
        NewProjectName = string.Empty;
    }

    [RelayCommand]
    private async Task NewProject()
    {
        await _projectManager.NewProjectAsync(_newProjectName);
        _hostWindow.Close(); // Close dialog after action
    }

    [RelayCommand]
    private async Task OnOpenAsync(ProjectDisplay projectDisplay)
    {
        await _projectManager.LoadProjectFromDb(projectDisplay.Name);
        _hostWindow.Close(); // Close dialog after action
    }
}
