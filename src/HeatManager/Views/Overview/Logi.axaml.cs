using Avalonia.Controls;
using HeatManager.ViewModels.Overview;

namespace HeatManager.Views.Overview;

public partial class Logi : UserControl
{
    public Logi()
    {
        InitializeComponent();
        DataContext = new LogiViewModel();
    }
}