using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using System.Collections.ObjectModel;

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
