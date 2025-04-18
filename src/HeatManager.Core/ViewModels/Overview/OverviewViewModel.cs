/* namespace HeatManager.Core.ViewModels;

internal class OverviewViewModel : ViewModelBase
{
} */

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HeatManager.Core.ViewModels;

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