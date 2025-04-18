using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HeatManager.Core.ViewModels;

namespace HeatManager.Core.Views;

public partial class Map : UserControl
{
    public Map()
    {
        InitializeComponent();
        DataContext = new MapViewModel();
    }
}