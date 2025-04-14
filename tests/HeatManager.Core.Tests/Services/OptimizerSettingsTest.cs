using System.Collections.Generic;
using HeatManager.Core.Services.Optimizers;
using Xunit;

namespace HeatManager.Core.Tests.Services;

public class OptimizerSettingsTest
{
    [Fact]
    public void Constructor_WithAllUnitsAndInitialStates_SetsCorrectValues()
    {
        // Arrange
        var allUnits = new List<string> { "Unit1", "Unit2", "Unit3" };
        var initialStates = new Dictionary<string, bool>
        {
            { "Unit1", true },
            { "Unit2", false },
            { "Unit3", true }
        };

        // Act
        var settings = new OptimizerSettings(allUnits, initialStates);

        // Assert
        Assert.Equal(3, settings.AllUnits.Count);
        Assert.True(settings.AllUnits["Unit1"]);
        Assert.False(settings.AllUnits["Unit2"]);
        Assert.True(settings.AllUnits["Unit3"]);
    }

    [Fact]
    public void Constructor_WithAllUnitsAndNoInitialStates_InitializesAllUnitsAsFalse()
    {
        // Arrange
        var allUnits = new List<string> { "Unit1", "Unit2", "Unit3" };

        // Act
        var settings = new OptimizerSettings(allUnits);

        // Assert
        Assert.Equal(3, settings.AllUnits.Count);
        Assert.False(settings.AllUnits["Unit1"]);
        Assert.False(settings.AllUnits["Unit2"]);
        Assert.False(settings.AllUnits["Unit3"]);
    }

    [Fact]
    public void Constructor_WithActiveUnits_SetsCorrectValues()
    {
        // Arrange
        var activeUnits = new Dictionary<string, bool>
        {
            { "Unit1", true },
            { "Unit2", false },
            { "Unit3", true }
        };

        // Act
        var settings = new OptimizerSettings(activeUnits);

        // Assert
        Assert.Equal(3, settings.AllUnits.Count);
        Assert.True(settings.AllUnits["Unit1"]);
        Assert.False(settings.AllUnits["Unit2"]);
        Assert.True(settings.AllUnits["Unit3"]);
    }

    [Fact]
    public void GetActiveUnits_ReturnsOnlyActiveUnits()
    {
        // Arrange
        var activeUnits = new Dictionary<string, bool>
        {
            { "Unit1", true },
            { "Unit2", false },
            { "Unit3", true }
        };
        var settings = new OptimizerSettings(activeUnits);

        // Act
        var result = settings.GetActiveUnits();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("Unit1", result);
        Assert.Contains("Unit3", result);
        Assert.DoesNotContain("Unit2", result);
    }

    [Fact]
    public void GetActiveUnits_WhenNoActiveUnits_ReturnsEmptyList()
    {
        // Arrange
        var activeUnits = new Dictionary<string, bool>
        {
            { "Unit1", false },
            { "Unit2", false },
            { "Unit3", false }
        };
        var settings = new OptimizerSettings(activeUnits);

        // Act
        var result = settings.GetActiveUnits();

        // Assert
        Assert.Empty(result);
    }
} 