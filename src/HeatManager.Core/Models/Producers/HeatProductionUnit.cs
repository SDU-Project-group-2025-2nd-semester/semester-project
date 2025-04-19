using HeatManager.Core.Models.Resources;
using HeatManager.Core.Services.AssetManagers;
using System.Text.Json.Serialization;

namespace HeatManager.Core.Models.Producers;

public class HeatProductionUnit : HeatProductionUnitBase
{
    public new HeatProductionUnit Clone() => (HeatProductionUnit)MemberwiseClone();
}