using HeatManager.Core.Models.Resources;

namespace HeatManager.Core.Services;

public interface IResourceManager
{
    public IEnumerable<IBasicResource> Resources { get; }

    public void AddResource(IBasicResource resource);

    // TODO: Add method to remove resource - need to figure out what to do with existing heat sources
}