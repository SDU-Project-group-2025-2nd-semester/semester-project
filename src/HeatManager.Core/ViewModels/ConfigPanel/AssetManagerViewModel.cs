using System.Collections.ObjectModel;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;
using HeatManager.Core.Services.AssetManagers;

namespace HeatManager.Core.ViewModels;

internal class AssetManagerViewModel : ViewModelBase, IAssetManagerViewModel
{   
    private readonly IAssetManager _assetManager = new AssetManager();
    internal ObservableCollection<ProductionUnitBase> Units => _assetManager.ProductionUnits;
}