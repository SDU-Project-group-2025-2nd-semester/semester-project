using Avalonia.Controls;
using CsvHelper;
using HeatManager.Core.DataLoader;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.Core.ViewModels;

namespace HeatManager.Core.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var sourceDataProvider = new SourceDataProvider();

            var parser = new CsvDataLoader(sourceDataProvider);

            parser.LoadData("./source-data-csv/winter.csv");    

            var assetManager = new AssetManager();
            
            
            var optimizerSettings = new OptimizerSettings(assetManager.ProductionUnits.Select(u => u.Name).ToList());
            
            optimizerSettings.SetActive("GB1");
            optimizerSettings.SetActive("GB2");
            optimizerSettings.SetActive("OB1");
            
            var optimizer = new DefaultOptimizer( assetManager, sourceDataProvider, optimizerSettings,new OptimizerStrategy(true), new object());
            
            DataContext = new MainWindowViewModel(sourceDataProvider, optimizer);
        }
    }
}