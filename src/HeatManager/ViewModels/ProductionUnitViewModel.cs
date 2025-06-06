using Avalonia.Media.Imaging;
using Avalonia.Platform;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Resources;
using System;
using System.ComponentModel;

namespace HeatManager.ViewModels;

public class ProductionUnitViewModel : INotifyPropertyChanged
{
    private readonly ProductionUnitBase _unit;
    public ProductionUnitBase ProductionUnit => _unit;
    public string Name => _unit.Name;
    public decimal Cost => _unit.Cost;
    public double Emissions => _unit.Emissions;
    public double MaxHeatProduction => _unit.MaxHeatProduction;
    public double MaxElectricityProduction
    {
        get
        {
            if (_unit is ElectricityProductionUnit electricityUnit)
            {
                return electricityUnit.MaxElectricity;
            }
            return 0;
        }
    }
    public ResourceType ResourceType => _unit.Resource.Type;
    public double ResourceConsumption => _unit.ResourceConsumption;

    public Bitmap Icon => IsActive 
        ? LoadBitmap("/Assets/Icons/circle-check-solid.png")
        : LoadBitmap("/Assets/Icons/circle-xmark-solid.png");

    public bool IsActive
    {
        get => _unit.IsActive;
        set
        {
            if (_unit.IsActive != value)
            {
                _unit.IsActive = value;
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(Icon));
                OnToggle?.Invoke();
            }
        }
    }

    public ProductionUnitViewModel(ProductionUnitBase unit)
    {
        _unit = unit;
        _unit.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ProductionUnitBase.IsActive))
            {   
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(Icon));
            }
        };

        OnPropertyChanged(nameof(IsActive));
        OnPropertyChanged(nameof(Icon));
    }

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

    // Delegate to notify when unit state changes
    public Action? OnToggle { get; set; }
} 