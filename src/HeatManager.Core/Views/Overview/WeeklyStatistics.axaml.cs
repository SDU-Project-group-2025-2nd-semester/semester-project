using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HeatManager.Core.ViewModels;

namespace HeatManager.Core.Views;

public partial class WeeklyStatistics : UserControl
{
    public WeeklyStatistics()
    {
        InitializeComponent();
        DataContext = new WeeklyStatisticsViewModel();
    }
}