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
        }

        private async void MainWindow_Opened(object? sender, System.EventArgs e)
        {
            var context =  (MainWindowViewModel)DataContext;
            context.OpenProjectManagerWindowCommand.Execute(null);
        }
    }
}