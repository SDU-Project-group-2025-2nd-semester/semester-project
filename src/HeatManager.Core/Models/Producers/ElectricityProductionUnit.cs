namespace HeatManager.Core.Models.Producers;

public class ElectricityProductionUnit : HeatProductionUnit, IElectricityProductionUnit
{

    /// <summary>
    /// MW
    /// </summary>
    public double MaxElectricity { get; set; }
}