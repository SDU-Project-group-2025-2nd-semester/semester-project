using System.Collections.ObjectModel;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;

namespace HeatManager.Core.ViewModels;

internal class AssetManagerViewModel : ViewModelBase, IAssetManagerViewModel
{   
    private readonly IAssetManager _assetManager = new AssetManager();
    internal ObservableCollection<IHeatProductionUnit> Units => _assetManager.ProductionUnits;
}