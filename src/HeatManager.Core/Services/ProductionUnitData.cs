using HeatManager.Core.Models;
using HeatManager.Core.Services.Optimizers;
using System.Collections.Generic;

namespace HeatManager.Core.Services;

public static class ProductionUnitData
{
    // Initialize the dictionary of units with their active/inactive states
    public static OptimizerSettings Units { get; } = new OptimizerSettings(new Dictionary<string, bool>
    {
        { "GB1", true },
        { "GB2", true },
        { "OB1", true },
        { "GM1", false },
        { "HP1", false }
    });

    // Method to get all ProductionUnit objects for the UI
    public static List<ProductionUnit> GetProductionUnits()
    {
        var productionUnits = new List<ProductionUnit>();
        foreach (var unit in Units.AllUnits)
        {
            productionUnits.Add(new ProductionUnit
            {
                Name = unit.Key,
                Status = unit.Value ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline
            });
        }
        return productionUnits;
    }

    public static void SetAllUnitsActive()
    {
        foreach (var unitName in Units.AllUnits.Keys)
        {
            Units.SetActive(unitName); 
        }
    }

    public static void SetUnitsBackToDefault()
    {
        Units.SetActive("GB1");
        Units.SetActive("GB2");
        Units.SetActive("OB1");
        Units.SetOffline("GM1");
        Units.SetOffline("HP1");
    }
}