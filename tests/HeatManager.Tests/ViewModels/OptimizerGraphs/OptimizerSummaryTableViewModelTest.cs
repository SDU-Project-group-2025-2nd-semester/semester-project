using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.ResultData;
using HeatManager.ViewModels.OptimizerGraphs;
using JetBrains.Annotations;
using Moq;
using Shouldly;

namespace HeatManager.Tests.ViewModels;

[TestSubject(typeof(OptimizerSummaryTableViewModel))]
public class OptimizerSummaryTableViewModelTest
{

    [Fact]
    public void Constructor_Should_Build_Correct_TableData()
    {
        // Arrange
        var schedule = new HeatProductionUnitSchedule("UnitX", ResourceType.Gas);

        var mockPoint1 = new Mock<IHeatProductionUnitResultDataPoint>();
        mockPoint1.Setup(p => p.TimeFrom).Returns(new DateTime(2025, 1, 1, 0, 0, 0));
        mockPoint1.Setup(p => p.TimeTo).Returns(new DateTime(2025, 1, 1, 1, 0, 0));
        mockPoint1.Setup(p => p.HeatProduction).Returns(100);
        mockPoint1.Setup(p => p.Emissions).Returns(10);
        mockPoint1.Setup(p => p.Cost).Returns(50m);
        mockPoint1.Setup(p => p.ResourceConsumption).Returns(20);
        mockPoint1.Setup(p => p.Utilization).Returns(0.75);

        var mockPoint2 = new Mock<IHeatProductionUnitResultDataPoint>();
        mockPoint2.Setup(p => p.TimeFrom).Returns(new DateTime(2025, 1, 1, 1, 0, 0));
        mockPoint2.Setup(p => p.TimeTo).Returns(new DateTime(2025, 1, 1, 2, 0, 0));
        mockPoint2.Setup(p => p.HeatProduction).Returns(200);
        mockPoint2.Setup(p => p.Emissions).Returns(15);
        mockPoint2.Setup(p => p.Cost).Returns(70m);
        mockPoint2.Setup(p => p.ResourceConsumption).Returns(30);
        mockPoint2.Setup(p => p.Utilization).Returns(0.85);

        schedule.AddDataPoint(mockPoint1.Object);
        schedule.AddDataPoint(mockPoint2.Object);

        var schedules = new List<HeatProductionUnitSchedule> { schedule };

        // Act
        var vm = new OptimizerSummaryTableViewModel(schedules);

        // Assert
        vm.TableData.Count.ShouldBe(1);
        var row = vm.TableData.First();

        row.Name.ShouldBe("UnitX");
        row.HeatProduction.ShouldBe(300.000m);                 // 100 + 200
        row.MaxHeatProduction.ShouldBe(200.000m);              // max(100, 200)
        row.Emissions.ShouldBe(25.000m);                       // 10 + 15
        row.MaxEmissions.ShouldBe(15.000m);                    // max(10, 15)
        row.Cost.ShouldBe(120.00m);                            // 50 + 70
        row.MaxCost.ShouldBe(70.00m);                          // max(50, 70)
        row.ResourceConsumption.ShouldBe(50.000m);             // 20 + 30
        row.MaxResourceConsumption.ShouldBe(30.000m);          // max(20, 30)
        row.Utilization.ShouldBe(1.600m);                      // 0.75 + 0.85
        row.MaxUtilization.ShouldBe(0.850m);                   // max(0.75, 0.85)
    }

}