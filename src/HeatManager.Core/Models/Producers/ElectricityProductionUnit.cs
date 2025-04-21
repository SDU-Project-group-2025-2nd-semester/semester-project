namespace HeatManager.Core.Models.Producers;

public class ElectricityProductionUnit : ProductionUnitBase
{

    /// <summary>
    /// MW
    /// </summary>
    public double MaxElectricity { get; set; }

    public new ElectricityProductionUnit Clone() => (ElectricityProductionUnit)MemberwiseClone();
}