using System;
using Avalonia.Controls;

namespace HeatManager.Views.ConfigPanel.Dialogs
{
    /// <summary>
    /// Dialog to confirm deletion of a unit.
    /// </summary>
    public partial class DeletionDialog : Window
    {
        /// <summary>
        /// Indicates whether the deletion was confirmed by the user.
        /// </summary>
        public bool Confirmed { get; private set; }

        /// <summary>
        /// Name of the unit to be deleted, displayed in the dialog.
        /// </summary>
        public string UnitName { get; }

        /// <summary>
        /// Initializes a new instance of DeletionDialog with the specified unit name.
        /// </summary>
        /// <param name="unitName">Name of the unit to delete.</param>
        public DeletionDialog(string unitName)
        {
            UnitName = unitName ?? throw new ArgumentNullException(nameof(unitName));
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Handler for Cancel button click; closes dialog without confirming deletion.
        /// </summary>
        private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }

        /// <summary>
        /// Handler for Delete button click; confirms deletion and closes dialog.
        /// </summary>
        private void Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }
    }
}