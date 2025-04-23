namespace HeatManager.Core.Services.Optimizers;

/// <summary> 
/// Interface defining the settings for an optimizer.
/// </summary> 
public interface IOptimizerSettings
{
    /// <summary>
    /// A dictionary containing all units and their active status.
    /// The key is the unit name, and the value is a boolean indicating whether the unit is active.
    /// </summary>
    Dictionary<string, bool> AllUnits { get; set; }
    
    /// <summary>
    /// Retrieves the names of all active units.
    /// </summary>
    /// <returns>A list of strings representing the names of active units.</returns>
    List<string> GetActiveUnitsNames();
}


/// <summary> 
/// Implementation of IOptimizerSettings.
/// </summary> 
public class OptimizerSettings : IOptimizerSettings
{
    /// <summary>
    /// A dictionary containing all units and their active status.
    /// The key is the unit name, and the value is a boolean indicating whether the unit is active.
    /// </summary>
    public Dictionary<string, bool> AllUnits { get; set; }
    
    /// <summary>
    /// Constructor that initializes the optimizer settings with a list of unit names.
    /// All units are initially set to inactive (false).
    /// </summary>
    /// <param name="unitsNames">A list of unit
    public OptimizerSettings(List<string> unitsNames)  
    {
        AllUnits = new Dictionary<string, bool>(); 
        foreach (var unit in unitsNames) 
        {
            AllUnits.Add(unit, false); 
        } 
    }
    
    /// <summary>
    /// Constructor that initializes the optimizer settings with a dictionary of active units.
    /// </summary>
    /// <param name="activeUnits">A dictionary where the key is the unit name and the value is a
    /// boolean indicating its active status.</param>
    public OptimizerSettings(Dictionary<string, bool> activeUnits)
    {
        AllUnits = activeUnits;
    }
    
    /// <summary>
    /// Retrieves the names of all active units.
    /// </summary>
    /// <returns>A list of strings representing the names of active units.</returns>
    public List<string> GetActiveUnitsNames()
    {
        return AllUnits.Where(x => x.Value).Select(x => x.Key).ToList();
    }
    
    /// <summary>
    /// Sets a specific unit to active status.
    /// </summary>
    /// <param name="name">The name of the unit to activate.</param>
    public void SetActive(string name)
    {
        if (AllUnits.ContainsKey(name))
        {
            AllUnits[name] = true;
        }
    }
    
    /// <summary>
    /// Sets a specific unit to inactive status.
    /// </summary>
    /// <param name="name">The name of the unit to disable.</param>
    public void SetInactive(string name)
    {
        if (AllUnits.ContainsKey(name))
        {
            AllUnits[name] = false;
        }
    }

    /// <summary>
    /// Changes the status of the unit (from active to inactive and vice versa).
    /// </summary>
    /// <param name="name">The name of the unit to toggle.</param>
    public void ToggleStatus(string name)
    {
        if (AllUnits.TryGetValue(name, out bool value))
        {
            AllUnits[name] = !value;
        }
    }
}