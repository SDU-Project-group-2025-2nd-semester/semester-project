using Avalonia.Controls;
using Avalonia.Input;
using HeatManager.Core.Models.Producers;
using HeatManager.ViewModels.ConfigPanel;
using HeatManager.Views.ConfigPanel.Dialogs;

namespace HeatManager.Views.ConfigPanel;

public partial class AssetManagerView : UserControl
{
    public AssetManagerView()
    {
        InitializeComponent();
    }

    private async void UnitBorder_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && DataContext is AssetManagerViewModel vm)
        {
            if (border.Tag is ProductionUnitBase unit)
            {
                var parentWindow = (Window)this.VisualRoot!;

                // Show dialog with Edit or Remove options
                var choiceDialog = new EditOrRemoveDialog(unit.Name);
                await choiceDialog.ShowDialog(parentWindow);

                if (choiceDialog.SelectedOption == EditOrRemoveDialog.EditOrRemoveOption.Remove)
                {
                    var deletionDialog = new DeletionDialog(unit.Name);
                    await deletionDialog.ShowDialog(parentWindow);

                    if (deletionDialog.Confirmed)
                        vm.RemoveUnit(unit);
                }
                else if (choiceDialog.SelectedOption == EditOrRemoveDialog.EditOrRemoveOption.Edit)
                {
                    var editDialog = new EditingDialog(unit); // Your edit dialog here
                    await editDialog.ShowDialog(parentWindow);

                    if (editDialog.Confirmed && editDialog.Unit != null)
                    {
                        vm.EditUnit(editDialog.UnitBase, editDialog.Unit);
                    }
                }
            }
        }
    }


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
