using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services.AssetManagers;
using System.Collections.ObjectModel;

namespace HeatManager.ViewModels.ConfigPanel;

internal class AssetManagerViewModel : ViewModelBase, IAssetManagerViewModel
{   
    private readonly IAssetManager _assetManager;

    public AssetManagerViewModel(IAssetManager assetManager)
    {
        _assetManager = assetManager;
    }
    internal ObservableCollection<ProductionUnitBase> Units => _assetManager.ProductionUnits;
}