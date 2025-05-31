using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.ProjectManagers;
using HeatManager.Core.Services.ResourceManagers;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.Core.Services.Optimizers;
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

    private readonly IResourceManager _resourceManager;
    private readonly Mock<IResourceManager> _mockResourceManager;

    private readonly ISourceDataProvider _sourceDataProvider;
    private readonly Mock<ISourceDataProvider> _mockSourceDataProvider;

    private readonly IOptimizer _optimizer;
    private readonly Mock<IOptimizer> _mockOptimizer;

    public ProjectManagerTest()
    {
        _mockSourceDataProvider = new Mock<ISourceDataProvider>();
        _mockOptimizer = new Mock<IOptimizer>();

        _mockSourceDataProvider.SetupProperty(m => m.SourceDataCollection);

        _sourceDataProvider = _mockSourceDataProvider.Object;
        _optimizer = _mockOptimizer.Object;

        var testResource = new Resource("Electricity");

        var obs = new ObservableCollection<ProductionUnitBase>
        {
            new ElectricityProductionUnit
            {
                Name = "TestElectricitySource",
                Resource = testResource
            },
            new HeatProductionUnit()
            {
                Name ="TestHeatSource",
                Resource = testResource
            }
        };

        _mockAssetManager = new Mock<IAssetManager>();

        var heatSources = new ObservableCollection<HeatProductionUnit>(); 
        _mockAssetManager
            .Setup(a => a.ProductionUnits)
            .Returns(obs); 

        _mockAssetManager
            .Setup(a => a.HeatProductionUnits)
            .Returns(heatSources); 

        _assetManager = _mockAssetManager.Object;



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
        var projectManager = new ProjectManager(_dbContext, _assetManager, _resourceManager, _sourceDataProvider, _optimizer);

        // Act
        await projectManager.NewProjectAsync("Test");

        // Assert

        projectManager.CurrentProject.ShouldNotBeNull();

        _mockAssetManager.Verify(a => a.ProductionUnits, Times.AtLeast(2));
        
        _mockResourceManager.Verify(r => r.Resources, Times.Exactly(2));

        _mockSourceDataProvider.VerifySet(s => s.SourceDataCollection = It.IsAny<SourceDataCollection>(), Times.Once);

        await Verify(projectManager.CurrentProject);
    }


    [Fact]
    public async Task SaveProjectAsync_SavesProjectSuccessfully()
    {
        // Arrange 
        var projectManager = new ProjectManager(_dbContext, _assetManager, _resourceManager, _sourceDataProvider, _optimizer);

        await projectManager.NewProjectAsync("Test");

        var testResource = new Resource("Oil");
        
        var heatUnit = new HeatProductionUnit { Name = "Test", Resource = testResource };
        _assetManager.ProductionUnits.Add(heatUnit);

        _resourceManager.Resources.Add(new Resource("Electricity"));

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
        var projectData = new
        {
            project.Id,
            project.Name,
            project.CreatedAt,
            project.LastOpened,
            ProjectData = new
            {
                ProductionUnits = project.ProjectData.ProductionUnits.Select(u => new
                {
                    u.Name,
                    Resource = new { u.Resource.Name, u.Resource.Type },
                    u.IsActive
                }),
                Resources = project.ProjectData.Resources.Select(r => new { r.Name, r.Type }),
                SourceData = new
                {
                    DataPoints = project.ProjectData.SourceData.DataPoints.Select(d => new
                    {
                        d.TimeFrom,
                        d.TimeTo,
                        d.HeatDemand,
                        d.ElectricityPrice
                    })
                }
            }
        };
        await Verify(projectData);
    }

    [Fact]
    public async Task LoadProjectAsync_LoadsSuccessfully()
    {
        // Arrange 
        var projectManager = new ProjectManager(_dbContext, _assetManager, _resourceManager, _sourceDataProvider, _optimizer);

        await projectManager.NewProjectAsync("Test");

        var testResource = new Resource("Oil");
            
        var heatUnit = new HeatProductionUnit { Name = "Test", Resource = testResource };
        _assetManager.ProductionUnits.Add(heatUnit);

        _resourceManager.Resources.Add(new Resource("Electricity"));

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

        var projectData = new
        {
            projectManager.CurrentProject.Id,
            projectManager.CurrentProject.Name,
            projectManager.CurrentProject.CreatedAt,
            projectManager.CurrentProject.LastOpened,
            ProjectData = new
            {
                ProductionUnits = projectManager.CurrentProject.ProjectData.ProductionUnits.Select(u => new
                {
                    u.Name,
                    Resource = new { u.Resource.Name, u.Resource.Type },
                    u.IsActive
                }),
                Resources = projectManager.CurrentProject.ProjectData.Resources.Select(r => new { r.Name, r.Type }),
                SourceData = new
                {
                    DataPoints = projectManager.CurrentProject.ProjectData.SourceData.DataPoints.Select(d => new
                    {
                        d.TimeFrom,
                        d.TimeTo,
                        d.HeatDemand,
                        d.ElectricityPrice
                    })
                }
            }
        };
        await Verify(projectData);
    }

    [Fact]
    public async Task GetProjects_ShouldReturnCorrectProjects()
    {
        // Arrange 
        var projectManager = new ProjectManager(_dbContext, _assetManager, _resourceManager, _sourceDataProvider, _optimizer);

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