namespace HeatManager.Core.Models.Resources;

public class Resource(string name)
{
    public string Name { get; set; } = name;
    public ResourceType Type { get; set; } = name switch
    {
        "Gas" => ResourceType.Gas,
        "Oil" => ResourceType.Oil,
        "Electricity" => ResourceType.Electricity,
        _ => throw new ArgumentException($"Invalid resource name: {name}")
    };
    public override string ToString()
    {
        return Name;
    }
}