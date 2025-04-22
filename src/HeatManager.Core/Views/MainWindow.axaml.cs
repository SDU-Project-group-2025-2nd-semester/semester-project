using Avalonia.Controls;
using CsvHelper;
using HeatManager.Core.DataLoader;
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
            
            DataContext = new MainWindowViewModel(sourceDataProvider);
        }
    }
}