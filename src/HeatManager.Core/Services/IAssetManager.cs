using System.Collections.ObjectModel;
using HeatManager.Core.Models.Producers;

namespace HeatManager.Core.Services;

public interface IAssetManager
{
    public ObservableCollection<HeatProductionUnit> ProductionUnits { get; }

    public void LoadUnits(string filepath);
}