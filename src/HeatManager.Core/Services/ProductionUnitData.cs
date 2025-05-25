using HeatManager.Core.Models;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services.Optimizers;
using System.Collections.Generic;

/*
namespace HeatManager.Core.Services;

public static class ProductionUnitData
{
    // Initialize the dictionary of units with their active/inactive states
    public static IOptimizerSettings Units { get; set; } = new OptimizerSettings();

    public static void UpdateOptimizerSettings(IOptimizer optimizer)
    {
         Units = optimizer.OptimizerSettings;
    }

    // Method to get all ProductionUnit objects for the UI
    public static List<ProductionUnitBase> GetProductionUnits()
    {
        var productionUnits = new List<ProductionUnitBase>();
        foreach (var unit in Units.AllUnits)
        {
            productionUnits.Add(new ProductionUnitBase
            {
                Name = unit.Key,
                UnitStatus = unit.Value ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline
            });
        }
        return productionUnits;
    }

    public static List<ProductionUnitBase> GetProductionUnits(IOptimizer optimizer)
    {
        UpdateOptimizerSettings(optimizer);
        return GetProductionUnits();
    }
    /*
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


}*/ 