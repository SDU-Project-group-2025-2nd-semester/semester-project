using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services.AssetManagers;
using System.Collections.ObjectModel;

namespace HeatManager.ViewModels.ConfigPanel;

internal class AssetManagerViewModel(IAssetManager assetManager) : ViewModelBase, IAssetManagerViewModel
{   

    internal ObservableCollection<ProductionUnitBase> Units => assetManager.ProductionUnits;
}