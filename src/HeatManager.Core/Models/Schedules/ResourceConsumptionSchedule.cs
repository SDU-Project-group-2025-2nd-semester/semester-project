namespace HeatManager.Core.Models.Schedules;

/// <summary>
/// Represents a schedule for tracking resource consumption over time.
/// </summary>
public class ResourceConsumptionSchedule
{
    /// <summary>
    /// Gets the name of the resource being consumed.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the array of consumption values for each time period.
    /// </summary>
    public double[] Consumption { get; }

    /// <summary>
    /// Gets the total consumption across all time periods.
    /// </summary>
    public double TotalConsumption { get; }
}