namespace HeatManager.Core.Models.Schedules;

public interface IResourceConsumptionSchedule
{
    public string Name { get; }

    public double[] Consumption { get; }

    public double TotalConsumption { get; }
}