using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;
using HeatManager.Core.Models;
using HeatManager.Core.Services.AssetManagers;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using System.Linq;
using HeatManager.ViewModels.Overview;

namespace HeatManager.ViewModels.ConfigPanel;

internal class AssetManagerViewModel : ViewModelBase, IAssetManagerViewModel
{
    private readonly IAssetManager _assetManager = new AssetManager();
    public ObservableCollection<CombinedProductionUnit> CombinedUnits { get; }

    public AssetManagerViewModel(ProductionUnitsViewModel productionUnitsViewModel)
    {
        // Call GetCombinedUnits to populate the CombinedUnits collection
        var combinedUnitsFromAssetManager = _assetManager.GetCombinedUnits();

        // Map the CombinedUnits and set the OnToggle property
        CombinedUnits = new ObservableCollection<CombinedProductionUnit>(
            combinedUnitsFromAssetManager.Select(unit =>
            {
                unit.OnToggle = () =>
                {
                    productionUnitsViewModel.RefreshProductionUnits(); // Notify ProductionUnitsViewModel
                    RefreshCombinedUnits();
                };

                return unit;
            })
        );
    }

    public void RefreshCombinedUnits()
    {
        foreach (var unit in CombinedUnits)
        {
            // Update the status shown of each unit based on the current state in ProductionUnitData
            if (ProductionUnitData.Units.AllUnits.TryGetValue(unit.Name, out var isActive))
            {
                unit.IsActive = isActive;
                unit.Status = isActive ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline;
            }
        }
    }
}