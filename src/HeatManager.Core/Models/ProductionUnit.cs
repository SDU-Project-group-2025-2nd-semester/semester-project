using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using HeatManager.Core.ViewModels;

namespace HeatManager.Core.Models;

public class ProductionUnit
{
    public string Name { get; set; }
    public ProductionUnitsViewModel.ProductionUnitStatus Status { get; set; }

    // Get the Bitmap icon based on the Status
    public Bitmap Icon => Status switch
    {
        ProductionUnitsViewModel.ProductionUnitStatus.Active => LoadBitmap("/Assets/Icons/circle-check-solid.png"),
        ProductionUnitsViewModel.ProductionUnitStatus.Standby => LoadBitmap("/Assets/Icons/circle-exclamation-solid.png"),
        ProductionUnitsViewModel.ProductionUnitStatus.Offline => LoadBitmap("/Assets/Icons/circle-xmark-solid.png"),
        _ => throw new System.NotImplementedException(),
    };

    private static Bitmap LoadBitmap(string resourcePath)
    {
        var uri = new Uri($"avares://HeatManager.Core{resourcePath}");
        return new Bitmap(AssetLoader.Open(uri));
    }
}