namespace HeatManager.Core.Models.Producers;

internal class ElectricityProductionUnit : HeatProductionUnit, IElectricityProductionUnit
{

    /// <summary>
    /// MW
    /// </summary>
    public double MaxElectricity { get; set; }
}