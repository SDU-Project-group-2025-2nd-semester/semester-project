using HeatManager.Core.Models.Producers;

namespace HeatManager.Core.Services;

internal class HeatSourceManager : IHeatSourceManager
{
    public IEnumerable<HeatProductionUnit> HeatSources { get; }

    public void AddHeatSource(HeatProductionUnit heatProductionUnit)
    {
        throw new NotImplementedException();
    }
}