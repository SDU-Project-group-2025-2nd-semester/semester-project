using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Models.Projects;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.ScheduleExporter;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HeatManager.ViewModels.DataExport;

public partial class DataExportViewModel(IAssetManager assetManager, IOptimizer optimizer, IProjectManager projectManager) : ViewModelBase
{
    [ObservableProperty]
    private bool heatProductionSummarized = false;
    [ObservableProperty]
    private bool heatProductionHourly = false;
    [ObservableProperty]
    private bool electricityProductionSummarized = false;
    [ObservableProperty]
    private bool electricityProductionHourly = false;
    [ObservableProperty]
    private bool isExporting = false;
    private string? projectName = projectManager.CurrentProject?.Name;

    private readonly Window _hostWindow;

    [RelayCommand]
    private async Task ExportData()
    {

        Schedule optimizedSchedule = optimizer.Optimize();
        ScheduleExporter exporter = new ScheduleExporter();

        /*var storage = _hostWindow.StorageProvider;


        var file = await _hostWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Pick a folder to save the results"
        }); 
        */
        string? dir = AppDomain.CurrentDomain.BaseDirectory;
        //string? dir =file.TryGetLocalPath();
        //improved nomenclature for the output files to be whatData + projectName
         

        while (dir != null && !Directory.Exists(Path.Combine(dir, "results")))
        {
            if (Directory.GetParent(dir) == null) break;
            dir = Directory.GetParent(dir)?.FullName;
        }

        if (dir == null)
            throw new DirectoryNotFoundException("Could not find the 'results' directory in any parent folder.");

        if (HeatProductionSummarized)
        {
            string SummarizedHeatProductionPath = Path.Combine(dir, "results", "SummarizedHeatProduction_" + projectName + ".csv");
            exporter.ExportScheduleData(SummarizedHeatProductionPath, optimizedSchedule.HeatProductionUnitSchedules);

        }

        if (HeatProductionHourly)
        {
            string HourlyHeatProductionPath = Path.Combine(dir, "results", "HourlyHeatProduction_" + projectName  +".csv");
            exporter.ExportScheduleData(HourlyHeatProductionPath, optimizedSchedule.HeatProduction);
        }

        if (ElectricityProductionSummarized)
        {
            string SummarizedElectricityProductionPath = Path.Combine(dir, "results", "SummarizedElectricityProduction_" + projectName +".csv");
            exporter.ExportScheduleData(SummarizedElectricityProductionPath, optimizedSchedule.ElectricityProductionUnitSchedules);
        }

        if (ElectricityProductionHourly)
        {
            if (optimizedSchedule.ElectricityProduction.Any())
            {
                string HourlyElectricityProductionPath = Path.Combine(dir, "results", "HourlyElectricityProduction_" + projectName + ".csv");
                exporter.ExportScheduleData(HourlyElectricityProductionPath, optimizedSchedule.ElectricityProduction);
            }
        }



    }

    
}
