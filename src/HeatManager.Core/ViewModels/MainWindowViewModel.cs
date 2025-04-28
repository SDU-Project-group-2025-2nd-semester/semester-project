using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.Core.Views;
// ReSharper disable InconsistentNaming

namespace HeatManager.Core.ViewModels;

public partial class MainWindowViewModel(ISourceDataProvider dataProvider, IOptimizer optimizer) : ViewModelBase
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
    private void SetOverviewView()
    {
        CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
    }
}
