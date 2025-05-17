using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.ViewModels.ConfigPanel;
using HeatManager.ViewModels.DemandPrice;
using HeatManager.ViewModels.Optimizer;
using HeatManager.ViewModels.Overview;
using HeatManager.Views.ConfigPanel;
using HeatManager.Views.DemandPrice;
using HeatManager.Views.Optimizer;
using HeatManager.Views.Overview;

// ReSharper disable InconsistentNaming

namespace HeatManager.ViewModels;

public partial class MainWindowViewModel(ISourceDataProvider dataProvider, IOptimizer optimizer) : ViewModelBase
{   
    [ObservableProperty]
    private UserControl? currentView;
    
    // public MainWindowViewModel() : this(default, default)
    // {
    //     // Set the default view to OverviewView
    //     CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
    // }
 
    private readonly ProductionUnitsViewModel productionUnitsViewModel = new ProductionUnitsViewModel();

    [RelayCommand]
    internal void SetConfigPanelView()
    {
        CurrentView = new AssetManagerView(productionUnitsViewModel);
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
        CurrentView = new OverviewView
        {
            DataContext = new OverviewViewModel(this, productionUnitsViewModel)
        };
    }
}
