namespace HeatManager.Core.Models.Resources;

//TODO: Restructure the asset manager to get this
public enum ResourceType
{
    Gas,
    Oil,
    Electricity
}

public interface IBasicResource
{
    public string Name { get; set; }
    public ResourceType Type { get; set; } //TODO: change it to this please
}

internal class BasicResource : IBasicResource
{
    public string Name { get; set; }
    public ResourceType Type { get; set; }
}