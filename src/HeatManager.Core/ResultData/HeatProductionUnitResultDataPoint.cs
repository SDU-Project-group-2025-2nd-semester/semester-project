namespace HeatManager.Core.Models.Schedules;

/// <summary>
/// Represents a concrete implementation of heat production unit result data point.
/// </summary>
public class HeatProductionUnitResultDataPoint : IHeatProductionUnitResultDataPoint
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
    /// Gets the utilization percentage of the heat production unit.
    /// </summary>
    public double Utilization { get; }

    /// <summary>
    /// Gets the amount of heat produced during this period.
    /// </summary>
    public double HeatProduction { get; }

    /// <summary>
    /// Gets the cost associated with the heat production during this period.
    /// </summary>
    public decimal Cost { get; }

    /// <summary>
    /// Gets the amount of resources consumed during this period.
    /// </summary>
    public double ResourceConsumption { get; }

    /// <summary>
    /// Gets the amount of emissions produced during this period.
    /// </summary>
    public double Emissions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeatProductionUnitResultDataPoint"/> class.
    /// </summary>
    /// <param name="timeFrom">The start time of the production period.</param>
    /// <param name="timeTo">The end time of the production period.</param>
    /// <param name="utilization">The utilization percentage of the heat production unit.</param>
    /// <param name="heatProduction">The amount of heat produced during this period.</param>
    /// <param name="cost">The cost associated with the heat production during this period.</param>
    /// <param name="resourceConsumption">The amount of resources consumed during this period.</param>
    /// <param name="emissions">The amount of emissions produced during this period.</param>
    public HeatProductionUnitResultDataPoint(DateTime timeFrom, DateTime timeTo, double utilization, double heatProduction, decimal cost, double resourceConsumption, double emissions)
    {
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        Utilization = utilization;
        HeatProduction = heatProduction;
        Cost = cost;
        ResourceConsumption = resourceConsumption;
        Emissions = emissions;
    }
}