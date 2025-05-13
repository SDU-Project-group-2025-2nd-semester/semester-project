using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Models.Schedules;
using HeatManager.ViewModels.Optimizer;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatManager.ViewModels.OptimizerGraphs;
internal partial class OptimizerHeatProductionGraphViewModel : ViewModelBase, IDataOptimizerViewModel, INotifyPropertyChanged
{


    private DateTimeOffset? _selectedDate;
    private string? _lastLabel;
    private List<DateTime> orderedTimes;


    public OptimizerHeatProductionGraphViewModel(List<HeatProductionUnitSchedule> schedules, List<DateTime> OrderedTimes, DateTimeOffset? minDate)
    {
        MinDate = minDate;
        orderedTimes = OrderedTimes;
        BuildChartSeries(schedules);
        ConfigureAxes(orderedTimes, schedules);
    }


    /// <summary>
    /// Gets the chart series collection.
    /// </summary>
    public ObservableCollection<ISeries> Series { get; } = new();

    /// <summary>
    /// Gets or sets the selected date for filtering.
    /// </summary>
    public DateTimeOffset? SelectedDate
    {
        get => _selectedDate;
        set
        {
            SetProperty(ref _selectedDate, value);
            OnDateSelected();
        }
    }

    /// <summary>
    /// Gets the minimum date in the dataset.
    /// </summary>
    public DateTimeOffset? MinDate { get; private set; }

    /// <summary>
    /// Gets the X-axis configuration.
    /// </summary>
    public List<Axis> XAxes { get; private set; } = new();

    /// <summary>
    /// Gets the Y-axis configuration.
    /// </summary>
    public List<Axis> YAxes { get; private set; } = new();

    /// <summary>
    /// Gets or sets the chart margin.
    /// </summary>
    public Margin? Margin { get; set; }

    /// <summary>
    /// Handles chart update events.
    /// </summary>
    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (ICartesianChartView)args.Chart;
    }

    private void BuildChartSeries(List<HeatProductionUnitSchedule> schedules)
    {
        Series.Clear();
        int i = 0;

        foreach (var unitSchedule in schedules)
        {
            i++;
            Series.Add(new StackedColumnSeries<double>
            {
                Values = unitSchedule.HeatProduction,
                Name = unitSchedule.Name,
                Fill = new SolidColorPaint(Colors[i])
            });
        }
    }

    private void ConfigureAxes(List<DateTime> orderedTimes, List<HeatProductionUnitSchedule> schedules)
    {
        int i = 0;
        foreach (var unitSchedule in schedules)
        {
            i++;

            XAxes = new List<Axis>
            {
                new Axis
                {
                    Name = "[ h ]",
                    MinLimit = 0,
                    MaxLimit = orderedTimes.Count > 0 ? orderedTimes.Count - 1 : 0,
                    Labeler = value =>
                    {
                        int index = (int)Math.Round(value);
                        if (index >= 0 && index < orderedTimes.Count)
                        {
                            var dateAxis = XAxes.FirstOrDefault();
                            var visibleRange = dateAxis?.MaxLimit - dateAxis?.MinLimit;
                            var t = orderedTimes[index];

                            string currentLabel = visibleRange > 45
                                ? t.ToString("dd/MM/yy")
                                : t.ToString("HH:mm dd/MM/yy");

                            if (visibleRange > 45)
                            {
                                if (currentLabel == _lastLabel)
                                    return string.Empty;

                                _lastLabel = currentLabel;
                            }

                            return currentLabel;
                        }

                        return string.Empty;
                    }
                }
            };

            YAxes = new List<Axis>
            {
                new Axis
                {
                    Name = "Heat Production [ MW ]"
                }
            };

            Margin = new Margin(100, Margin.Auto, 50, Margin.Auto);
            SelectedDate = MinDate;
        }
    }



    /// <summary>
    /// Called when a new date is selected.
    /// </summary>
    private void OnDateSelected()
    {
        if (SelectedDate.HasValue)
        {
            SetXMinLimit();
        }
    }

    /// <summary>
    /// Sets the X-axis window based on selected date.
    /// </summary>
    private void SetXMinLimit()
    {

        if (SelectedDate.HasValue)
        {
            var index = orderedTimes.FindIndex(t => t >= SelectedDate.Value.DateTime);

            if (index != -1)
            {
                XAxes[0].MinLimit = index;
                XAxes[0].MaxLimit = Math.Min(index + 30, orderedTimes.Count - 1);
            }
            else
            {
                XAxes[0].MinLimit = 0;
            }
        }
        else
        {
            XAxes[0].MinLimit = 0;
        }
    }




    /// <summary>
    /// Predefined chart color palette.
    /// </summary>
    private static readonly SKColor[] Colors =
    {
        SKColors.White, SKColors.Maroon, SKColors.Red, SKColors.Magenta, SKColors.Pink,
        SKColors.Green, SKColors.Blue, SKColors.Yellow, SKColors.Orange, SKColors.Purple,
        SKColors.Brown, SKColors.Gray, SKColors.Black, SKColors.Cyan, SKColors.Lime,
        SKColors.Teal, SKColors.Navy, SKColors.Olive, SKColors.Aqua, SKColors.Silver,
        SKColors.Gold
    };
}