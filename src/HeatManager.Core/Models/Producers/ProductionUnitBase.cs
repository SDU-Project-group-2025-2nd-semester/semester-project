using HeatManager.Core.Models.Resources;
using HeatManager.Core.Services.AssetManagers;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HeatManager.Core.Models.Producers;

/// <summary>
/// Base class for all production units in the system.
/// </summary>
public abstract class ProductionUnitBase : INotifyPropertyChanged
{
    private ProductionUnitStatus _unitStatus;
    private bool _isActive;

    public string Name { get; set; } = string.Empty;
    public decimal Cost { get; set; }

    /// <summary>
    /// MW
    /// </summary>
    public double MaxHeatProduction { get; set; }

    /// <summary>
    /// MWh(resource)/MWh(th)
    /// </summary>
    public double ResourceConsumption { get; set; }

    [JsonConverter(typeof(BasicResourceConverter))]
    public Resource? Resource { get; set; }

    /// <summary>
    /// kg/MWh(th)
    /// </summary>
    public double Emissions { get; set; }


    //public virtual double? MaxElectricitySafe => null;

    public ProductionUnitBase Clone() => (ProductionUnitBase)MemberwiseClone();

    public ProductionUnitStatus UnitStatus
    {
        get => _unitStatus;
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                _unitStatus = _isActive ? ProductionUnitStatus.Active : ProductionUnitStatus.Offline;
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(UnitStatus));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 