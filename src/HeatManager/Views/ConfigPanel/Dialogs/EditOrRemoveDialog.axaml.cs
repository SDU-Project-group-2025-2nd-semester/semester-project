using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HeatManager.Views.ConfigPanel.Dialogs;
public partial class EditOrRemoveDialog : Window
{
    public EditOrRemoveOption SelectedOption { get; private set; } = EditOrRemoveOption.None;
    public string UnitName { get; private set; }

    public EditOrRemoveDialog(string unitName)
    {
        UnitName = unitName ?? throw new ArgumentNullException(nameof(unitName));
        InitializeComponent();
    }

    private void EditButton_Click(object? sender, RoutedEventArgs e)
    {
        SelectedOption = EditOrRemoveOption.Edit;
        Close();
    }

    private void RemoveButton_Click(object? sender, RoutedEventArgs e)
    {
        SelectedOption = EditOrRemoveOption.Remove;
        Close();
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        SelectedOption = EditOrRemoveOption.Cancel;
        Close();
    }

    public enum EditOrRemoveOption
    {
        None,
        Edit,
        Remove,
        Cancel
    }

}
