using CommunityToolkit.Mvvm.Input;

namespace HeatManager.ViewModels.Overview;

public partial class OverviewViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    public ProductionUnitsViewModel ProductionUnitsViewModel { get; }

    public OverviewViewModel(MainWindowViewModel mainWindowViewModel, ProductionUnitsViewModel productionUnitsViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        ProductionUnitsViewModel = productionUnitsViewModel;
    }

    [RelayCommand]
    private void NavigateToConfigPanel()
    {
        _mainWindowViewModel.SetConfigPanelView();
    }
}