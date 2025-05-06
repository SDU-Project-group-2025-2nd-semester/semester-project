/* namespace HeatManager.Core.ViewModels;

internal class OverviewViewModel : ViewModelBase
{
} */

using CommunityToolkit.Mvvm.Input;

namespace HeatManager.ViewModels.Overview;

public partial class OverviewViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    public OverviewViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }

    [RelayCommand]
    private void NavigateToConfigPanel()
    {
        _mainWindowViewModel.SetConfigPanelView();
    }
}