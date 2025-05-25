using Avalonia.Media.Imaging;
using Avalonia.Platform;
using HeatManager.Core.Models;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;
using System;
using System.ComponentModel;

namespace HeatManager.ViewModels;

public partial class ProductionUnitViewModel : ViewModelBase
{
    public ProductionUnitBase ProductionUnit { get; private set; }
    public string Name => ProductionUnit.Name;
    public decimal Cost => ProductionUnit.Cost;
    public double Emissions => ProductionUnit.Emissions;
    public double MaxHeat => ProductionUnit.MaxHeatProduction;

    public double MaxElectricity
    {
        get
        {
            if (ProductionUnit is ElectricityProductionUnit electricityUnit)
            {
                return electricityUnit.MaxElectricity;
            }
            else
            {
                return 0;
            }
        }
    }

    private ProductionUnitStatus _unitStatus; 
    public ProductionUnitStatus UnitStatus
    {
        get => _unitStatus;
        set
        {
            if (_unitStatus != value)
            {
                _unitStatus = value;
                
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(UnitStatus));
                OnPropertyChanged(nameof(Icon));
            }
        }
    }
    
    public Bitmap Icon => UnitStatus switch
    {
        ProductionUnitStatus.Active  => LoadBitmap("/Assets/Icons/circle-check-solid.png"),
        ProductionUnitStatus.Standby => LoadBitmap("/Assets/Icons/circle-exclamation-solid.png"),
        ProductionUnitStatus.Offline => LoadBitmap("/Assets/Icons/circle-xmark-solid.png"),
        _ => throw new System.NotImplementedException(),
    };
    
    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                UnitStatus = _isActive ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline;
                if (_isActive)
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

    public ProductionUnitViewModel(ProductionUnitBase productionUnit)
    {
        ProductionUnit = productionUnit;
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