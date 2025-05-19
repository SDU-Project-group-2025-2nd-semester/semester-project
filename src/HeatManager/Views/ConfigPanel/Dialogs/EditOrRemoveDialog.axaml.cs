using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HeatManager.Views.ConfigPanel.Dialogs
{
    /// <summary>
    /// Dialog allowing the user to choose whether to edit, remove, or cancel action on a unit.
    /// </summary>
    public partial class EditOrRemoveDialog : Window
    {
        /// <summary>
        /// The option selected by the user in this dialog.
        /// </summary>
        public EditOrRemoveOption SelectedOption { get; private set; } = EditOrRemoveOption.None;

        /// <summary>
        /// The name of the unit this dialog is acting upon.
        /// </summary>
        public string UnitName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the EditOrRemoveDialog with the specified unit name.
        /// </summary>
        /// <param name="unitName">Name of the unit to edit or remove.</param>
        public EditOrRemoveDialog(string unitName)
        {
            UnitName = unitName ?? throw new ArgumentNullException(nameof(unitName));
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Handles the Edit button click event.
        /// Sets the selected option to Edit and closes the dialog.
        /// </summary>
        private void EditButton_Click(object? sender, RoutedEventArgs e)
        {
            SelectedOption = EditOrRemoveOption.Edit;
            Close();
        }

        /// <summary>
        /// Handles the Remove button click event.
        /// Sets the selected option to Remove and closes the dialog.
        /// </summary>
        private void RemoveButton_Click(object? sender, RoutedEventArgs e)
        {
            SelectedOption = EditOrRemoveOption.Remove;
            Close();
        }

        /// <summary>
        /// Handles the Cancel button click event.
        /// Sets the selected option to Cancel and closes the dialog.
        /// </summary>
        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            SelectedOption = EditOrRemoveOption.Cancel;
            Close();
        }

        /// <summary>
        /// Enum representing possible user choices in this dialog.
        /// </summary>
        public enum EditOrRemoveOption
        {
            None,
            Edit,
            Remove,
            Cancel
        }
    }
}
