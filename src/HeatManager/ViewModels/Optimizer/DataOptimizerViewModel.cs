using HeatManager.Core.Services.Optimizers;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeatManager.ViewModels.Optimizer;

internal class DataOptimizerViewModel : ViewModelBase, IDataOptimizerViewModel
{
    public ObservableCollection<ISeries> Series { get; } = [];
    private readonly IOptimizer _optimizer;

    public DataOptimizerViewModel(IOptimizer optimizer)
    {
        _optimizer = optimizer;
        
        OptimizeData();
    }
    
    public void OptimizeData()
    {
        var schedule = _optimizer.Optimize();

        var schedules = schedule.HeatProductionUnitSchedules.ToList();

        for (int i = 0; i < schedules.Count; i++)
        {

            var unitSchedule = schedules[i];
            
            Series.Add(new StackedColumnSeries<double>
            {
                Values =  unitSchedule.HeatProduction,
                Name = unitSchedule.Name,
                Fill = new SolidColorPaint(Colors[i % Colors.Length])
            });
        }
    }
    
    public static readonly SKColor[] Colors = new[]
    {
        SKColors.Red,
        SKColors.Green,
        SKColors.Blue,
        SKColors.Yellow,
        SKColors.Orange,
        SKColors.Purple,
        SKColors.Pink,
        SKColors.Brown,
        SKColors.Gray,
        SKColors.Black,
        SKColors.White,
        SKColors.Cyan,
        SKColors.Magenta,
        SKColors.Lime,
        SKColors.Teal,
        SKColors.Navy,
        SKColors.Olive,
        SKColors.Maroon,
        SKColors.Aqua,
        SKColors.Silver,
        SKColors.Gold
    };
}
