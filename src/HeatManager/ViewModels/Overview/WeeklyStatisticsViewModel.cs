using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services.SourceDataProviders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeatManager.ViewModels.Overview;

public partial class WeeklyStatisticsViewModel : ViewModelBase
{
    [ObservableProperty]
    private double heatDemand;

    [ObservableProperty]
    private double totalHeat;

    [ObservableProperty]
    private double resourceConsumption;

    [ObservableProperty]
    private double co2Emissions;

    [ObservableProperty]
    private double expenses;

    public WeeklyStatisticsViewModel(List<HeatProductionUnitSchedule> schedules, ISourceDataProvider sourceDataProvider)
    {
        // Sum up the values for all units
        TotalHeat = Math.Round(schedules.Sum(s => (double)s.TotalHeatProduction), 3);
        ResourceConsumption = Math.Round(schedules.Sum(s => s.TotalResourceConsumption.Value), 3);
        Co2Emissions = Math.Round(schedules.Sum(s => (double)s.TotalEmissions), 3);
        Expenses = Math.Round(schedules.Sum(s => (double)s.TotalCost), 3);

        // Sum all HeatDemand values from the source data
        HeatDemand = Math.Round(sourceDataProvider.SourceDataCollection?.DataPoints.Sum(dp => dp.HeatDemand) ?? 0, 3);
    }
}
