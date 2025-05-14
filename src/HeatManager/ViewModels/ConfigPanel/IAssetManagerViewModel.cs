using HeatManager.Core.Models.Producers;
namespace HeatManager.ViewModels.ConfigPanel;

internal interface IAssetManagerViewModel
{
    public void RemoveUnit(ProductionUnitBase unit);
}