using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.ScheduleExporter;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HeatManager.ViewModels.DataExporter;

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

    private readonly Window _hostWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow!;

    [RelayCommand]
    private async Task ExportData()
    {

        Schedule optimizedSchedule = optimizer.Optimize();
        ScheduleExporter exporter = new ScheduleExporter();

        var folders = await _hostWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Export Folder",
            AllowMultiple = false
        });

        var folder = folders.FirstOrDefault();

        if (folder == null)
            return;

        var folderPath = folder.TryGetLocalPath();

        if (folderPath == null)
            throw new InvalidOperationException("Could not get the local path of the selected folder.");

        if (folderPath == null)
            throw new DirectoryNotFoundException("Could not find the 'results' directory in any parent folder.");

        if (HeatProductionSummarized)
        {
            string SummarizedHeatProductionPath = Path.Combine(folderPath, "SummarizedHeatProduction_" + projectName + ".csv");
            exporter.ExportScheduleData(SummarizedHeatProductionPath, optimizedSchedule.HeatProductionUnitSchedules);

        }

        if (HeatProductionHourly)
        {
            string HourlyHeatProductionPath = Path.Combine(folderPath, "HourlyHeatProduction_" + projectName  +".csv");
            exporter.ExportScheduleData(HourlyHeatProductionPath, optimizedSchedule.HeatProduction);
        }

        if (ElectricityProductionSummarized)
        {
            string SummarizedElectricityProductionPath = Path.Combine(folderPath, "SummarizedElectricityProduction_" + projectName +".csv");
            exporter.ExportScheduleData(SummarizedElectricityProductionPath, optimizedSchedule.ElectricityProductionUnitSchedules);
        }

        // if (ElectricityProductionHourly)
        // {
        //     if (optimizedSchedule.ElectricityProduction.Any())
        //     {
        //         string HourlyElectricityProductionPath = Path.Combine(folderPath,  "HourlyElectricityProduction_" + projectName + ".csv");
        //         exporter.ExportScheduleData(HourlyElectricityProductionPath, optimizedSchedule.ElectricityProduction);
        //     }
        // }
    }
}