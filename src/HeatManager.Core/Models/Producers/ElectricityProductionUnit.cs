namespace HeatManager.Core.Models.Producers;

public class ElectricityProductionUnit : HeatProductionUnit
{

    /// <summary>
    /// MW
    /// </summary>
    public double MaxElectricity { get; set; }
}