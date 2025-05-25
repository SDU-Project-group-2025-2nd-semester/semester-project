using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models;
using HeatManager.Core.Services;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeatManager.ViewModels.Overview;

public partial class ProductionUnitsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ProductionUnitViewModel>? productionUnits;
    
    private readonly IAssetManager _assetManager;
    private readonly IOptimizer _optimizer;

    [ObservableProperty]
    private bool isScenario1Selected;

    // Static property to persist the selected scenario state
    public static bool IsScenario1SelectedState { get; set; } = false;

    public ProductionUnitsViewModel(IAssetManager assetManager, IOptimizer optimizer)
    { 
        _assetManager = assetManager;
        _optimizer = optimizer;
        // Set the scenario state without resetting the units
        IsScenario1Selected = IsScenario1SelectedState;
        RefreshProductionUnits();
    }

    partial void OnIsScenario1SelectedChanged(bool value)
    {
        // Only load scenarios if the user explicitly changes the selection
        if (value != IsScenario1SelectedState)
        {
            // Persist the state
            IsScenario1SelectedState = value;
            
            if (value) // If Scenario 1 is selected
            {
                LoadScenario1();
            }
            else // If Scenario 2 is selected
            {
                LoadScenario2();
            }
        }
    }

    public void LoadScenario1()
    {
        var units = _assetManager.ProductionUnits;
        foreach (var unit in units)
        {
            unit.IsActive = unit.Name switch
            {
                "GB1" or "GB2" or "OB1" => true,
                _ => false
            };
        }
        RefreshProductionUnits();
    }

    public void LoadScenario2()
    {
        var units = _assetManager.ProductionUnits;
        foreach (var unit in units)
        {
            unit.IsActive = unit.Name switch
            {
                "GB1" or "GB2" or "OB1" or "GM1" or "HP1" => true,
                _ => false
            };
        }
        RefreshProductionUnits();
    }
    
    public void RefreshProductionUnits()
    {
        var units = _assetManager.ProductionUnits;
        ProductionUnits = new ObservableCollection<ProductionUnitViewModel>(units.Select(u => new ProductionUnitViewModel(u)));
    }
}