using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Linq;
using HeatManager.Core.Services.Optimizers;
using LiveChartsCore;
using System;
using System.ComponentModel;

namespace HeatManager.ViewModels.Optimizer;

internal class DataOptimizerViewModel : ViewModelBase, IDataOptimizerViewModel, INotifyPropertyChanged
{
    public ObservableCollection<ISeries> Series { get; } = new ObservableCollection<ISeries>();
    public ObservableCollection<ScheduleData> TableData { get; } = new ObservableCollection<ScheduleData>();  // Data for the table

    private readonly IOptimizer _optimizer;

    private bool _isChartVisible = true; // Default to chart view
    private bool _isTableVisible = false;  
    public bool IsChartVisible
    {
        get => _isChartVisible;
        set
        {
            if (SetProperty(ref _isChartVisible, value))
            {
                IsTableVisible = !value;
            }
        }
    }

    public bool IsTableVisible
    {
        get => !_isChartVisible;
        set
        {
            SetProperty(ref _isTableVisible, value);
        }
    }

    public void ToggleView()
    {
        if (IsChartVisible)
        {
            IsChartVisible = false;
        }
        else
        {
            IsChartVisible = true;
        }
    }

    public DataOptimizerViewModel(IOptimizer optimizer)
    {
        _optimizer = optimizer;
        OptimizeData();
    }

    public void OptimizeData()
    {
        Console.WriteLine("Optimizing data...");
        var schedule = _optimizer.Optimize();
        var schedules = schedule.HeatProductionUnitSchedules.ToList();

        for (int i = 0; i < schedules.Count; i++)
        {
            var unitSchedule = schedules[i];

            // Add series for chart
            Series.Add(new StackedColumnSeries<double>
            {
                Values = unitSchedule.HeatProduction,
                Name = unitSchedule.Name,
                Fill = new SolidColorPaint(Colors[i % Colors.Length])
            });
            

            TableData.Add(new ScheduleData
            {
                Name = unitSchedule.Name,
                HeatProduction = Convert.ToDecimal(unitSchedule.TotalHeatProduction),
                Cost = unitSchedule.TotalCost,
                Emissions = Convert.ToDecimal(unitSchedule.TotalEmissions)

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



public class ScheduleData
{
    public string Name { get; set; }
    public decimal HeatProduction { get; set; }
    public decimal Cost { get; set; }
    public decimal Emissions { get; set; }
}

