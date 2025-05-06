using Avalonia.Controls;
using Avalonia.Interactivity;
using HeatManager.ViewModels.Optimizer;
using System;

namespace HeatManager.Views.Optimizer
{
    public partial class DataOptimizerView : UserControl
    {
        public DataOptimizerView()
        {
            InitializeComponent();
            
        }

        private void ToggleView_Click(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("ToggleView_Click");
            if (DataContext is DataOptimizerViewModel vm)
            {
                vm.ToggleView();
            }
        }

    }
}