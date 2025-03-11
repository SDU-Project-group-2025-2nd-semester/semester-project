using HeatManager.Core.DataLoader;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Services;
using HeatManager.Core.Services.Optimizers;
using JetBrains.Annotations;
using Moq;
using Xunit;
using Shouldly;
using System.Collections.Generic;

namespace HeatManager.Core.Tests.DataLoader;

[TestSubject(typeof(CsvDataLoader))]
public class CsvDataLoaderTest
{
    public IResourceManager ResourceManager { get; }

    public CsvDataLoaderTest()
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
    public void METHOD()
    {
        // Arrange

        int expected = 0;

        // Act


        // Assert
        


        expected.ShouldBe(0);
    }
}