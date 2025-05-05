using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models;
using HeatManager.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HeatManager.ViewModels.Overview;

public partial class ProductionUnitsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ProductionUnit> productionUnits;

    public ProductionUnitsViewModel()
    {
        // Load production units from the shared ProductionUnitData class
        ProductionUnits = new ObservableCollection<ProductionUnit>(ProductionUnitData.GetProductionUnits());
    }
}