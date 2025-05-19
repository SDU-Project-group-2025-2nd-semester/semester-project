using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;
using HeatManager.Core.Models;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using System.Linq;
using HeatManager.ViewModels.Overview;

namespace HeatManager.ViewModels.ConfigPanel
{
    /// <summary>
    /// ViewModel for managing production units and coordinating asset manager and optimizer services.
    /// </summary>
    internal class AssetManagerViewModel : ViewModelBase, IAssetManagerViewModel
    {
        private readonly IAssetManager _assetManager;
        private readonly IOptimizer _optimizer;

        /// <summary>
        /// Observable collection of production units managed by the asset manager.
        /// </summary>
        internal ObservableCollection<ProductionUnitBase> Units => _assetManager.ProductionUnits;

        public AssetManagerViewModel(IAssetManager assetManager, IOptimizer optimizer)
        {
            _assetManager = assetManager;
            _optimizer = optimizer;
        }

        /// <summary>
        /// Removes a production unit and updates the optimizer.
        /// </summary>
        public void RemoveUnit(ProductionUnitBase unit)
        {
            _assetManager.RemoveUnit(unit);
            _optimizer.UpdateProductionUnits(_assetManager);
        }

        /// <summary>
        /// Adds a new production unit and updates the optimizer.
        /// </summary>
        public void AddUnit(ProductionUnitBase unit)
        {
            _assetManager.AddUnit(unit);
            _optimizer.UpdateProductionUnits(_assetManager);
        }

        /// <summary>
        /// Edits an existing production unit by replacing it and updating the optimizer.
        /// </summary>
        public void EditUnit(ProductionUnitBase unitBase, ProductionUnitBase unit)
        {
            _assetManager.RemoveUnit(unitBase);
            _assetManager.AddUnit(unit);
            _optimizer.UpdateProductionUnits(_assetManager);
        }
    }
}

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

