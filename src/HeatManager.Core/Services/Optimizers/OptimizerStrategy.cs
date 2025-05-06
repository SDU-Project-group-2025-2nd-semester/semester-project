namespace HeatManager.Core.Services.Optimizers;


public enum OptimizationType
{
    PriceOptimization,
    Co2Optimization, 
    BalancedOptimization
}

public interface IOptimizerStrategy
{
    OptimizationType Optimization { get; }
}

public class OptimizerStrategy : IOptimizerStrategy
{
    public OptimizationType Optimization { get; }
    
    public OptimizerStrategy(bool priceOptimization)
    {
        Optimization = priceOptimization ? OptimizationType.PriceOptimization : OptimizationType.Co2Optimization;
    }
    
}