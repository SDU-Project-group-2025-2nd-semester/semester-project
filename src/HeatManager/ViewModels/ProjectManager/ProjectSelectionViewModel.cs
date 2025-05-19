using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.DataLoader;
using HeatManager.Core.Models.Projects;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.SourceDataProviders;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
    private string _newProjectName = string.Empty;

    [ObservableProperty]
    private ProjectDisplay? _selectedProject;

    [ObservableProperty]
    private bool _isDataImported;

    private readonly IProjectManager _projectManager;
    private readonly Window _hostWindow;
    private readonly IDataLoader _dataLoader;
    private readonly ISourceDataProvider _sourceDataProvider;

    /// <inheritdoc/>
    public ProjectSelectionViewModel(IProjectManager projectManager, Window hostWindow, IDataLoader dataLoader, ISourceDataProvider sourceDataProvider)
    {
        try
        {
            projectManager.GetProjectsFromDatabaseDisplays().ForEach(Projects.Add);
        }
        catch (InvalidOperationException) { } // This is in case the database is not created yet

        _isDataImported = true;
        _projectManager = projectManager;
        _hostWindow = hostWindow;
        _dataLoader = dataLoader;
        _sourceDataProvider = sourceDataProvider;
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
        await _projectManager.NewProjectAsync(NewProjectName);
        IsDataImported = false;
    }

    [RelayCommand]
    private async Task OnOpenAsync(ProjectDisplay projectDisplay)
    {
        await _projectManager.LoadProjectFromDb(projectDisplay.Name);
        _hostWindow.Close(); // Close dialog after action
    }

    [RelayCommand]
    private async Task GetFile()
    {
        var storage = _hostWindow.StorageProvider;

        var file = await storage.OpenFilePickerAsync(new()
        {
            Title = "Select .csv file with source data",
            SuggestedFileName = "source-data.csv",
            FileTypeFilter = [Csv],
            AllowMultiple = false,
        });


        var filePath = file.FirstOrDefault();

        if (filePath is null)
        {
            return;
        }

        await _dataLoader.LoadData(filePath);

        IsDataImported = _sourceDataProvider.SourceDataCollection?.DataPoints is not null;

        if (IsDataImported)
        {
            _hostWindow.Close(); // Close dialog after action
        }

    }

    private static FilePickerFileType Csv { get; } = new("Csv")
    {
        Patterns = ["*.csv"],
    };
}
