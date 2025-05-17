using HeatManager.Core.Models.Producers;
namespace HeatManager.ViewModels.ConfigPanel;

internal interface IAssetManagerViewModel
{
    public void RemoveUnit(ProductionUnitBase unit);
    public void AddUnit(ProductionUnitBase unit);
    public void EditUnit(ProductionUnitBase unitBase, ProductionUnitBase unit);
}