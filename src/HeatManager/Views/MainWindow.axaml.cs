using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Reactive; 
using HeatManager.ViewModels;
using System;

namespace HeatManager.Views
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            
            InitializeComponent();

            this.Opened += MainWindow_Opened;
            this.SizeChanged += MainWindow_SizeChanged;

            // Update sizes based on the current window size
            this.GetObservable(BoundsProperty).Subscribe(new AnonymousObserver<Rect>(bounds =>
            {
                double windowWidth = bounds.Width;

                // Dynamically calculate font sizes
                Application.Current!.Resources["HeadingFontSize"] = Math.Max(16, windowWidth * 0.02); // Minimum 16
                Application.Current.Resources["SubheadingFontSize"] = Math.Max(11, windowWidth * 0.012); 
                Application.Current.Resources["NormalTextFontSize"] = Math.Max(9, windowWidth * 0.009); 

                // Dynamically calculate margin
                double margin = Math.Max(2, windowWidth * 0.009);
                Application.Current!.Resources["BorderMargin"] = new Thickness(margin);

                // Dynamically calculate icon size
                double iconSize = Math.Max(8, windowWidth * 0.014);
                Application.Current!.Resources["IconSize"] = iconSize;
            }));
        }

        private async void MainWindow_Opened(object? sender, System.EventArgs e)
        {
            var context =  (MainWindowViewModel)DataContext;
            context.OpenProjectManagerWindowCommand.Execute(null);
        }

        private void MainWindow_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            // Automatically close the pane if the window width is smaller than 1000
            if (DataContext is MainWindowViewModel context)
            {
                context.IsPaneOpen = e.NewSize.Width >= 1000;
            }
        }
    }
}