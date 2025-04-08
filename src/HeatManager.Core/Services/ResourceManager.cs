using HeatManager.Core.Models.Resources;

namespace HeatManager.Core.Services;

internal class ResourceManager : IResourceManager
{
    public IEnumerable<IBasicResource> Resources { get; }

    private readonly List<IBasicResource> _resources = [];

    public void AddResource(IBasicResource resource)
    {
        _resources.Add(resource);
    }
}