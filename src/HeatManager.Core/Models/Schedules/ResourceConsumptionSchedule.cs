namespace HeatManager.Core.Models.Schedules;

public class ResourceConsumptionSchedule
{
    public string Name { get; }

    public double[] Consumption { get; }

    public double TotalConsumption { get; }
}