namespace HeatManager.Core.Models.Schedules;

/// <summary>
/// Represents a comprehensive schedule that aggregates heat and electricity production schedules.
/// This class manages time-based data, costs, emissions, and production metrics.
/// </summary>
public class Schedule
{
    /// <summary>
    /// Gets the total number of time periods in the schedule.
    /// </summary>
    public int Length { get; private set; }
    //Main Data
    /// <summary>
    /// Gets the collection of heat production unit schedules.
    /// </summary>
    public IEnumerable<HeatProductionUnitSchedule> HeatProductionUnitSchedules { get; }
    /// <summary>
    /// Gets the collection of electricity production unit schedules.
    /// </summary>
    public IEnumerable<ElectricityProductionUnitSchedule> ElectricityProductionUnitSchedules { get; }
    
    //Time Data
    /// <summary>
    /// Gets the start time of the schedule.
    /// </summary>
    public DateTime Start { get; private set; }
    /// <summary>
    /// Gets the end time of the schedule.
    /// </summary>
    public DateTime End { get; private set; }
    /// <summary>
    /// Gets the time resolution of the schedule.
    /// </summary>
    public TimeSpan Resolution { get; private set; }

    //Cost related data
    /// <summary>
    /// Gets the array of costs for each time period.
    /// </summary>
    public decimal[] Costs { get; private set; }
    /// <summary>
    /// Gets the total cost across all time periods.
    /// </summary>
    public decimal TotalCost => Costs.Sum();
    
    //Emissions related data
    /// <summary>
    /// Gets the array of emissions for each time period.
    /// </summary>
    public double[] Emissions { get; private set;}
    /// <summary>
    /// Gets the total emissions across all time periods.
    /// </summary>
    public double TotalEmissions => Emissions.Sum(); 
    
    //Heat related data
    /// <summary>
    /// Gets or sets the array of heat production values for each time period.
    /// </summary>
    public double[] HeatProduction { get; set; }
    /// <summary>
    /// Gets the total heat production across all time periods.
    /// </summary>
    public double TotalHeatProduction => HeatProduction.Sum();
    
    //Electricity related data
    /// <summary>
    /// Gets or sets the array of electricity prices for each time period.
    /// </summary>
    public decimal[] ElectricityPrice { get; set; }

    /// <summary>
    /// Initializes a new instance of the Schedule class.
    /// </summary>
    /// <param name="heatProductionUnitSchedules">Collection of heat production unit schedules.</param>
    /// <param name="electricityProductionUnitSchedules">Collection of electricity production unit schedules.</param>
    public Schedule(IEnumerable<HeatProductionUnitSchedule> heatProductionUnitSchedules,
        IEnumerable<ElectricityProductionUnitSchedule> electricityProductionUnitSchedules)
    {
        HeatProductionUnitSchedules = heatProductionUnitSchedules;
        ElectricityProductionUnitSchedules = electricityProductionUnitSchedules;

        CreateProperties(); 
    }

    private void CreateProperties()
    {
        Length = HeatProductionUnitSchedules.ElementAt(0).DataPoints.Count();
        Start = HeatProductionUnitSchedules.ElementAt(0).DataPoints.ElementAt(0).TimeFrom; 
        End = HeatProductionUnitSchedules.ElementAt(0).DataPoints
            .ElementAt(Length - 1).TimeTo; //TODO: make this actually readable
        Resolution = End - Start; 
        
        
        
        Costs = GetCostsByHour(HeatProductionUnitSchedules);
        Emissions = GetEmissionsByHour(HeatProductionUnitSchedules); 
        HeatProduction = GetHeatProductionByHour(HeatProductionUnitSchedules);
        
        
    }
    
    private double[] GetEmissionsByHour(IEnumerable<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        IEnumerable<HeatProductionUnitSchedule> productionUnitSchedules = heatProductionUnitSchedules.ToList();
        var emissions = new double[Length];
        
        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in productionUnitSchedules)
            {
                emissions[i] += schedule.Emissions.ElementAt(i);
            }
        }
        return emissions; 
    }
    
    private decimal[] GetCostsByHour(IEnumerable<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        IEnumerable<HeatProductionUnitSchedule> productionUnitSchedules = heatProductionUnitSchedules.ToList();
        var costs = new decimal[Length];
        
        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in productionUnitSchedules)
            {
                costs[i] += schedule.Costs.ElementAt(i);
            }
        }
        return costs; 
    }

    private double[] GetHeatProductionByHour(IEnumerable<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        IEnumerable<HeatProductionUnitSchedule> productionUnitSchedules = HeatProductionUnitSchedules.ToList();
        var heatProduction = new double[Length];
        
        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in productionUnitSchedules)
            {
                heatProduction[i] += schedule.HeatProduction.ElementAt(i);
            }
        }
        return heatProduction; 
    }
}