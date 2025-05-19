using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;
using HeatManager.Core.Models;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.ViewModels;
using HeatManager.ViewModels.ConfigPanel;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using System.Linq;
using HeatManager.ViewModels.Overview;
using System.Collections.Generic;

namespace HeatManager.ViewModels.ConfigPanel
{
    /// <summary>
    /// ViewModel for managing production units and coordinating asset manager and optimizer services.
    /// </summary>
    internal class AssetManagerViewModel : ViewModelBase, IAssetManagerViewModel
    {
        private readonly IAssetManager _assetManager;
        private readonly IOptimizer _optimizer;
        private readonly ProductionUnitsViewModel _productionUnitsViewModel = new ProductionUnitsViewModel();
        public ObservableCollection<ProductionUnitBase> CombinedUnits { get; }

        /// <summary>
        /// Observable collection of production units managed by the asset manager.
        /// </summary>
        internal ObservableCollection<ProductionUnitBase> Units => _assetManager.ProductionUnits;

        public AssetManagerViewModel(IAssetManager assetManager, IOptimizer optimizer)
        {
            _assetManager = assetManager;
            _optimizer = optimizer;
            //_productionUnitsViewModel = productionUnitsViewModel; 
            CombinedUnits = _assetManager.ProductionUnits;
            
            var combinedUnitsFromAssetManager = _assetManager.GetCombinedUnits();
            /*
            CombinedUnits = new ObservableCollection<CombinedProductionUnit>(
                combinedUnitsFromAssetManager.Select(unit =>
                {
                    unit.OnToggle = () =>
                    {
                        _productionUnitsViewModel.RefreshProductionUnits(); // Notify ProductionUnitsViewModel
                        RefreshCombinedUnits();
                    };

                    return unit;
                })
            );*/ 
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
        
        /*
        public void RefreshCombinedUnits()
        {
            Dictionary<string, bool> combinedUnits = new Dictionary<string, bool>();
            foreach (var unit in CombinedUnits)
            {
                combinedUnits.Add(unit.Unit.Name, unit.IsActive);
                // Update the status shown of each unit based on the current state in ProductionUnitData
                if (ProductionUnitData.Units.AllUnits.TryGetValue(unit.Unit.Name, out var isActive))
                {
                    unit.IsActive = isActive;
                    unit.Status = isActive ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline;
                }
                
            }
            _optimizer.ChangeOptimizationSettings(new OptimizerSettings(combinedUnits));
        }*/
    }
}


