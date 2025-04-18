using HeatManager.Core.Models.Producers;
using System.Collections.ObjectModel;

namespace HeatManager.Core.Services.AssetManagers;

public interface IAssetManager
{
    public ObservableCollection<HeatProductionUnit> ProductionUnits { get; }

    public void LoadUnits(string filepath);
}