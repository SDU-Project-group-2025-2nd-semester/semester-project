using Avalonia.Controls;
using HeatManager.Core.ViewModels;

namespace HeatManager.Core.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}