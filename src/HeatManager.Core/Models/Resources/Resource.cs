namespace HeatManager.Core.Models.Resources;

public class Resource
{
    // Used for serialization
    public Resource() { }

    public Resource(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public ResourceType Type => Name switch
    {
        "Gas" => ResourceType.Gas,
        "Oil" => ResourceType.Oil,
        "Electricity" => ResourceType.Electricity,
        _ => throw new ArgumentException($"Invalid resource name: {Name}")
    };

    public override string ToString() => Name;
}