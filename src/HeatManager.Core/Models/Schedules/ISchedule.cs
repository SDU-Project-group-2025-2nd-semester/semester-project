using System.Collections.Immutable;

namespace HeatManager.Core.Models.Schedules;

public interface ISchedule
{
    public int Length { get; }
    //Main Data
    public IEnumerable<IHeatProductionUnitSchedule> HeatProductionUnitSchedules { get; }
    public IEnumerable<IElectricityProductionUnitSchedule> ElectricityProductionUnitSchedules { get; }
    
    //Time Data
    public DateTime Start { get; }
    public DateTime End { get; }
    public TimeSpan Resolution { get;  }

    //Cost related data
    public decimal[] Costs { get; }
    public decimal TotalCost { get; }
    
    //Emissions related data
    public double[] Emissions { get; }
    public double TotalEmissions { get; } 
    
    //Heat related data
    public double[] HeatProduction { get; }
    public double TotalHeatProduction { get; }

    //Electricity related data
    public decimal[] ElectricityPrice { get; }


    

    internal class ElectricityProductionUnitSchedule : IElectricityProductionUnitSchedule
    {
        public string Name { get; }
        public List<IHeatProductionUnitResultDataPoint> DataPoints { get; }
        public double[] HeatProduction { get; }
        public double TotalHeatProduction { get; }
        public double MaxHeatProduction { get; }
        public double[] Emissions { get; }
        public double TotalEmissions { get; }
        public double MaxEmissions { get; }
        public decimal[] Costs { get; }
        public decimal TotalCost { get; }
        public decimal MaxCost { get; }
        public double[] ResourceConsumption { get; }
        public double TotalResourceConsumption { get; }
        public double MaxResourceConsumption { get; }
        public double[] Utilization { get; }
        public double TotalUtilization { get; }
        public double MaxUtilization { get; }
        public void AddDataPoint(IHeatProductionUnitResultDataPoint dataPoint)
        {
            throw new NotImplementedException();
        }

        public double MaxElectricity { get; set; }
    }


}