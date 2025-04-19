using HeatManager.Core.Models.Producers;

namespace HeatManager.Core.Services.HeatSourceManager;

public interface IHeatSourceManager
{
    public List<HeatProductionUnit> HeatSources { get; }

    public void AddHeatSource(HeatProductionUnit heatProductionUnit); // TODO: Probably set to something line name or so, since I don't want to expose the whole class

    // TODO: Add method to remove heat source 
}