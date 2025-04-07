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

}