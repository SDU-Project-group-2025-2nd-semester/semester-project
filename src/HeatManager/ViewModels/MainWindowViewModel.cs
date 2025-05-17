using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.DataLoader;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.ViewModels.ConfigPanel;
using HeatManager.ViewModels.DemandPrice;
using HeatManager.ViewModels.Optimizer;
using HeatManager.ViewModels.Overview;
using HeatManager.ViewModels.ProjectManager;
using HeatManager.Views.ConfigPanel;
using HeatManager.Views.DemandPrice;
using HeatManager.Views.Optimizer;
using HeatManager.Views.Overview;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace HeatManager.ViewModels;

public partial class MainWindowViewModel(ISourceDataProvider dataProvider, IOptimizer optimizer, IProjectManager projectManager, IDataLoader dataLoader, Window window, IServiceProvider serviceProvider) : ViewModelBase
{
    [ObservableProperty]
    private UserControl? currentView;

    // public MainWindowViewModel() : this(default, default)
    // {
    //     // Set the default view to OverviewView
    //     CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
    // }

    [RelayCommand]
    private async Task SaveProject()
    {
        await projectManager.SaveProjectAsync();
    }

    [RelayCommand]
    internal void SetConfigPanelView()
    {
        CurrentView = new AssetManagerView { DataContext = new AssetManagerViewModel() };
    }

    [RelayCommand]
    private void SetOptimizerView()
    {
        CurrentView = new DataOptimizerView { DataContext = new DataOptimizerViewModel(optimizer) };
    }

    [RelayCommand]
    private void SetGridProductionView()
    {
        CurrentView = new GridProductionView { DataContext = new GridProductionViewModel(dataProvider) };
    }

    [RelayCommand]
    private async Task OpenProjectManagerWindow()
    {
        var dialog = ActivatorUtilities.CreateInstance<ProjectSelectionWindow>(serviceProvider);

        dialog.DataContext = ActivatorUtilities.CreateInstance<ProjectSelectionViewModel>(serviceProvider, dialog);

        await dialog.ShowDialog(window);
    }

    [RelayCommand]
    private void SetOverviewView()
    {
        CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
    }
}
