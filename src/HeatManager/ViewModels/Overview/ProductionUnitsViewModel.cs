using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HeatManager.ViewModels.Overview;

public partial class ProductionUnitsViewModel : ViewModelBase
{
    

    public static readonly Dictionary<ProductionUnitStatus, string> StatusIcons = new()
    {
        { ProductionUnitStatus.Active, "/Assets/Icons/circle-check-solid.png" },   // Green tick
        { ProductionUnitStatus.Standby, "/Assets/Icons/circle-exclamation-solid.png" }, // Yellow exclamation mark 
        { ProductionUnitStatus.Offline, "/Assets/Icons/circle-xmark-solid.png" }  // Red cross 
    };

    [ObservableProperty]
    private ObservableCollection<ProductionUnit> productionUnits;

    public ProductionUnitsViewModel()
    {
        ProductionUnits = new ObservableCollection<ProductionUnit>
        {
            new ProductionUnit { Name = "GB 1", Status = ProductionUnitStatus.Active },
            new ProductionUnit { Name = "GB 2", Status = ProductionUnitStatus.Active },
            new ProductionUnit { Name = "OB 1", Status = ProductionUnitStatus.Active },
            new ProductionUnit { Name = "GM 1", Status = ProductionUnitStatus.Offline },
            new ProductionUnit { Name = "HP 1", Status = ProductionUnitStatus.Offline }
        };
    }
}