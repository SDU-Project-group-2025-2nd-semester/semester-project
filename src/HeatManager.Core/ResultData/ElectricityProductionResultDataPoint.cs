namespace HeatManager.Core.Models.Schedules;

/// <summary>
/// Represents a concrete implementation of electricity production result data point.
/// </summary>
public class ElectricityProductionResultDataPoint : IElectricityProductionResultDataPoint
{
    /// <summary>
    /// Gets the start time of the production period.
    /// </summary>
    public DateTime TimeFrom { get; }

    /// <summary>
    /// Gets the end time of the production period.
    /// </summary>
    public DateTime TimeTo { get; }

    /// <summary>
    /// Gets the price of electricity during this period.
    /// </summary>
    public decimal ElectricityPrice { get; }

    /// <summary>
    /// Gets the amount of electricity produced during this period.
    /// </summary>
    public double ElectricityProduction { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ElectricityProductionResultDataPoint"/> class.
    /// </summary>
    /// <param name="timeFrom">The start time of the production period.</param>
    /// <param name="timeTo">The end time of the production period.</param>
    /// <param name="electricityPrice">The price of electricity during this period.</param>
    /// <param name="electricityProduction">The amount of electricity produced during this period.</param>
    public ElectricityProductionResultDataPoint(DateTime timeFrom, DateTime timeTo, decimal electricityPrice, double electricityProduction)
    {
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        ElectricityPrice = electricityPrice;
        ElectricityProduction = electricityProduction;
    }
}