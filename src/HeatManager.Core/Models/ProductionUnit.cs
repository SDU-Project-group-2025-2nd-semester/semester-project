using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.ComponentModel;
using HeatManager.Core.Services;

namespace HeatManager.Core.Models;

public class ProductionUnit : INotifyPropertyChanged
{
    public string Name { get; set; } = "default";
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

    // Get the Bitmap icon based on the Status
    public Bitmap Icon => Status switch
    {
        ProductionUnitStatus.Active  => LoadBitmap("/Assets/Icons/circle-check-solid.png"),
        ProductionUnitStatus.Standby => LoadBitmap("/Assets/Icons/circle-exclamation-solid.png"),
        ProductionUnitStatus.Offline => LoadBitmap("/Assets/Icons/circle-xmark-solid.png"),
        _ => throw new System.NotImplementedException(),
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
                Status = isActive ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline;

                // Update the state in ProductionUnitData
                if (isActive)
                {
                    ProductionUnitData.Units.SetActive(Name);
                }
                else
                {
                    ProductionUnitData.Units.SetOffline(Name);
                }

                // Notify the ViewModel to refresh the ProductionUnits collection
                OnToggle?.Invoke();

                OnPropertyChanged(nameof(IsActive));
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