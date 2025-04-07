using System.Collections.Immutable;

namespace HeatManager.Core.Models.Schedules;

public interface ISchedule
{
    ImmutableList<IHeatProductionUnitSchedule> HeatProductionUnitSchedules { get; }

    ImmutableList<IElectricityProductionUnitSchedule> ElectricityProductionUnitSchedules { get; }

    /// <summary>
    /// Equals to the time between two entries in the schedule
    /// </summary>
    TimeSpan Resolution { get; }

    DateTime Start { get; }

    DateTime End { get; }

    public double[] Emissions { get; }

    public double[] Costs { get; }

    public double TotalCost { get; }

    public int Length { get; }

    // TODO: Add comments with corresponding units
    public double TotalEmissions { get; }

    /// <summary>
    /// MWh
    /// </summary>
    public double[] HeatDemand { get; set; }

    /// <summary>
    /// DKK/MWh(el)
    /// </summary>
    public decimal[] ElectricityPrice { get; set; }


    

    internal class ElectricityProductionUnitSchedule : IElectricityProductionUnitSchedule
    {
        public string Name { get; }
        public decimal TotalCost { get; }
        
        public double MaxHeatProduction { get; }

        public double ResourceConsumption { get; }
        public double TotalEmissions { get; }
        
        public List<IHeatProductionUnitResultDataPoint> DataPoints { get; }
        public void AddDataPoint(IHeatProductionUnitResultDataPoint dataPoint)
        {
            throw new NotImplementedException();
        }

        public double Emissions { get; }

        public double MaxElectricity { get; set; }
    }


}