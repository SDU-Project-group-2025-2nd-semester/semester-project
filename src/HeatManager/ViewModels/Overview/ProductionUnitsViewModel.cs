using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models;
using HeatManager.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HeatManager.ViewModels.Overview;

public partial class ProductionUnitsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ProductionUnit>? productionUnits;

    [ObservableProperty]
    private bool isScenario1Selected;

    // Static property to persist the selected scenario state
    public static bool IsScenario1SelectedState { get; set; } = true;

    public ProductionUnitsViewModel()
    { 
        IsScenario1Selected = IsScenario1SelectedState;
        if (IsScenario1Selected)
        {
            LoadScenario1();
        }
        else
        {
            LoadScenario2();
        }
    }

    partial void OnIsScenario1SelectedChanged(bool value)
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

    public void LoadScenario1()
    {
        ProductionUnitData.SetUnitsBackToDefault();
        RefreshProductionUnits();
    }

    public void LoadScenario2()
    {
        ProductionUnitData.SetAllUnitsActive();
        RefreshProductionUnits();
    }

    public void RefreshProductionUnits()
    {
        ProductionUnits = new ObservableCollection<ProductionUnit>(
            ProductionUnitData.GetProductionUnits()
        );
    }
}