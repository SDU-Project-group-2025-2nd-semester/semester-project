using HeatManager.Core.Models.Producers;

namespace HeatManager.Core.Services;

internal class HeatSourceManager : IHeatSourceManager
{
    public IEnumerable<IHeatProductionUnit> HeatSources { get; }

    public void AddHeatSource(IHeatProductionUnit heatProductionUnit)
    {
        throw new NotImplementedException();
    }
}