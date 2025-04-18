namespace HeatManager.Core.Models.Resources;

internal class BasicResource : IBasicResource
{
    public string Name { get; set; }

    public override string ToString()
    {
            return Name;
    }
}