using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services.SourceDataProviders;
using System.Collections.Generic;
using System.Linq;

namespace HeatManager.ViewModels.Overview;

public partial class OverviewViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    public ProductionUnitsViewModel ProductionUnitsViewModel { get; }
    public WeeklyStatisticsViewModel WeeklyStatisticsVM { get; }

    public OverviewViewModel(MainWindowViewModel mainWindowViewModel, ProductionUnitsViewModel productionUnitsViewModel, ISourceDataProvider sourceDataProvider)
    {
        _mainWindowViewModel = mainWindowViewModel;
        ProductionUnitsViewModel = productionUnitsViewModel;
        
        // Get the schedules from the optimizer
        var schedule = _mainWindowViewModel.Optimizer.Optimize();
        List<HeatProductionUnitSchedule> schedules = schedule.HeatProductionUnitSchedules.ToList();

        WeeklyStatisticsVM = new WeeklyStatisticsViewModel(schedules, sourceDataProvider);
    }

    [RelayCommand]
    private void NavigateToConfigPanel()
    {
        _mainWindowViewModel.SetConfigPanelView();
    }
}