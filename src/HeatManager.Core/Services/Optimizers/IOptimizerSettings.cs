namespace HeatManager.Core.Services.Optimizers;

public interface IOptimizerSettings
{
    Dictionary<string, bool> AllUnits { get; set; }
    List<string> GetActiveUnitsNames();
}