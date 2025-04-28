using Avalonia.Controls;
using HeatManager.ViewModels.Overview;

namespace HeatManager.Views.Overview;

public partial class Map : UserControl
{
    public Map()
    {
        InitializeComponent();
        DataContext = new MapViewModel();
    }
}