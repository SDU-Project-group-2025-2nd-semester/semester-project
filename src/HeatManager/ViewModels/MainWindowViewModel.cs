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

// ReSharper disable InconsistentNaming

namespace HeatManager.ViewModels;


public partial class MainWindowViewModel(IAssetManager assetManager,ISourceDataProvider dataProvider, IOptimizer optimizer, IProjectManager projectManager, IDataLoader dataLoader, Window window, IServiceProvider serviceProvider) : ViewModelBase
{

    [ObservableProperty]
    private UserControl? currentView;

    [ObservableProperty]
    private bool isPaneOpen; //= true; // Default to open

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

        CurrentView = new AssetManagerView { DataContext = new AssetManagerViewModel(assetManager, optimizer) };

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

    [RelayCommand]
    private void ExportData()
    {

        assetManager.LoadUnits(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Producers", "ProductionUnits.json"));

        optimizer.ChangeOptimizationSettings(new OptimizerSettings
        {
            AllUnits = assetManager.ProductionUnits.ToDictionary(x => x.Name, _ => true),
        });
        Schedule optimizedSchedule = optimizer.Optimize();
        ScheduleExporter exporter = new ScheduleExporter();

        string? dir = AppDomain.CurrentDomain.BaseDirectory;

        while (dir != null && !Directory.Exists(Path.Combine(dir, "results")))
        {
            if (Directory.GetParent(dir) == null) break;
            dir = Directory.GetParent(dir)?.FullName;
        }

        if (dir == null)
            throw new DirectoryNotFoundException("Could not find the 'results' directory in any parent folder.");

        string OptimizedHeatProductionPath = Path.Combine(dir, "results", "OptimizedHeatProduction.csv");

        string OptimizedElectricityProductionPath = Path.Combine(dir, "results", "OptimizedElectricityProduction.csv");

        exporter.ExportScheduleData(OptimizedHeatProductionPath, optimizedSchedule.HeatProductionUnitSchedules);
        exporter.ExportScheduleData(OptimizedElectricityProductionPath, optimizedSchedule.ElectricityProductionUnitSchedules);
    }
}
