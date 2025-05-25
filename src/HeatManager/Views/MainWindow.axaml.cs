using Avalonia;
using Avalonia.Controls;
using HeatManager.Core.DataLoader;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Avalonia.Interactivity;
using Avalonia.Reactive; 

namespace HeatManager.Views
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            
            InitializeComponent();

            //var sourceDataProvider = new SourceDataProvider();

            //var parser = new CsvDataLoader(sourceDataProvider);

            //parser.LoadData("./source-data-csv/winter.csv");    


            //var assetManager = new AssetManager();


            //var optimizerSettings = new OptimizerSettings(assetManager.ProductionUnits.Select(u => u.JustSomeRadndomProperty).ToList());

            //optimizerSettings.SetActive("GB1");
            //optimizerSettings.SetActive("GB2");
            //optimizerSettings.SetActive("OB1");

            //var optimizer = new DefaultOptimizer( assetManager, sourceDataProvider, optimizerSettings,new OptimizerStrategy(true));

            //DataContext = new MainWindowViewModel(sourceDataProvider, optimizer);

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