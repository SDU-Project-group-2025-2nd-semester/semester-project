namespace HeatManager.Core.Models.Schedules;

public interface IElectricityProductionUnitSchedule : IHeatProductionUnitSchedule
{
    public double MaxElectricity { get; set; }
}