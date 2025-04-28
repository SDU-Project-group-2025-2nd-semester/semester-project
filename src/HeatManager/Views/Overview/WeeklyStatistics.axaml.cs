using Avalonia.Controls;
using HeatManager.ViewModels.Overview;

namespace HeatManager.Views.Overview;

public partial class WeeklyStatistics : UserControl
{
    public WeeklyStatistics()
    {
        InitializeComponent();
        DataContext = new WeeklyStatisticsViewModel();
    }
}