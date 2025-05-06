using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services.AssetManagers;
using System.Collections.ObjectModel;

namespace HeatManager.ViewModels.ConfigPanel;

internal class AssetManagerViewModel : ViewModelBase, IAssetManagerViewModel
{   
    private readonly IAssetManager _assetManager = new AssetManager();
    internal ObservableCollection<ProductionUnitBase> Units => _assetManager.ProductionUnits;
}