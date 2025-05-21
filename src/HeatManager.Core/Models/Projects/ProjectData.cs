using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Models.SourceData;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HeatManager.Core.Models.Projects;

public class ProjectData
{

    [JsonIgnore]
    public List<ProductionUnitBase> ProductionUnits { get; set; } = [];

    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<HeatProductionUnit> HeatProductionUnits
    {
        get => ProductionUnits.OfType<HeatProductionUnit>().ToList();
        set => ProductionUnits.AddRange(value);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<ElectricityProductionUnit> ElectricityProduction
    {
        get => ProductionUnits.OfType<ElectricityProductionUnit>().ToList();
        set => ProductionUnits.AddRange(value);
    }


    //public List<HeatProductionUnit> HeatProductionUnits { get; set; } = [];
    public List<Resource> Resources { get; set; } = [];
    public SourceDataCollection? SourceData { get; set; }
}