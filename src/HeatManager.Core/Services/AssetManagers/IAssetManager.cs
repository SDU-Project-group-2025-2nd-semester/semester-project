using HeatManager.Core.Models;
using HeatManager.Core.Models.Producers;
using System.Collections.ObjectModel;

namespace HeatManager.Core.Services.AssetManagers;

public interface IAssetManager
{
    public ObservableCollection<ProductionUnitBase> ProductionUnits { get; }
    public ObservableCollection<HeatProductionUnit> HeatProductionUnits { get; }
    public ObservableCollection<CombinedProductionUnit> GetCombinedUnits(); 

    public void LoadUnits(string filepath);
}