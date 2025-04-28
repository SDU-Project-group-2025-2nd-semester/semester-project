namespace HeatManager.Core.Models.Schedules;

/// <summary>
/// Represents a schedule for a single electricity production unit, tracking production and pricing data.
/// </summary>
public class ElectricityProductionUnitSchedule
{
    /// <summary>
    /// Gets the name of the electricity production unit.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the array of electricity production values for each time period.
    /// </summary>
    public double[] ElectricityProduction => DataPoints.Select(x => x.ElectricityProduction).ToArray();

    /// <summary>
    /// Gets the maximum electricity production value across all time periods.
    /// </summary>
    public double MaxElectricityProduction => ElectricityProduction.Max();

    /// <summary>
    /// Gets the total electricity production across all time periods.
    /// </summary>
    public double TotalElectricityProduction => ElectricityProduction.Sum();
    
    /// <summary>
    /// Gets the array of electricity prices for each time period.
    /// </summary>
    public decimal[] ElectricityPrices => DataPoints.Select(x => x.ElectricityPrice).ToArray();

    /// <summary>
    /// Gets the maximum electricity price across all time periods.
    /// </summary>
    public decimal MaxElectricityPrice => ElectricityPrices.Max(); 

    /// <summary>
    /// Gets the total electricity price across all time periods.
    /// </summary>
    public decimal TotalElectricityPrice => ElectricityPrices.Sum();

    /// <summary>
    /// Gets or sets the collection of data points for this schedule.
    /// </summary>
    public List<ElectricityProductionResultDataPoint> DataPoints { get; set; }

    /// <summary>
    /// Initializes a new instance of the ElectricityProductionUnitSchedule class.
    /// </summary>
    /// <param name="name">The name of the electricity production unit.</param>
    public ElectricityProductionUnitSchedule(string name)
    {
        Name = name; 
        DataPoints = new List<ElectricityProductionResultDataPoint>();
    }
    
    /// <summary>
    /// Adds a new data point to the schedule.
    /// </summary>
    /// <param name="dataPoint">The data point to add.</param>
    public void AddDataPoint(ElectricityProductionResultDataPoint dataPoint)
    {
        DataPoints.Add(dataPoint);
    }
}