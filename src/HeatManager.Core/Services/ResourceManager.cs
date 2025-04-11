using HeatManager.Core.Models.Resources;

namespace HeatManager.Core.Services;

internal class ResourceManager : IResourceManager
{
    public IEnumerable<BasicResource> Resources { get; }

    private readonly List<BasicResource> _resources = [];

    public void AddResource(BasicResource resource)
    {
        _resources.Add(resource);
    }
}