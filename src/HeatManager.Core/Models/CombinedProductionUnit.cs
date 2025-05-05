using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using HeatManager.Core.Models.Resources;

namespace HeatManager.Core.Models;

// Combining data from the JSON file (ProductionUnits.json) and the Units object (ProductionUnitData.cs)
// for the AssetManagerView
public class CombinedProductionUnit
{
    public string? Name { get; set; }
    public ProductionUnitStatus Status { get; set; }
    public decimal Cost { get; set; }
    public double MaxHeatProduction { get; set; }
    public double Emissions { get; set; }
    public double ResourceConsumption { get; set; }
    public Resource? Resource { get; set; }

    // Dynamically resolve the Bitmap icon based on the Status
    public Bitmap Icon => Status switch
    {
        ProductionUnitStatus.Active  => LoadBitmap("/Assets/Icons/circle-check-solid.png"),
        ProductionUnitStatus.Standby => LoadBitmap("/Assets/Icons/circle-exclamation-solid.png"),
        ProductionUnitStatus.Offline => LoadBitmap("/Assets/Icons/circle-xmark-solid.png"),
        _ => throw new NotImplementedException(),
    };

    private static Bitmap LoadBitmap(string resourcePath)
    {
        var uri = new Uri($"avares://HeatManager{resourcePath}");
        return new Bitmap(AssetLoader.Open(uri));
    }
}