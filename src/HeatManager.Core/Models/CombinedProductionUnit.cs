using Avalonia.Media.Imaging;
using Avalonia.Platform;
using HeatManager.Core.Models.Producers;
using System;
using System.ComponentModel;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Services;

namespace HeatManager.Core.Models;

// Combining data from the JSON file (ProductionUnits.json) and the Units object (ProductionUnitData.cs)
// for the AssetManagerView
public class CombinedProductionUnit : INotifyPropertyChanged
{
    public ProductionUnitBase Unit { get; set;  }
    //public string Name { get; set; } = "default";
    private ProductionUnitStatus status;
    public ProductionUnitStatus Status
    {
        get => status;
        set
        {
            if (status != value)
            {
                status = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(Icon));
            }
        }
    }
    //public decimal Cost { get; set; }
   // public double MaxHeatProduction { get; set; }
    //public double Emissions { get; set; }
    //public double ResourceConsumption { get; set; }
    //public Resource? Resource { get; set; }

    // Dynamically resolve the Bitmap icon based on the Status
    public Bitmap Icon => Status switch
    {
        ProductionUnitStatus.Active  => LoadBitmap("/Assets/Icons/circle-check-solid.png"),
        ProductionUnitStatus.Standby => LoadBitmap("/Assets/Icons/circle-exclamation-solid.png"),
        ProductionUnitStatus.Offline => LoadBitmap("/Assets/Icons/circle-xmark-solid.png"),
        _ => throw new NotImplementedException(),
    };

    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set
        {
            if (isActive != value)
            {
                isActive = value;

                // Directly update the Units object in ProductionUnitData
                if (isActive)
                {
                    ProductionUnitData.Units.SetActive(Unit.Name);
                }
                else
                {
                    ProductionUnitData.Units.SetOffline(Unit.Name);
                }

                // Notify the ViewModel to refresh the ProductionUnits collection
                OnToggle?.Invoke();
                
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(Icon));
            }
        }
    }
    
    // Delegate to notify the ViewModel
    public Action? OnToggle { get; set; }
    
    private static Bitmap LoadBitmap(string resourcePath)
    {
        var uri = new Uri($"avares://HeatManager{resourcePath}");
        return new Bitmap(AssetLoader.Open(uri));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}