namespace HeatManager.Core.Services.Optimizers;

public class OptimizerSettings : IOptimizerSettings
{
    public Dictionary<string, bool> AllUnits { get; set; }
    
    public OptimizerSettings(List<string> allUnits, Dictionary<string, bool>? initialStates = null)  
    {
        AllUnits = new Dictionary<string, bool>();

        if (initialStates == null)
        {
            return; 
        }
        
        foreach (var unit in allUnits)
        {
            if (initialStates.ContainsKey(unit))
            {
                AllUnits[unit] = initialStates[unit];
            }
        }
    }
    
    public OptimizerSettings(Dictionary<string, bool> activeUnits)
    {
        AllUnits = activeUnits;
    }
    
    public void SetUnitState(string unitName, bool state)
    {
        if (AllUnits.ContainsKey(unitName))
        {
            AllUnits[unitName] = state;
        }
    }
    
    public List<string> GetActiveUnits()
    {
        return AllUnits.Where(x => x.Value).Select(x => x.Key).ToList();
    }
}