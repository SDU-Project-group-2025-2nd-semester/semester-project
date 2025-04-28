namespace HeatManager.Core.Models.Schedules;

/// <summary>
/// Represents a data point for heat production unit results.
/// </summary>
public interface IHeatProductionUnitResultDataPoint
{
    /// <summary>
    /// Gets the start time of the production period.
    /// </summary>
    DateTime TimeFrom { get; }

    /// <summary>
    /// Gets the end time of the production period.
    /// </summary>
    DateTime TimeTo { get; }

    /// <summary>
    /// Gets the utilization percentage of the heat production unit.
    /// </summary>
    double Utilization { get; }

    /// <summary>
    /// Gets the amount of heat produced during this period.
    /// </summary>
    double HeatProduction { get; }

    /// <summary>
    /// Gets the cost associated with the heat production during this period.
    /// </summary>
    decimal Cost { get; }

    /// <summary>
    /// Gets the amount of resources consumed during this period.
    /// </summary>
    double ResourceConsumption { get; }

    /// <summary>
    /// Gets the amount of emissions produced during this period.
    /// </summary>
    double Emissions { get; }
}