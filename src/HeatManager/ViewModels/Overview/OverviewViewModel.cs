using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Models.Schedules;
using System.Collections.Generic;
using System.Linq;

namespace HeatManager.ViewModels.Overview;

public partial class OverviewViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    public WeeklyStatisticsViewModel WeeklyStatisticsVM { get; }

    public OverviewViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;

        // Get the schedules from the optimizer
        var schedule = _mainWindowViewModel.Optimizer.Optimize();
        List<HeatProductionUnitSchedule> schedules = schedule.HeatProductionUnitSchedules.ToList();

        WeeklyStatisticsVM = new WeeklyStatisticsViewModel(schedules);
    }

    [RelayCommand]
    private void NavigateToConfigPanel()
    {
        _mainWindowViewModel.SetConfigPanelView();
    }
}