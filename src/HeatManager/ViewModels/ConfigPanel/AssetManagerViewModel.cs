using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using System.Collections.ObjectModel;
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
        private readonly ProductionUnitsViewModel _productionUnitsViewModel;
        public ObservableCollection<ProductionUnitViewModel> ProductionUnitViewModels { get; }

        public AssetManagerViewModel(IAssetManager assetManager, IOptimizer optimizer, ProductionUnitsViewModel productionUnitsViewModel)
        {
            _assetManager = assetManager;
            _optimizer = optimizer;
            _productionUnitsViewModel = productionUnitsViewModel; 
            
            ProductionUnitViewModels = new ObservableCollection<ProductionUnitViewModel>();
            
            var units = _assetManager.ProductionUnits;
            foreach (var unit in units)
            {
                ProductionUnitViewModels.Add(new ProductionUnitViewModel(unit));
            }
        }

        /// <summary>
        /// Removes a production unit and updates the optimizer.
        /// </summary>
        public void RemoveUnit(ProductionUnitBase unit)
        {
            _assetManager.RemoveUnit(unit);
            _optimizer.UpdateProductionUnits(_assetManager);
            RefreshProductionUnitViewModels();
        }

        /// <summary>
        /// Adds a new production unit and updates the optimizer.
        /// </summary>
        public void AddUnit(ProductionUnitBase unit)
        {
            _assetManager.AddUnit(unit);
            _optimizer.UpdateProductionUnits(_assetManager);
            RefreshProductionUnitViewModels();
        }

        /// <summary>
        /// Edits an existing production unit by replacing it and updating the optimizer.
        /// </summary>
        public void EditUnit(ProductionUnitBase unitBase, ProductionUnitBase unit)
        {
            _assetManager.RemoveUnit(unitBase);
            _assetManager.AddUnit(unit);
            _optimizer.UpdateProductionUnits(_assetManager);
            RefreshProductionUnitViewModels();
        }
        
        public void RefreshProductionUnitViewModels()
        {
            Dictionary<string, bool> unitStates = new Dictionary<string, bool>();
            foreach (var viewModel in ProductionUnitViewModels)
            {
                unitStates.Add(viewModel.Name, viewModel.IsActive);
            }
            _optimizer.ChangeOptimizationSettings(new OptimizerSettings(unitStates));
        }
    }
}


