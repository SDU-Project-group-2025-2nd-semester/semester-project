using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HeatManager.Core.ViewModels;

namespace HeatManager.Core.Views;

public partial class ProductionUnits : UserControl
{
    public ProductionUnits()
    {
        InitializeComponent();
        DataContext = new ProductionUnitsViewModel();
    }
}