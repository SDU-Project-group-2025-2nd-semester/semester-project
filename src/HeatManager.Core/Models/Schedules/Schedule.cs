using System.Collections.Immutable;

namespace HeatManager.Core.Models.Schedules;

public class Schedule
{
    public int Length { get; private set; }
    //Main Data
    public ImmutableList<HeatProductionUnitSchedule> HeatProductionUnitSchedules { get; }
    public ImmutableList<ElectricityProductionUnitSchedule> ElectricityProductionUnitSchedules { get; }
    
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

    public Schedule(IEnumerable<HeatProductionUnitSchedule> heatProductionUnitSchedules,
        IEnumerable<ElectricityProductionUnitSchedule> electricityProductionUnitSchedules)
    {
        HeatProductionUnitSchedules = heatProductionUnitSchedules.ToImmutableList();
        ElectricityProductionUnitSchedules = electricityProductionUnitSchedules.ToImmutableList();

        Length = HeatProductionUnitSchedules[0].DataPoints.Count();
        Start = HeatProductionUnitSchedules[0].DataPoints[0].TimeFrom; 
        End = HeatProductionUnitSchedules[0].DataPoints[^1].TimeTo; 
        Resolution = End - Start; 
        
        Costs = GetCostsByHour(HeatProductionUnitSchedules);
        Emissions = GetEmissionsByHour(HeatProductionUnitSchedules); 
        HeatProduction = GetHeatProductionByHour(HeatProductionUnitSchedules);

    }

    private double[] GetEmissionsByHour(ImmutableList<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {

        var emissions = new double[Length];
        
        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in heatProductionUnitSchedules)
            {
                emissions[i] += schedule.Emissions.ElementAt(i);
            }
        }
        return emissions; 
    }
    
    private decimal[] GetCostsByHour(ImmutableList<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        var costs = new decimal[Length];
        
        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in heatProductionUnitSchedules)
            {
                costs[i] += schedule.Costs.ElementAt(i);
            }
        }
        return costs; 
    }

    private double[] GetHeatProductionByHour(ImmutableList<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        var heatProduction = new double[Length];
        
        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in heatProductionUnitSchedules)
            {
                heatProduction[i] += schedule.HeatProduction[i];
            }
        }
        return heatProduction; 
    }
}