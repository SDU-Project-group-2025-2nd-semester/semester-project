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


    internal class HeatProductionUnitSchedule : IHeatProductionUnitSchedule
    {
        public bool[] IsOn { get; }

        public string Name { get; }

        public decimal Cost { get; }

        public double MaxHeatProduction { get; }

        public double ResourceConsumption { get; }

        public IResourceConsumptionSchedule ResourceConsumption2 { get; }

        public double Emissions { get; }
    }

    internal class ElectricityProductionUnitSchedule : IElectricityProductionUnitSchedule
    {
        public bool[] IsOn { get; } // Probably add percentage 

        public string Name { get; }

        public decimal Cost { get; }

        public double MaxHeatProduction { get; }

        public double ResourceConsumption { get; }

        public IResourceConsumptionSchedule ResourceConsumption2 { get; }

        public double Emissions { get; }

        public double MaxElectricity { get; set; }
    }


}