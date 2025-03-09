using HeatManager.Core.Models.Producers;

namespace HeatManager.Core.Services;

public interface IHeatSourceManager
{
    public IEnumerable<IHeatProductionUnit> HeatSources { get; }

    public void AddHeatSource(IHeatProductionUnit heatProductionUnit); // TODO: Probably set to something line name or so, since I don't want to expose the whole class

    // TODO: Add method to remove heat source 
}