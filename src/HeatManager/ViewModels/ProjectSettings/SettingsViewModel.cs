using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models;
using HeatManager.Views.Overview;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatManager.ViewModels.ProjectSettings;
public partial class SettingsViewModel : ViewModelBase
{
    public static readonly Dictionary<ProductionUnitStatus, string> StatusIcons = new()
    {
        { ProductionUnitStatus.Active, "/Assets/Icons/circle-check-solid.png" },   // Green tick
        { ProductionUnitStatus.Standby, "/Assets/Icons/circle-exclamation-solid.png" }, // Yellow exclamation mark 
        { ProductionUnitStatus.Offline, "/Assets/Icons/circle-xmark-solid.png" }  // Red cross 
    };

    [ObservableProperty]
    private ObservableCollection<ProductionUnit> productionUnits;

    public SettingsViewModel()
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

    [ObservableProperty]
    private string projectTitle;

    [ObservableProperty]
    private string projectDescription;

    [ObservableProperty]
    private bool winter;

    [ObservableProperty]
    private bool summer;




}
