using System.Collections.Generic;

public class ResourceRegistry : IResourceProvider
{
    private readonly HashSet<Resource> _resources = new HashSet<Resource>();
    private readonly HashSet<Resource> _occupiedResources = new HashSet<Resource>();

    public IEnumerable<Resource> Resources => _resources;

    public void Register(Resource resource)
    {
        if (resource == null)
            return;

        _resources.Add(resource);
    }

    public void Unregister(Resource resource)
    {
        if (resource == null)
            return;

        _resources.Remove(resource);
        _occupiedResources.Remove(resource);
    }

    public bool IsOccupied(Resource resource)
    {
        return resource != null && _occupiedResources.Contains(resource);
    }

    public bool TryOccupy(Resource resource)
    {
        if (resource == null)
            return false;

        if (_resources.Contains(resource) == false)
            return false;

        return _occupiedResources.Add(resource);
    }

    public void Release(Resource resource)
    {
        if (resource == null)
            return;

        _occupiedResources.Remove(resource);
    }
}