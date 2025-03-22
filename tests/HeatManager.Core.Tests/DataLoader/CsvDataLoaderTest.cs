using CsvHelper;
using CsvHelper.Configuration;
using HeatManager.Core.DataLoader;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services;
using HeatManager.Core.Services.Optimizers;
using JetBrains.Annotations;
using Moq;
using Xunit;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HeatManager.Core.Tests.DataLoader;

[TestSubject(typeof(CsvDataLoader))]
public class CsvDataLoaderTests
{
    public IResourceManager ResourceManager { get; }

    public CsvDataLoaderTests()
    {
        var mockObject = new Mock<IResourceManager>();

        //var anotherMock = new Mock<IOptimizer>();
        //anotherMock
        //    .Setup(om => om.OptimizeAsync(It.<>()))
        //    .Returns(new List<IBasicResource>());


        mockObject
            .Setup(rm => rm.Resources)
            .Returns(new List<IBasicResource>() { });

        ResourceManager = mockObject.Object;
    }

    [Fact]
    public void LoadData_ShouldParseCsvAndStoreInSourceDataProvider()
    {
        // Arrange
        var mockSourceDataProvider = new Mock<ISourceDataProvider>();
        var dataLoader = new CsvDataLoader(mockSourceDataProvider.Object);
    
        // Act
        dataLoader.LoadData("mockFile.csv");  
        // Assert
        // Verify that SetDataCollection was called with the correct data
        mockSourceDataProvider.Verify(
            m => m.SetDataCollection(It.Is<SourceDataCollection>(data => data.DataPoints.Count == 4)),
            Times.Once); // We expect 4 records in the CSV
    }
    [Fact]
    public void LoadData_ShouldParseCsvAndStoreMockRecords()
    {
        // Arrange
        var mockSourceDataProvider = new Mock<ISourceDataProvider>();
        var dataLoader = new CsvDataLoader(mockSourceDataProvider.Object);

        // Mock expected records
        var mockRecords = new List<ISourceDataPoint>
        {
            new SourceDataPoint
            {
                TimeFrom = new DateTime(2024, 8, 11, 0, 0, 0),
                TimeTo = new DateTime(2024, 8, 11, 1, 0, 0),
                HeatDemand = 1.79,
                ElectricityPrice = 752.03m
            },
            new SourceDataPoint
            {
                TimeFrom = new DateTime(2024, 8, 11, 1, 0, 0),
                TimeTo = new DateTime(2024, 8, 11, 2, 0, 0),
                HeatDemand = 1.85,
                ElectricityPrice = 691.05m
            },
            new SourceDataPoint
            {
                TimeFrom = new DateTime(2024, 8, 11, 2, 0, 0),
                TimeTo = new DateTime(2024, 8, 11, 3, 0, 0),
                HeatDemand = 1.76,
                ElectricityPrice = 674.78m
            },
            new SourceDataPoint
            {
                TimeFrom = new DateTime(2024, 8, 11, 3, 0, 0),
                TimeTo = new DateTime(2024, 8, 11, 4, 0, 0),
                HeatDemand = 1.67,
                ElectricityPrice = 652.95m
            }
        };

        // Act
        dataLoader.LoadData("mockFile.csv"); 

        // Assert
        // Verify that SetDataCollection was called with the correct data
        mockSourceDataProvider.Verify(m => m.SetDataCollection(It.Is<SourceDataCollection>(collection =>
            collection.DataPoints.SequenceEqual(mockRecords, new SourceDataPointEqualityComparer()))), Times.Once);
    }

// Custom EqualityComparer to compare SourceDataPoint objects
public class SourceDataPointEqualityComparer : IEqualityComparer<ISourceDataPoint>
{
    public bool Equals(ISourceDataPoint x, ISourceDataPoint y)
    {
        return x.TimeFrom == y.TimeFrom &&
               x.TimeTo == y.TimeTo &&
               x.HeatDemand == y.HeatDemand &&
               x.ElectricityPrice == y.ElectricityPrice;
    }

    public int GetHashCode(ISourceDataPoint obj)
    {
        return obj.GetHashCode();
    }
}

}