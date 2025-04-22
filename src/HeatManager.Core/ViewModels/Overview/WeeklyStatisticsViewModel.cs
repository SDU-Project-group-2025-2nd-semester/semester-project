using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models;

namespace HeatManager.Core.ViewModels;

public partial class WeeklyStatisticsViewModel : ViewModelBase
{
    [ObservableProperty]
    private double heatDemand = 2500;

    [ObservableProperty]
    private double maxProduction = 500;

    [ObservableProperty]
    private double peakConsumption = 300;

    [ObservableProperty]
    private double expenses = 895;

    [ObservableProperty]
    private double profit = 335.77;
}
