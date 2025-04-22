using HeatManager.Core.DataLoader;
using HeatManager.Core.Services.SourceDataProviders;
using JetBrains.Annotations;

namespace HeatManager.Core.Tests.DataLoader;

[TestSubject(typeof(CsvDataLoader))]
public class CsvDataLoaderTests
{
    [Fact]
    public async Task LoadData_ShouldParseCsvAndStoreInSourceDataProvider()
    {
        // Arrange
        var sourceDataProvider = new SourceDataProvider();
        var dataLoader = new CsvDataLoader(sourceDataProvider);

        // Act
        dataLoader.LoadData("mockFile.csv");

        // Assert
        await Verify(sourceDataProvider.SourceDataCollection);
    }
    [Fact]
    public async Task LoadData_ShouldParseCsvAndStoreMockRecords()
    {
        // Arrange
        var dataProvider = new SourceDataProvider();
        var dataLoader = new CsvDataLoader(dataProvider);

        // Act
        dataLoader.LoadData("mockFile.csv");

        // Assert
        await Verify(dataProvider.SourceDataCollection);
    }
}