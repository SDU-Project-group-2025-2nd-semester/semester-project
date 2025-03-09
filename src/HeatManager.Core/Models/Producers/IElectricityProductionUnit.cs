namespace HeatManager.Core.Models.Producers;

public interface IElectricityProductionUnit : IHeatProductionUnit
{
    /// <summary>
    /// MW
    /// </summary>
    public double MaxElectricity { get; set; }
}