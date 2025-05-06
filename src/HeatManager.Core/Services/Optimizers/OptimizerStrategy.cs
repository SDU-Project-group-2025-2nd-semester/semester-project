namespace HeatManager.Core.Services.Optimizers;

/// <summary>
/// Enum representing different types of optimization strategies.
/// </summary>
public enum OptimizationType
{
    PriceOptimization,
    Co2Optimization, 
    BalancedOptimization
}

/// <summary>
/// Interface defining the strategy for the optimizer.
/// </summary>
public interface IOptimizerStrategy
{
    OptimizationType Optimization { get; }
}

/// /// <summary>
/// Class implementing the optimizer strategy.
/// </summary>
public class OptimizerStrategy : IOptimizerStrategy
{
    public OptimizationType Optimization { get; }
    /// <summary>
    /// Constructor that initializes the optimizer strategy based on the price optimization setting.
    /// </summary>
    /// <param name="priceOptimization">If true, sets the strategy to PriceOptimization; otherwise, sets it to Co2Optimization.</param>
    /// <remarks> This is here just for backward compatibility. </remarks>
    
    [Obsolete("This constructor is just for ensuring the code works before major refactoring. Please use the constructor with OptimizationType parameter instead.")]
    public OptimizerStrategy(bool priceOptimization)
    {
        Optimization = priceOptimization ? OptimizationType.PriceOptimization : OptimizationType.Co2Optimization;
    }
    
    /// <summary>
    /// Constructor that initializes the optimizer strategy based on the provided optimization type.
    /// </summary>
    /// <param name="optimization">The <see cref="OptimizationType"/> to set.</param>
    public OptimizerStrategy(OptimizationType optimization)
    {
        Optimization = optimization;
    }
    
}