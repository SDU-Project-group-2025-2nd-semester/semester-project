namespace HeatManager.Core.Models.Schedules;

/// <summary>
/// Represents a data point for electricity production results.
/// </summary>
public interface IElectricityProductionResultDataPoint
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
    /// Gets the price of electricity during this period.
    /// </summary>
    decimal ElectricityPrice { get; }

    /// <summary>
    /// Gets the amount of electricity produced during this period.
    /// </summary>
    double ElectricityProduction { get; }
}