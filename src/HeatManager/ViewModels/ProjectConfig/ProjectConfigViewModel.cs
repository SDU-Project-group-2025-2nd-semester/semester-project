using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.ViewModels.ProjectManager;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HeatManager.ViewModels.ProjectConfig;

public partial class ProjectConfigViewModel(Window window, IServiceProvider serviceProvider, IProjectManager projectManager) : ViewModelBase
{    
    [RelayCommand]
    private async Task OpenProjectManagerWindow()
    {

        var dialog = ActivatorUtilities.CreateInstance<ProjectSelectionWindow>(serviceProvider);

        dialog.DataContext = ActivatorUtilities.CreateInstance<ProjectSelectionViewModel>(serviceProvider, dialog);

        await dialog.ShowDialog(window);

    }
    
    [RelayCommand]
    private async Task SaveProject()
    {
        try
        {
            Console.WriteLine("Saving project...");
            await projectManager.SaveProjectAsync();
            Console.WriteLine("Project saved successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving project: {ex.Message}");
            throw;
        }
    }
}