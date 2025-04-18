using Castle.Components.DictionaryAdapter;
using Castle.Core.Resource;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Projects;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.HeatSourceManager;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.ResourceManagers;
using HeatManager.Core.Services.SourceDataProviders;
using JetBrains.Annotations;
using Moq;
using Shouldly;
using System.Collections.ObjectModel;

namespace HeatManager.Core.Tests.Services;

[TestSubject(typeof(ProjectManager))]
public class ProjectManagerTest : DatabaseAccess
{
    private readonly IAssetManager _assetManager;
    private readonly Mock<IAssetManager> _mockAssetManager;

    private readonly IHeatSourceManager _heatSourceManager;
    private readonly Mock<IHeatSourceManager> _mockHeatSourceManager;

    private readonly IResourceManager _resourceManager;
    private readonly Mock<IResourceManager> _mockResourceManager;

    private readonly ISourceDataProvider _sourceDataProvider;
    private readonly Mock<ISourceDataProvider> _mockSourceDataProvider;

    public ProjectManagerTest()
    {
        _mockSourceDataProvider = new Mock<ISourceDataProvider>();

        _mockSourceDataProvider.SetupProperty(m => m.SourceDataCollection);

        _sourceDataProvider = _mockSourceDataProvider.Object;

        var testResource = new Resource { Name = "TestResource" };

        var obs = new ObservableCollection<HeatProductionUnit>
        {
            new ElectricityProductionUnit
            {
                Name = "TestElectricitySource",
                Resource = testResource
            },
            new()
            {
                Name ="TestHeatSource",
                Resource = testResource
            }
        };

        _mockAssetManager = new Mock<IAssetManager>();

        _mockAssetManager
            .Setup(a => a.ProductionUnits)
            .Returns(obs);

        _assetManager = _mockAssetManager.Object;

        _mockHeatSourceManager = new Mock<IHeatSourceManager>();

        var heatSources = new List<HeatProductionUnit>();

        _mockHeatSourceManager
            .Setup(h => h.HeatSources)
            .Returns(heatSources);

        _heatSourceManager = _mockHeatSourceManager.Object;

        _mockResourceManager = new Mock<IResourceManager>();
        var resources = new List<Resource> { testResource };

        _mockResourceManager
            .Setup(r => r.Resources)
            .Returns(resources);

        _resourceManager = _mockResourceManager.Object;

    }

    [Fact]
    public async Task NewProjectAsync_CreatesProjectSuccessfully()
    {
        // Arrange 
        var projectManager = new ProjectManager(_dbContext, _assetManager, _heatSourceManager, _resourceManager, _sourceDataProvider);

        // Act
        await projectManager.NewProjectAsync("Test");

        // Assert

        projectManager.CurrentProject.ShouldNotBeNull();

        _mockAssetManager.Verify(a => a.ProductionUnits, Times.Exactly(2));

        _mockHeatSourceManager.Verify(h => h.HeatSources, Times.Exactly(2));

        _mockResourceManager.Verify(r => r.Resources, Times.Exactly(2));

        _mockSourceDataProvider.VerifySet(s => s.SourceDataCollection = It.IsAny<SourceDataCollection>(), Times.Once);

        await Verify(projectManager.CurrentProject);
    }


    [Fact]
    public async Task SaveProjectAsync_SavesProjectSuccessfully()
    {
        // Arrange 
        var projectManager = new ProjectManager(_dbContext, _assetManager, _heatSourceManager, _resourceManager, _sourceDataProvider);

        await projectManager.NewProjectAsync("Test");

        var testResource = new Resource();

        _heatSourceManager.HeatSources.AddRange([new HeatProductionUnit { Name = "Test", Resource = testResource }]);

        _assetManager.ProductionUnits.Add(new HeatProductionUnit { Name = "Test", Resource = testResource });

        _resourceManager.Resources.Add(new Resource(){Name = "TestResource"});

        _sourceDataProvider.SourceDataCollection = new SourceDataCollection([
            new SourceDataPoint()
            {
                ElectricityPrice = 10, HeatDemand = 10, TimeFrom = DateTime.UtcNow, TimeTo = DateTime.UtcNow + TimeSpan.FromMinutes(1),
            }
        ]);

        // Act
        await projectManager.SaveProjectAsync();

        var project = await _dbContext.Projects.FindAsync(projectManager.CurrentProject.Id);

        // Assert
        await Verify(project);
    }

    [Fact]

    public async Task LoadProjectAsync_LoadsSuccessfully()
    {
        // Arrange 
        var projectManager = new ProjectManager(_dbContext, _assetManager, _heatSourceManager, _resourceManager, _sourceDataProvider);

        await projectManager.NewProjectAsync("Test");

        var testResource = new Resource();

        _heatSourceManager.HeatSources.AddRange([new HeatProductionUnit { Name = "Test", Resource = testResource }]);

        _assetManager.ProductionUnits.Add(new HeatProductionUnit { Name = "Test", Resource = testResource });

        _resourceManager.Resources.Add(new Resource(){Name = "TestResource"});

        _sourceDataProvider.SourceDataCollection = new SourceDataCollection([
            new SourceDataPoint()
            {
                ElectricityPrice = 10, HeatDemand = 10, TimeFrom = DateTime.UtcNow, TimeTo = DateTime.UtcNow,
            }
        ]);

        await projectManager.SaveProjectAsync();

        // Act

        await projectManager.NewProjectAsync("NewProject");

        var newProject = projectManager.CurrentProject;

        await projectManager.LoadProjectFromDb("Test");

        // Assert
        
        newProject.Name.ShouldBe("NewProject");

        await Verify(projectManager.CurrentProject);
    }

    [Fact]
    public async Task GetProjects_ShouldReturnCorrectProjects()
    {
        // Arrange 
        var projectManager = new ProjectManager(_dbContext, _assetManager, _heatSourceManager, _resourceManager, _sourceDataProvider);

        await projectManager.NewProjectAsync("Test1");

        
        await projectManager.SaveProjectAsync();

        await projectManager.NewProjectAsync("Test2");

        await projectManager.SaveProjectAsync();

        // Act

        var projects = await projectManager.GetProjectsFromDatabaseDisplaysAsync();

        // Assert
        await Verify(projects);
    }

}