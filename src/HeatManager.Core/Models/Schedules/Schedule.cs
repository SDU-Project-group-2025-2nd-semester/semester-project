namespace HeatManager.Core.Models.Schedules;

public class Schedule
{
    public int Length { get; private set; }
    //Main Data
    public IEnumerable<HeatProductionUnitSchedule> HeatProductionUnitSchedules { get; }
    public IEnumerable<ElectricityProductionUnitSchedule> ElectricityProductionUnitSchedules { get; }
    
    //Time Data
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }
    public TimeSpan Resolution { get; private set; }

    //Cost related data
    public decimal[] Costs => GetCostsByHour(HeatProductionUnitSchedules); 
    public decimal TotalCost => Costs.Sum();
    
    //Emissions related data
    public double[] Emissions => GetEmissionsByHour(HeatProductionUnitSchedules); 
    public double TotalEmissions => Emissions.Sum(); 
    
    //Heat related data
    public double[] HeatProduction => GetHeatProductionByHour(HeatProductionUnitSchedules);
    public double TotalHeatProduction => HeatProduction.Sum();
    
    //Electricity related data
    public decimal[] ElectricityPrice
    {
        get
        {
            if (ElectricityProductionUnitSchedules.Any())
            {
                return GetElectricityCostsByHour(ElectricityProductionUnitSchedules.ElementAt(0));
            } else return new decimal[Length];
        }
    }

    public double[] ElectricityProduction
    {
        get
        {
            if (ElectricityProductionUnitSchedules.Any())
            {
                return GetElectricityProductionByHour(ElectricityProductionUnitSchedules);
            } else return new double[Length];
        }
    } 
    
    //Resource consumption related data

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

    private decimal[] GetElectricityCostsByHour(ElectricityProductionUnitSchedule electricityProductionUnitSchedule)
    {
        var electricityCosts = new decimal[Length];
        var dataPoints = electricityProductionUnitSchedule.DataPoints.ToList();
        for (int i = 0; i < Length; i++)
        {
            electricityCosts[i] = dataPoints.ElementAt(i).ElectricityPrice;
        }
        
        return electricityCosts;
    }

    private double[] GetElectricityProductionByHour(
        IEnumerable<ElectricityProductionUnitSchedule> electricityProductionUnitSchedules)
    {
        var electricityProduction = new double[Length];
        var schedules = electricityProductionUnitSchedules.ToList();

        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in schedules)
            {
                electricityProduction[i] = schedule.ElectricityProduction.ElementAt(i);
            }
        }
        return electricityProduction;
    }
}