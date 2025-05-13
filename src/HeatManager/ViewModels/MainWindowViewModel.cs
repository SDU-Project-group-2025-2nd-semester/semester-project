using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.ViewModels.ConfigPanel;
using HeatManager.ViewModels.DemandPrice;
using HeatManager.ViewModels.Optimizer;
using HeatManager.ViewModels.Overview;
using HeatManager.ViewModels.ProjectSettings;
using HeatManager.Views.ConfigPanel;
using HeatManager.Views.DemandPrice;
using HeatManager.Views.Optimizer;
using HeatManager.Views.Overview;
using HeatManager.Views.ProjectSettings;

// ReSharper disable InconsistentNaming

namespace HeatManager.ViewModels;

public partial class MainWindowViewModel(ISourceDataProvider dataProvider, IOptimizer optimizer, IAssetManager assetManager) : ViewModelBase
{   
    [ObservableProperty]
    private UserControl? currentView;
    
    // public MainWindowViewModel() : this(default, default)
    // {
    //     // Set the default view to OverviewView
    //     CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
    // }


    [RelayCommand]
    internal void SetConfigPanelView()
    {
        CurrentView = new AssetManagerView { DataContext = new AssetManagerViewModel(assetManager) };
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
    private void SetOverviewView()
    {
        CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
    }

    [RelayCommand]
    private void SetSettingsView()
    {
        CurrentView = new SettingsView { DataContext = new SettingsViewModel() };
    }
}
