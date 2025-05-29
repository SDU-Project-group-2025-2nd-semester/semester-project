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

        CurrentView = new Logi { DataContext = new LogiViewModel() };
    }
    
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

    [RelayCommand]
    internal void SetConfigPanelView()
    {
        CurrentView = new AssetManagerView { DataContext = new AssetManagerViewModel(_assetManager, _optimizer, _productionUnitsViewModel) };
    }

    [RelayCommand]
    private void SetOptimizerView()
    {
        CurrentView = new DataOptimizerView { DataContext = new DataOptimizerViewModel(_optimizer) };
    }

    [RelayCommand]
    private void SetGridProductionView()
    {
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
        CurrentView = new OverviewView
        {
            DataContext = new OverviewViewModel(this, _productionUnitsViewModel, _dataProvider)
        };
    }

    [RelayCommand]
    private void ExportData()
    {
        // Use current state of units instead of reloading from JSON
        _optimizer.ChangeOptimizationSettings(new OptimizerSettings
        {
            AllUnits = _assetManager.ProductionUnits.ToDictionary(x => x.Name, _ => true),
        });
        
        Schedule optimizedSchedule = _optimizer.Optimize();
        var exporter = new ScheduleExporter();

        string? dir = AppDomain.CurrentDomain.BaseDirectory;
        while (dir != null && !Directory.Exists(Path.Combine(dir, "results")))
        {
            if (Directory.GetParent(dir) == null) break;
            dir = Directory.GetParent(dir)?.FullName;
        }

        if (dir == null)
            throw new DirectoryNotFoundException("Could not find the 'results' directory in any parent folder.");

        string optimizedHeatProductionPath = Path.Combine(dir, "results", "OptimizedHeatProduction.csv");
        string optimizedElectricityProductionPath = Path.Combine(dir, "results", "OptimizedElectricityProduction.csv");

        exporter.ExportScheduleData(optimizedHeatProductionPath, optimizedSchedule.HeatProductionUnitSchedules);
        exporter.ExportScheduleData(optimizedElectricityProductionPath, optimizedSchedule.ElectricityProductionUnitSchedules);
    }
}