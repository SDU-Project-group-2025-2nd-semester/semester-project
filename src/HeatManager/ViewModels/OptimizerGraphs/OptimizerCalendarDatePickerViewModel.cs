using System;
using System.Collections.Generic;
using System.Linq;

namespace HeatManager.ViewModels.OptimizerGraphs;

internal partial class OptimizerCalendarDatePickerViewModel : ViewModelBase
{

    /// <summary>
    /// Delegate for handling date selection events in the calendar.
    /// </summary>
    /// <param name="selectedDate">The newly selected date, or null if selection was cleared.</param>
    public delegate void DateSelectedHandler(DateTime? selectedDate);

    /// <summary>
    /// Event that fires when a date is selected in the calendar control.
    /// Subscribers can respond to date selection changes by handling this event.
    /// </summary>
    public event DateSelectedHandler? DateSelected;

    /// <summary>
    /// Private backing field for the <see cref="CalendarSelectedDate"/> property.
    /// Stores the currently selected date in the calendar component.
    /// </summary>
    private DateTime? _calendarSelectedDate;

    /// <summary>
    /// Gets or sets the currently selected date in the calendar.
    /// When the value changes, notifies the parent via the <see cref="DateSelected"/> event.
    /// </summary>
    /// <value>
    /// The selected date, or <c>null</c> if no date is selected.
    /// </value>
    public DateTime? CalendarSelectedDate
    {
        get => _calendarSelectedDate;
        set
        {
            if (SetProperty(ref _calendarSelectedDate, value))
            {
                // Notify parent about the date change
                DateSelected?.Invoke(value);
            }
        }
    }

    /// <summary>
    /// Gets the minimum date in the dataset.
    /// </summary>
    public DateTime MinDate { get; private set; }

    /// <summary>
    /// Gets the maximum date in the dataset.
    /// </summary>
    public DateTime MaxDate { get; private set; }

    /// <summary>
    /// Gets or sets the start date to display in the calendar.
    /// </summary>
    public DateTime CalendarDisplayDateStart { get; set; }

    /// <summary>
    /// Gets or sets the end date to display in the calendar.
    /// </summary>
    public DateTime CalendarDisplayDateEnd { get; set; }

    public OptimizerCalendarDatePickerViewModel(List<DateTime> orderedTimes)
    {
        SetDateRange(orderedTimes);
        CalendarDisplayDateStart = MinDate;
        CalendarDisplayDateEnd = MaxDate;
    }

    /// <summary>
    /// Sets the minimum and maximum date values based on the provided list of time points.
    /// </summary>
    /// <param name="times">A list of DateTime objects.</param>
    private void SetDateRange(List<DateTime> times)
    {
        if (times.Count > 0)
        {
            MinDate = times.First();
            MaxDate = times.Last();
        }
    }
}