using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.ViewModels.ProjectManager;

namespace HeatManager;

public partial class ProjectSelectionWindow : Window
{
    public ProjectSelectionWindow(IProjectManager projectManager)
    {
        InitializeComponent();
        DataContext = new ProjectSelectionViewModel(projectManager, this);
    }
}