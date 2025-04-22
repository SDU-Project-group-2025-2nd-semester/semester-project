namespace HeatManager.Core.Services.Optimizers;

public class OptimizerSettings : IOptimizerSettings
{
    public Dictionary<string, bool> AllUnits { get; set; }
    
    public OptimizerSettings(List<string> unitsNames)  
    {
        AllUnits = new Dictionary<string, bool>(); 
        foreach (var unit in unitsNames) 
        {
            AllUnits.Add(unit, false); 
        } 
    }
    
    public OptimizerSettings(Dictionary<string, bool> activeUnits)
    {
        AllUnits = activeUnits;
    }
    
    public List<string> GetActiveUnitsNames()
    {
        return AllUnits.Where(x => x.Value).Select(x => x.Key).ToList();
    }

    public void SetActive(string name)
    {
        if (AllUnits.ContainsKey(name))
        {
            AllUnits[name] = true;
        }
    }
}