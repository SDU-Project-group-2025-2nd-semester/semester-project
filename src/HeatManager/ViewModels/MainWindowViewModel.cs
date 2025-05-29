using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.DataLoader;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.ScheduleExporter;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.Core.Services.AssetManagers;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HeatManager.Views.DataExporter;
using HeatManager.ViewModels.DataExporter;

// ReSharper disable InconsistentNaming

namespace HeatManager.ViewModels;


public partial class MainWindowViewModel(IAssetManager assetManager, ISourceDataProvider dataProvider, IOptimizer optimizer, IProjectManager projectManager, IDataLoader dataLoader, Window window, IServiceProvider serviceProvider) : ViewModelBase
{

    public enum ViewType
    {
        Overview,
        ConfigPanel,
        Optimizer,
        GridProduction,
        DataExport,
        ProjectManager,
        SaveProject
    }

    [ObservableProperty]
    private ViewType currentViewType;

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
        CurrentViewType = ViewType.ConfigPanel;
        CurrentView = new AssetManagerView { DataContext = new AssetManagerViewModel(assetManager, optimizer) };
    }

    [RelayCommand]
    private void SetOptimizerView()
    {
        CurrentViewType = ViewType.Optimizer;
        CurrentView = new DataOptimizerView { DataContext = new DataOptimizerViewModel(optimizer) };
    }

    [RelayCommand]
    private void SetGridProductionView()
    {
        CurrentViewType = ViewType.GridProduction;
        CurrentView = new GridProductionView { DataContext = new GridProductionViewModel(dataProvider) };
    }

    [RelayCommand]
    private async Task OpenProjectManagerWindow()
    {
        CurrentViewType = ViewType.ProjectManager;

        var dialog = ActivatorUtilities.CreateInstance<ProjectSelectionWindow>(serviceProvider);

        dialog.DataContext = ActivatorUtilities.CreateInstance<ProjectSelectionViewModel>(serviceProvider, dialog);

        await dialog.ShowDialog(window);

        CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
        CurrentViewType = ViewType.Overview;
    }

    [RelayCommand]
    private void SetOverviewView()
    {
        CurrentViewType = ViewType.Overview;
        CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
    }

    [RelayCommand]
    private void SetDataExportView()
    {
        CurrentViewType = ViewType.DataExport;
        CurrentView = new DataExportView { DataContext = new DataExportViewModel(assetManager, optimizer, projectManager) };
    }
    

    
}
