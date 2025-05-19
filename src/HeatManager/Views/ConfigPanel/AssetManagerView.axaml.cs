using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using HeatManager.Core.Models;
using HeatManager.Core.Services;
using HeatManager.ViewModels.ConfigPanel;
using HeatManager.ViewModels.Overview;

using Avalonia.Input;
using HeatManager.Core.Models.Producers;
using HeatManager.ViewModels.ConfigPanel;
using HeatManager.Views.ConfigPanel.Dialogs;

namespace HeatManager.Views.ConfigPanel
{
    /// <summary>
    /// View for managing production units (assets).
    /// </summary>
    public partial class AssetManagerView : UserControl
    {
        public AssetManagerView(ProductionUnitsViewModel productionUnitsViewModel)
        {
            InitializeComponent();
        }

        public AssetManagerView() { }

        /// <summary>
        /// Handles the event when a production unit border is clicked.
        /// Shows a dialog allowing the user to choose to edit or remove the unit.
        /// </summary>
        private async void UnitBorder_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Border border && DataContext is AssetManagerViewModel vm)
            {
                if (border.Tag is ProductionUnitBase unit)
                {
                    var parentWindow = (Window)this.VisualRoot!;

                    // Show dialog to select edit or remove option
                    var choiceDialog = new EditOrRemoveDialog(unit.Name);
                    await choiceDialog.ShowDialog(parentWindow);

                    if (choiceDialog.SelectedOption == EditOrRemoveDialog.EditOrRemoveOption.Remove)
                    {
                        // Confirm deletion dialog
                        var deletionDialog = new DeletionDialog(unit.Name);
                        await deletionDialog.ShowDialog(parentWindow);

                        if (deletionDialog.Confirmed)
                            vm.RemoveUnit(unit);
                    }
                    else if (choiceDialog.SelectedOption == EditOrRemoveDialog.EditOrRemoveOption.Edit)
                    {
                        // Show editing dialog
                        var editDialog = new EditingDialog(unit);
                        await editDialog.ShowDialog(parentWindow);

                        if (editDialog.Confirmed && editDialog.Unit != null)
                        {
                            vm.EditUnit(editDialog.UnitBase, editDialog.Unit);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Add New Unit button click.
        /// Opens a dialog to add a new production unit.
        /// </summary>
        private async void AddNewUnit_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is AssetManagerViewModel vm)
            {
                var dialog = new AdditionDialog();
                var parentWindow = (Window)this.VisualRoot!;

                await dialog.ShowDialog(parentWindow);

                if (dialog.Confirmed && dialog.Unit != null)
                {
                    vm.AddUnit(dialog.Unit);
                }
            }
        }
    }
}
