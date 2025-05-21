using HeatManager.ViewModels.Optimizer;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.SourceDataProviders;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Headless.XUnit;
using JetBrains.Annotations;
using Moq;
using Shouldly;
using System.Collections.ObjectModel;
using HeatManager.Core.ResultData;

namespace HeatManager.Tests.ViewModels;

public class DataOptimizerViewModelTest
{

    private static IOptimizer CreateMinimalWorkingOptimizer()
    {
        var mockAssetManager = new Mock<IAssetManager>();
        var mockSourceDataProvider = new Mock<ISourceDataProvider>();
        var mockOptimizerSettings = new Mock<IOptimizerSettings>();
        var mockOptimizerStrategy = new Mock<IOptimizerStrategy>();

        var oil = new Resource("Oil");

        var unit = new HeatProductionUnit
        {
            Name = "Unit1",
            MaxHeatProduction = 100,
            Cost = 1,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = oil
        };

        var sourceDataPoint = new SourceDataPoint
        {
            HeatDemand = 50,
            ElectricityPrice = 0,
            TimeFrom = new DateTime(2024, 01, 01),
            TimeTo = new DateTime(2024, 01, 01).AddHours(1)
        };

        mockAssetManager.Setup(a => a.ProductionUnits)
            .Returns(new ObservableCollection<ProductionUnitBase> { unit });

        mockSourceDataProvider.Setup(p => p.SourceDataCollection)
            .Returns(new SourceDataCollection([sourceDataPoint]));

        mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames())
            .Returns(new List<string> { "Unit1" });

        mockOptimizerStrategy.Setup(s => s.Optimization)
            .Returns(OptimizationType.PriceOptimization);

        return new DefaultOptimizer(
            mockAssetManager.Object,
            mockSourceDataProvider.Object,
            mockOptimizerSettings.Object,
            mockOptimizerStrategy.Object);
    }


    [AvaloniaFact]
    public void ViewModel_Should_Initialize_Correctly_With_Single_Unit_Schedule()
    {
        // Arrange
        var optimizer = CreateMinimalWorkingOptimizer();

        // Act
        optimizer.Optimize(); 
        var vm = new DataOptimizerViewModel(optimizer);

        // Assert ViewModel state
        vm.SelectedView.ShouldBe(OptimizerViewType.HeatProductionGraph);
        vm.MinDate?.Date.ShouldBe(new DateTime(2024, 01, 01));
        vm.MaxDate?.Date.ShouldBe(new DateTime(2024, 01, 01));
        vm.DateRangeText.ShouldContain("01 Jan. 2024");
    }


    [AvaloniaFact]
    public void Changing_SelectedViewOption_Should_Update_SelectedView()
    {
        // Arrange
        var optimizer = CreateMinimalWorkingOptimizer();
        var vm = new DataOptimizerViewModel(optimizer);

        var summaryOption = vm.ViewOptions.First(v => v.ViewType == OptimizerViewType.SummaryTable);

        // Act
        vm.SelectedViewOption = summaryOption;

        // Assert
        vm.SelectedView.ShouldBe(OptimizerViewType.SummaryTable);
    }



}
