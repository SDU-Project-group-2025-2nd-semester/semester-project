using Avalonia.Controls;
using HeatManager.ViewModels.Overview;

namespace HeatManager.Views.Overview;

public partial class ProductionUnits : UserControl
{
    public ProductionUnits()
    {
        InitializeComponent();
        DataContext = new ProductionUnitsViewModel();
    }
}