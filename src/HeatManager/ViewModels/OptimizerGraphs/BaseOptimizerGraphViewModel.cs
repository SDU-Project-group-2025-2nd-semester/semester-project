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
using CommunityToolkit.Mvvm.ComponentModel;

namespace HeatManager.ViewModels.OptimizerGraphs;

/// <summary>
/// Base ViewModel for displaying optimizer graphs using LiveCharts2
/// </summary>
internal abstract partial class BaseOptimizerGraphViewModel : ViewModelBase, IDataOptimizerViewModel, INotifyPropertyChanged
{
    // private DateTimeOffset? _selectedDate;
    private DateTime? _calendarSelectedDate;

    private string? _lastLabel;
    protected List<DateTime> orderedTimes;

    /// <summary>
    /// Gets the chart series collection.
    /// </summary>
    public ObservableCollection<ISeries> Series { get; } = new();

    /// <summary>
    /// Gets the view model responsible for managing the calendar date picker functionality
    /// within the optimizer graph. This view model handles the selection and display of dates
    /// relevant to the optimization process.
    /// </summary>
    public OptimizerCalendarDatePickerViewModel CalendarDatePicker { get; private set; }



    /// <summary>
    /// Gets or sets the selected date for filtering.
    /// </summary>
    public DateTime? CalendarSelectedDate
    {
        get => _calendarSelectedDate;
        set
        {
            SetProperty(ref _calendarSelectedDate, value);
            OnDateSelected();
        }
    }

    /// <summary>
    /// Gets the minimum date in the dataset.
    /// </summary>
    public DateTime? MinDate { get; private set; }



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
    /// Initializes a new instance of the <see cref="OptimizerHeatProductionGraphViewModel"/> class.
    /// </summary>
    /// <param name="schedules">List of heat production unit schedules</param>
    /// <param name="OrderedTimes">Ordered time slots for chart axis labeling</param>
    /// <param name="minDate">Minimum(earliest) date in the dataset</param>
    protected BaseOptimizerGraphViewModel(List<HeatProductionUnitSchedule> schedules, List<DateTime> OrderedTimes, DateTimeOffset? minDate)
    {
        this.orderedTimes = OrderedTimes;
        MinDate = minDate?.DateTime;
        BuildChartSeries(schedules);
        ConfigureAxes(orderedTimes, schedules);

        CalendarDatePicker = new OptimizerCalendarDatePickerViewModel(OrderedTimes);
        CalendarDatePicker.DateSelected += OnCalendarDateSelected;

        if (MinDate.HasValue)
        {
            CalendarDatePicker.CalendarSelectedDate = MinDate.Value;
        }
    }

    /// <summary>
    ///  Handles chart's update events.
    /// </summary>
    /// <param name="args"></param>
    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (ICartesianChartView)args.Chart;
    }

    /// <summary>
    /// Create the chart series - each subclass must implement this
    /// </summary>
    /// <param name="schedules">List of Unit Schedules</param>
    protected abstract void BuildChartSeries(List<HeatProductionUnitSchedule> schedules);

    /// <summary>
    /// Gets the Y-axis title - each subclass must implement this
    /// </summary>
    protected abstract string GetYAxisTitle();

    /// <summary>
    /// Configures the chart's X and Y axes using provided time and schedule data.
    /// </summary>
    /// <param name="orderedTimes">The list of ordered time slots.</param>
    /// <param name="schedules">The list of schedules.</param>
    protected void ConfigureAxes(List<DateTime> orderedTimes, List<HeatProductionUnitSchedule> schedules)
    {
        XAxes = new List<Axis>
        {
            new Axis
            {
                Name = " Date [ h ]",
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
                Name = GetYAxisTitle()
            }
        };

        Margin = new Margin(100, Margin.Auto, 50, Margin.Auto);
        CalendarSelectedDate = MinDate;
    }

    /// <summary>
    /// Handles date selection events from the calendar picker control.
    /// </summary>
    /// <param name="selectedDate">The newly selected date from the calendar, or null if selection was cleared.</param>
    /// <remarks>
    /// This method synchronizes the calendar component's selected date with the graph view model's state.
    /// When a new date is selected in the calendar, this handler updates the <see cref="CalendarSelectedDate"/> 
    /// property, which in turn triggers <see cref="OnDateSelected"/> to adjust the graph's visible range.
    /// </remarks>
    private void OnCalendarDateSelected(DateTime? selectedDate)
    {
        CalendarSelectedDate = selectedDate;
    }

    /// <summary>
    /// Called when a new date is selected.
    /// </summary>
    protected void OnDateSelected()
    {
        Console.WriteLine($"CalendarSelectedDate.HasValue: {CalendarSelectedDate.HasValue}");
        if (CalendarSelectedDate.HasValue)
        {
            SetXMinLimit();
        }
    }

    /// <summary>
    /// Sets the X-axis window range based on the selected date.
    /// </summary>
    protected void SetXMinLimit()
    {
        if (CalendarSelectedDate.HasValue)
        {
            var index = orderedTimes.FindIndex(t => t >= CalendarSelectedDate.Value);
            Console.WriteLine($"SetXMinLimit() > in:  {index}");

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
    /// Predefined color palette for chart series.
    /// </summary>
    protected static readonly SKColor[] Colors =
    {
        SKColors.White, SKColors.Maroon, SKColors.Red, SKColors.Magenta, SKColors.Pink,
        SKColors.Green, SKColors.Blue, SKColors.Yellow, SKColors.Orange, SKColors.Purple,
        SKColors.Brown, SKColors.Gray, SKColors.Black, SKColors.Cyan, SKColors.Lime,
        SKColors.Teal, SKColors.Navy, SKColors.Olive, SKColors.Aqua, SKColors.Silver,
        SKColors.Gold
    };
}
