using HeatManager.Core.Services.Optimizers;

namespace HeatManager.Core.Tests.Services;
/*
 * dotnet test --filter "FullyQualifiedName~OptimizerStrategyTest"
 */
public class OptimizerStrategyTest
{
    [Fact]
    public void Constructor_WithPriceOptimization_SetsPriceOptimization()
    {
        // Act
        var strategy = new OptimizerStrategy(OptimizationType.PriceOptimization);

        // Assert
        Assert.Equal(OptimizationType.PriceOptimization, strategy.Optimization);
    }

    [Fact]
    public void Constructor_WithCo2Optimization_SetsCo2Optimization()
    {
        // Act
        var strategy = new OptimizerStrategy(OptimizationType.Co2Optimization);

        // Assert
        Assert.Equal(OptimizationType.Co2Optimization, strategy.Optimization);
    }

    [Fact]
    public void Constructor_ImplementsIOptimizerStrategy()
    {
        // Act
        var strategy = new OptimizerStrategy(OptimizationType.PriceOptimization);

        // Assert
        Assert.IsAssignableFrom<IOptimizerStrategy>(strategy);
    }
} 