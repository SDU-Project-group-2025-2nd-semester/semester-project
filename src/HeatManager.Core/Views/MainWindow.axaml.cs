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

            parser.LoadData("./source-data-csv/summer.csv");

            var assetManager = new AssetManager();
            
            
            var optimizerSettings = new OptimizerSettings(assetManager.ProductionUnits.Select(u => u.Name).ToList());
            
            foreach (var optimizerSettingsAllUnit in optimizerSettings.AllUnits)
            {
                optimizerSettings.AllUnits[optimizerSettingsAllUnit.Key] = true;
            }
            
            var optimizer = new DefaultOptimizer( assetManager, sourceDataProvider, optimizerSettings,new OptimizerStrategy(true), new object());
            
            DataContext = new MainWindowViewModel(sourceDataProvider, optimizer);
        }
    }
}