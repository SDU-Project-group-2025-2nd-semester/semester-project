using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;
using HeatManager.Core.Models;
using HeatManager.Core.Services.AssetManagers;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;

namespace HeatManager.ViewModels.ConfigPanel;

internal class AssetManagerViewModel : ViewModelBase, IAssetManagerViewModel
{   
    private readonly IAssetManager _assetManager = new AssetManager();
    public ObservableCollection<CombinedProductionUnit> CombinedUnits { get; }

    public AssetManagerViewModel()
    {
        // Load combined units for the UI
        CombinedUnits = _assetManager.GetCombinedUnits();
    }
}