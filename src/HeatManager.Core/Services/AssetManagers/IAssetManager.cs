using HeatManager.Core.Models.Producers;
using System.Collections.ObjectModel;

namespace HeatManager.Core.Services.AssetManagers;

public interface IAssetManager
{
    public ObservableCollection<HeatProductionUnitBase> ProductionUnits { get; }

    public void LoadUnits(string filepath);
}