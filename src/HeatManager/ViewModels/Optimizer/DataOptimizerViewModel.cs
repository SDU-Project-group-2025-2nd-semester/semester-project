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
    public ObservableCollection<ISeries> Series { get; } = new();
    public ObservableCollection<ScheduleData> TableData { get; } = new();

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
                // Toggle table visibility based on chart view
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
        // Switch between chart and table view
        IsChartVisible = !IsChartVisible;
    }

    public DataOptimizerViewModel(IOptimizer optimizer)
    {
        _optimizer = optimizer;
        OptimizeData();
    }

    public void OptimizeData()
    {
        var schedule = _optimizer.Optimize();
        var schedules = schedule.HeatProductionUnitSchedules.ToList();

        Series.Clear();
        TableData.Clear();

        foreach (var unitSchedule in schedules)
        {
            // Add data to chart
            Series.Add(new StackedColumnSeries<double>
            {
                Values = unitSchedule.HeatProduction,
                Name = unitSchedule.Name,
                Fill = new SolidColorPaint(Colors[schedules.IndexOf(unitSchedule) % Colors.Length])
            });

            // Add data to table
            TableData.Add(new ScheduleData
            {
                Name = unitSchedule.Name,
                HeatProduction = Math.Round(Convert.ToDecimal(unitSchedule.TotalHeatProduction), 3),
                MaxHeatProduction = Math.Round(Convert.ToDecimal(unitSchedule.MaxHeatProduction), 3),
                Emissions = Math.Round(Convert.ToDecimal(unitSchedule.TotalEmissions), 3),
                MaxEmissions = Math.Round(Convert.ToDecimal(unitSchedule.MaxEmissions), 3),
                Cost = Math.Round(unitSchedule.TotalCost, 2),
                MaxCost = Math.Round(unitSchedule.MaxCost, 2),
                ResourceConsumption = Math.Round(Convert.ToDecimal(unitSchedule.TotalResourceConsumption), 3),
                MaxResourceConsumption = Math.Round(Convert.ToDecimal(unitSchedule.MaxResourceConsumption), 3),
                Utilization = Math.Round(Convert.ToDecimal(unitSchedule.TotalUtilization), 3),
                MaxUtilization = Math.Round(Convert.ToDecimal(unitSchedule.MaxUtilization), 3)
            });
        }
    }

    // Chart color palette
    public static readonly SKColor[] Colors = new[]
    {
        SKColors.Red, SKColors.Green, SKColors.Blue, SKColors.Yellow, SKColors.Orange,
        SKColors.Purple, SKColors.Pink, SKColors.Brown, SKColors.Gray, SKColors.Black,
        SKColors.White, SKColors.Cyan, SKColors.Magenta, SKColors.Lime, SKColors.Teal,
        SKColors.Navy, SKColors.Olive, SKColors.Maroon, SKColors.Aqua, SKColors.Silver,
        SKColors.Gold
    };
}

// ViewModel table row representation
public class ScheduleData
{
    public string Name { get; set; }
    public decimal HeatProduction { get; set; }
    public decimal MaxHeatProduction { get; set; }
    public decimal Emissions { get; set; }
    public decimal MaxEmissions { get; set; }
    public decimal Cost { get; set; }
    public decimal MaxCost { get; set; }
    public decimal ResourceConsumption { get; set; }
    public decimal MaxResourceConsumption { get; set; }
    public decimal Utilization { get; set; }
    public decimal MaxUtilization { get; set; }
}
