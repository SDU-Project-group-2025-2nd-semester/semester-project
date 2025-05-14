using HeatManager.Core.Models.Producers;
using System.Collections.ObjectModel;

namespace HeatManager.Core.Services.AssetManagers;

public interface IAssetManager
{
    public ObservableCollection<ProductionUnitBase> ProductionUnits { get; }
    public ObservableCollection<HeatProductionUnit> HeatProductionUnits { get; }

    public void LoadUnits(string filepath);
    public void AddUnit(ProductionUnitBase unit);
    public void RemoveUnit(ProductionUnitBase unit);
}