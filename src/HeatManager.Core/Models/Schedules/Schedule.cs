using System.Collections.Immutable;

namespace HeatManager.Core.Models.Schedules;

internal class Schedule : ISchedule
{
    public int Length { get; private set; }
    //Main Data
    public IEnumerable<IHeatProductionUnitSchedule> HeatProductionUnitSchedules { get; }
    public IEnumerable<IElectricityProductionUnitSchedule> ElectricityProductionUnitSchedules { get; }
    
    //Time Data
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }
    public TimeSpan Resolution { get; private set; }

    //Cost related data
    public decimal[] Costs { get; private set; }
    public decimal TotalCost => Costs.Sum();
    
    //Emissions related data
    public double[] Emissions { get; private set;}
    public double TotalEmissions => Emissions.Sum(); 
    
    //Heat related data
    public double[] HeatProduction { get; set; }
    public double TotalHeatProduction => HeatProduction.Sum();
    
    //Electricity related data
    public decimal[] ElectricityPrice { get; set; }

    public Schedule(IEnumerable<IHeatProductionUnitSchedule> heatProductionUnitSchedules,
        IEnumerable<IElectricityProductionUnitSchedule> electricityProductionUnitSchedules)
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
    
    private double[] GetEmissionsByHour(IEnumerable<IHeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        IEnumerable<IHeatProductionUnitSchedule> productionUnitSchedules = heatProductionUnitSchedules.ToList();
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
    
    private decimal[] GetCostsByHour(IEnumerable<IHeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        IEnumerable<IHeatProductionUnitSchedule> productionUnitSchedules = heatProductionUnitSchedules.ToList();
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

    private double[] GetHeatProductionByHour(IEnumerable<IHeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        IEnumerable<IHeatProductionUnitSchedule> productionUnitSchedules = HeatProductionUnitSchedules.ToList();
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