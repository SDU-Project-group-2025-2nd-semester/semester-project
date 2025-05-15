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
                var dialog = new DeletionDialog(unit.Name);

                var parentWindow = (Window)this.VisualRoot!;

                await dialog.ShowDialog(parentWindow);

                if (dialog.Confirmed)
                {
                    vm.RemoveUnit(unit);
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

            if (dialog.Confirmed)
            {
                vm.AddUnit(dialog.Unit);
            }
        }
    }
}
