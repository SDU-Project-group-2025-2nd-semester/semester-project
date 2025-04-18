using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Models.Projects;

public class ProjectData
{
    public List<HeatProductionUnit> ProductionUnits { get; set; } = [];
    public List<HeatProductionUnit> HeatProductionUnits { get; set; } = [];
    public List<Resource> Resources { get; set; } = [];
    public SourceDataCollection? SourceData { get; set; }
}