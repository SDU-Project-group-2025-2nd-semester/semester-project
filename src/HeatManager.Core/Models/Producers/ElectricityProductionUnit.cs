namespace HeatManager.Core.Models.Producers;

public class ElectricityProductionUnit : ProductionUnitBase
{

    /// <summary>
    /// MW
    /// </summary>
    public double MaxElectricity { get; set; }

    public override double? MaxElectricitySafe => MaxElectricity;

    public new ElectricityProductionUnit Clone() => (ElectricityProductionUnit)MemberwiseClone();
}