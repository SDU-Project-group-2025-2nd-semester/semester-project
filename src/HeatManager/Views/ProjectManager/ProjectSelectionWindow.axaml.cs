using Avalonia.Controls;

namespace HeatManager;

public partial class ProjectSelectionWindow : Window
{
    public ProjectSelectionWindow(/*IProjectManager projectManager*/)
    {
        InitializeComponent();
        //DataContext = new ProjectSelectionViewModel(projectManager);
    }
}