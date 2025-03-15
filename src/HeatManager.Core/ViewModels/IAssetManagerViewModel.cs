using System.Collections.ObjectModel;
using HeatManager.Core.Models.Producers;

namespace HeatManager.Core.ViewModels;

internal interface IAssetManagerViewModel
{
    ObservableCollection<IHeatProductionUnit> ProductionUnits { get; }
}