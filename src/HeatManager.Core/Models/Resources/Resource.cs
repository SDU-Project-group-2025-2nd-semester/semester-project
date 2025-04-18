namespace HeatManager.Core.Models.Resources;

public class Resource
{
    public string Name { get; set; }
    public ResourceType Type { get; set; }

    public Resource(string name) 
    { 
        Name = name; 
        switch (name) 
        { 
            case "Gas":
                Type = ResourceType.Gas;
                break;
            case "Oil":
                Type = ResourceType.Oil;
                break;
            case "Electricity":
                Type = ResourceType.Electricity;
                break;
            default:
                throw new ArgumentException($"Invalid resource name: {name}");
        }
    }
}