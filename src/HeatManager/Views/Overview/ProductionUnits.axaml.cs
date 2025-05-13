using Avalonia.Controls;
using HeatManager.ViewModels.Overview;
using Avalonia.Interactivity;

namespace HeatManager.Views.Overview;

public partial class ProductionUnits : UserControl
{
    public ProductionUnits()
    {
        InitializeComponent();
        DataContext = new ProductionUnitsViewModel();
    }
}