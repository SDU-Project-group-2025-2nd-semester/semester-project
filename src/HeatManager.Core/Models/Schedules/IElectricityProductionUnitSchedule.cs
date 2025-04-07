namespace HeatManager.Core.Models.Schedules;

public interface IElectricityProductionUnitSchedule : IHeatProductionUnitSchedule
{
    
    public double[] ElectricityProduction { get; set; }
    public double MaxElectricityProduction => ElectricityProduction.Max();
    public double TotalElectricityProduction => ElectricityProduction.Sum();
    
    public decimal[] ElectricityPrices { get; set; }
    public decimal MaxElectricityPrice => ElectricityPrices.Max();
    public decimal TotalElectricityPrice => ElectricityPrices.Sum();

}