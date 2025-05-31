namespace HeatManager.Core.Models;

public enum ProductionUnitStatus
{
    Active,
    Standby,
    Offline
}

public static class ProductionUnitStatusExtensions
{
    public static readonly Dictionary<ProductionUnitStatus, string> StatusIcons = new()
    {
        { ProductionUnitStatus.Active, "/Assets/Icons/circle-check-solid.png" },   // Green tick
        { ProductionUnitStatus.Standby, "/Assets/Icons/circle-exclamation-solid.png" }, // Yellow exclamation mark 
        { ProductionUnitStatus.Offline, "/Assets/Icons/circle-xmark-solid.png" }  // Red cross 
    };

    public static string GetIconPath(this ProductionUnitStatus status)
    {
        return StatusIcons[status];
    }
}