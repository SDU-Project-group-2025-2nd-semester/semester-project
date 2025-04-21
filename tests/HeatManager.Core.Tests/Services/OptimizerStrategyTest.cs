using HeatManager.Core.Services.Optimizers;

namespace HeatManager.Core.Tests.Services;
/*
 * dotnet test --filter "FullyQualifiedName~OptimizerStrategyTest"
 */
public class OptimizerStrategyTest
{
    [Fact]
    public void Constructor_WithPriceOptimizationTrue_SetsPriceOptimization()
    {
        // Arrange
        bool priceOptimization = true;

        // Act
        var strategy = new OptimizerStrategy(priceOptimization);

        // Assert
        Assert.Equal(OptimizationType.PriceOptimization, strategy.Optimization);
    }

    [Fact]
    public void Constructor_WithPriceOptimizationFalse_SetsCo2Optimization()
    {
        // Arrange
        bool priceOptimization = false;

        // Act
        var strategy = new OptimizerStrategy(priceOptimization);

        // Assert
        Assert.Equal(OptimizationType.Co2Optimization, strategy.Optimization);
    }

    [Fact]
    public void Constructor_ImplementsIOptimizerStrategy()
    {
        // Arrange
        bool priceOptimization = true;

        // Act
        var strategy = new OptimizerStrategy(priceOptimization);

        // Assert
        Assert.IsAssignableFrom<IOptimizerStrategy>(strategy);
    }
} 