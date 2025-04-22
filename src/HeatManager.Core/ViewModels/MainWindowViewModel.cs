using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Views;

namespace HeatManager.Core.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{   
    [ObservableProperty]
    private UserControl ?currentView = new AssetManagerView { DataContext = new AssetManagerViewModel() };

    [RelayCommand]
    internal void SetConfigPanelView()
    {
        CurrentView = new AssetManagerView { DataContext = new AssetManagerViewModel() };
    }
    
    [RelayCommand]
    private void SetOptimizerView()
    {
        CurrentView = new DataOptimizerView { DataContext = new DataOptimizerViewModel() };
    }

    [RelayCommand]
    private void SetGridProductionView()
    {
        CurrentView = new GridProductionView { DataContext = new GridProductionViewModel() };
    }

    [RelayCommand]
    private void SetOverviewView()
    {
        CurrentView = new OverviewView { DataContext = new OverviewViewModel(this) };
    }
}
