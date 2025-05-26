using CommunityToolkit.Mvvm.ComponentModel;
using HeatManager.Core.Models;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace HeatManager.ViewModels.Overview;

public partial class ProductionUnitsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ProductionUnitViewModel>? productionUnits;
    
    private readonly IAssetManager _assetManager;

    public ProductionUnitsViewModel(IAssetManager assetManager)
    { 
        _assetManager = assetManager;
        
        // Initialize the collection even if empty
        ProductionUnits = new ObservableCollection<ProductionUnitViewModel>();
        
        // Subscribe to collection changes
        _assetManager.ProductionUnits.CollectionChanged += OnProductionUnitsCollectionChanged;

        // Subscribe to individual unit changes
        foreach (var unit in _assetManager.ProductionUnits)
        {
            if (unit is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged += OnUnitPropertyChanged;
            }
        }
        RefreshProductionUnits();
    }
    
    public void RefreshProductionUnits()
    {
        var units = _assetManager.ProductionUnits;
        ProductionUnits = new ObservableCollection<ProductionUnitViewModel>(units.Select(u => new ProductionUnitViewModel(u)));
    }
    
    private void OnProductionUnitsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (ProductionUnitBase unit in e.NewItems!)
            {
                if (unit is INotifyPropertyChanged notifier)
                {
                    notifier.PropertyChanged += OnUnitPropertyChanged;
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (ProductionUnitBase unit in e.OldItems!)
            {
                if (unit is INotifyPropertyChanged notifier)
                {
                    notifier.PropertyChanged -= OnUnitPropertyChanged;
                }
            }
        }

        RefreshProductionUnits();
    }

    private void OnUnitPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProductionUnitBase.IsActive))
        {
            RefreshProductionUnits();
        }
    } 
}