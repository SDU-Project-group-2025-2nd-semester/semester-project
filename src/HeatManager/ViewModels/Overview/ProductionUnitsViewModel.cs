using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models;
using HeatManager.Core.Services;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeatManager.ViewModels.Overview;

public partial class ProductionUnitsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ProductionUnit>? productionUnits;
    
    private IAssetManager _assetManager;
    private IOptimizer _optimizer;

    [ObservableProperty]
    private bool isScenario1Selected;

    // Static property to persist the selected scenario state
    public static bool IsScenario1SelectedState { get; set; } = true;

    public ProductionUnitsViewModel(IAssetManager assetManager, IOptimizer optimizer)
    { 
        _assetManager = assetManager;
        _optimizer = optimizer;
        // Set the scenario state without resetting the units
        IsScenario1Selected = IsScenario1SelectedState;
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
        var unitNames = _assetManager.ProductionUnits.Select(u => u.Name).ToList();
        var settings = new OptimizerSettings(unitNames);
        settings.SetActive("GB1");
        settings.SetActive("GB2");
        settings.SetActive("OB1");
        
        _optimizer.ChangeOptimizationSettings(settings);
        RefreshProductionUnits();
    }

    public void LoadScenario2()
    {
        var unitNames = _assetManager.ProductionUnits.Select(u => u.Name).ToList();
        var settings = new OptimizerSettings(unitNames);
        settings.SetActive("GB1");
        settings.SetActive("GB2");
        settings.SetActive("OB1");
        settings.SetActive("GM1");
        settings.SetActive("HP1");
        _optimizer.ChangeOptimizationSettings(settings);
        RefreshProductionUnits();
    }

    public void RefreshProductionUnits()
    {
        ProductionUnits = new ObservableCollection<ProductionUnit>(
            ProductionUnitData.GetProductionUnits()
        );
    }
}