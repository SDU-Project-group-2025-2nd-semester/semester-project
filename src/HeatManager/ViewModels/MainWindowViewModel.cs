using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.DataLoader;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.ScheduleExporter;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.ViewModels.ConfigPanel;
using HeatManager.ViewModels.DataExporter;
using HeatManager.ViewModels.DemandPrice;
using HeatManager.ViewModels.Optimizer;
using HeatManager.ViewModels.Overview;
using HeatManager.ViewModels.ProjectConfig;
using HeatManager.ViewModels.ProjectManager;
using HeatManager.Views.ConfigPanel;
using HeatManager.Views.DataExporter;
using HeatManager.Views.DemandPrice;
using HeatManager.Views.Optimizer;
using HeatManager.Views.Overview;
using HeatManager.Views.ProjectConfig;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace HeatManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IAssetManager _assetManager;
    private readonly ISourceDataProvider _dataProvider;
    private readonly IOptimizer _optimizer;
    private readonly IProjectManager _projectManager;
    private readonly IDataLoader _dataLoader;
    private readonly Window _window;
    private readonly IServiceProvider _serviceProvider;
    private readonly ProductionUnitsViewModel _productionUnitsViewModel;

    public IOptimizer Optimizer { get => _optimizer; }

    [ObservableProperty]
    private UserControl? currentView;

    public MainWindowViewModel(
        IAssetManager assetManager,
        ISourceDataProvider dataProvider,
        IOptimizer optimizer,
        IProjectManager projectManager,
        IDataLoader dataLoader,
        Window window,
        IServiceProvider serviceProvider)
    {
        _assetManager = assetManager;
        _dataProvider = dataProvider;
        _optimizer = optimizer;
        _projectManager = projectManager;
        _dataLoader = dataLoader;
        _window = window;
        _serviceProvider = serviceProvider;

        // Initialize optimizer with empty settings
        _optimizer.ChangeOptimizationSettings(new OptimizerSettings());
        _productionUnitsViewModel = new ProductionUnitsViewModel(_assetManager);
    }

    // public MainWindowViewModel() : this(default, default)
    // {
    //     // Set the default view to OverviewView
    //     CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
    // }

    [ObservableProperty]
    private bool isPaneOpen;

    [RelayCommand]
    private async Task SaveProject()
    {
        try
        {
            Console.WriteLine("Saving project...");
            await _projectManager.SaveProjectAsync();
            Console.WriteLine("Project saved successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving project: {ex.Message}");
            throw;
        }
    }

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


    [RelayCommand]
    internal void SetConfigPanelView()
    {
        CurrentViewType = ViewType.ConfigPanel;
        CurrentView = new AssetManagerView { DataContext = new AssetManagerViewModel(_assetManager, _optimizer, _productionUnitsViewModel) };
    }

    [RelayCommand]
    private void SetOptimizerView()
    {
        CurrentViewType = ViewType.Optimizer;
        CurrentView = new DataOptimizerView { DataContext = new DataOptimizerViewModel(_optimizer) };
    }

    [RelayCommand]
    private void SetGridProductionView()
    {
        CurrentViewType = ViewType.GridProduction;
        CurrentView = new GridProductionView { DataContext = new GridProductionViewModel(_dataProvider) };
    }

    [RelayCommand]
    private async Task OpenProjectManagerWindow()
    {

        var dialog = ActivatorUtilities.CreateInstance<ProjectSelectionWindow>(_serviceProvider);

        dialog.DataContext = ActivatorUtilities.CreateInstance<ProjectSelectionViewModel>(_serviceProvider, dialog);

        await dialog.ShowDialog(_window);

        
    }

    [RelayCommand]
    private void SetOverviewView()
    {
        CurrentViewType = ViewType.Overview;
        CurrentView = new OverviewView { DataContext = new OverviewViewModel(this, _productionUnitsViewModel) };
    }

    [RelayCommand]
    private void SetDataExportView()
    {
        CurrentViewType = ViewType.DataExport;
        CurrentView = new DataExportView { DataContext = new DataExportViewModel(_assetManager, _optimizer, _projectManager) };
    }

    [RelayCommand]
    private void OpenProjectConfigView()
    {
        CurrentViewType = ViewType.ProjectManager;
        CurrentView = new ProjectConfigView { DataContext = new ProjectConfigViewModel(_window, _serviceProvider, _projectManager) };
    }
}
