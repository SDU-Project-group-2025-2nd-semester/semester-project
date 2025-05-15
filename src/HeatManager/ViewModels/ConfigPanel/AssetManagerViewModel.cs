using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using System.Collections.ObjectModel;

namespace HeatManager.ViewModels.ConfigPanel;

internal class AssetManagerViewModel : ViewModelBase, IAssetManagerViewModel
{
    private readonly IAssetManager _assetManager;
    private readonly IOptimizer _optimizer;
    internal ObservableCollection<ProductionUnitBase> Units => _assetManager.ProductionUnits;

    public AssetManagerViewModel(IAssetManager assetManager, IOptimizer optimizer)
    {
        _assetManager = assetManager;
        _optimizer = optimizer;
    }
    public void RemoveUnit(ProductionUnitBase unit)
    {
        _assetManager.RemoveUnit(unit);
        _optimizer.UpdateProductionUnits(_assetManager);
    }

    public void AddUnit(ProductionUnitBase unit)
    {
        _assetManager.AddUnit(unit);
        _optimizer.UpdateProductionUnits(_assetManager);
    }

}