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
        // Load combined units for the UI
        CombinedUnits = new ObservableCollection<CombinedProductionUnit>(
        ProductionUnitData.Units.AllUnits.Select(unit => new CombinedProductionUnit
        {
            Name = unit.Key,
            IsActive = unit.Value,
            Status = unit.Value ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline,
            OnToggle = productionUnitsViewModel.RefreshProductionUnits // Notify ProductionUnitsViewModel
        })
    );
    }
}