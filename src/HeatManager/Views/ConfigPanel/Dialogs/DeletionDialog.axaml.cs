using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace HeatManager.Views.ConfigPanel.Dialogs
{
    public partial class DeletionDialog: Window
    {
        public bool Confirmed { get; private set; }

        public string UnitName { get; }

        public DeletionDialog(string unitName)
        {
            UnitName = unitName;
            InitializeComponent();
            DataContext = this;
        }

        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }

        private void Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }
    }
}
