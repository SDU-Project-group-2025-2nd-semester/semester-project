using HeatManager.Core.Models.Producers;

namespace HeatManager.Core.Services.HeatSourceManager;

internal class HeatSourceManager : IHeatSourceManager
{
    public List<HeatProductionUnit> HeatSources { get; }

    public void AddHeatSource(HeatProductionUnit heatProductionUnit)
    {
        throw new NotImplementedException();
    }
}