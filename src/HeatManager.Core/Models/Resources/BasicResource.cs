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
    public string Name { get; }
    public ResourceType Type { get; } 
}

internal class BasicResource : IBasicResource
{
    public string Name => Type.ToString();

    public override string ToString() => Name;

    public ResourceType Type { get; }
}