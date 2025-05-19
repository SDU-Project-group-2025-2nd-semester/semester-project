using HeatManager.Core.Models.Producers;

namespace HeatManager.ViewModels.ConfigPanel
{
    /// <summary>
    /// Interface defining operations for managing production units.
    /// </summary>
    internal interface IAssetManagerViewModel
    {
        /// <summary>
        /// Removes the specified production unit.
        /// </summary>
        void RemoveUnit(ProductionUnitBase unit);

        /// <summary>
        /// Adds a new production unit.
        /// </summary>
        void AddUnit(ProductionUnitBase unit);

        /// <summary>
        /// Edits an existing production unit by replacing the original with a new unit.
        /// </summary>
        void EditUnit(ProductionUnitBase unitBase, ProductionUnitBase unit);
    }
}
