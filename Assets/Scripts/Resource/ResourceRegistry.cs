using System.Collections.Generic;
using UnityEngine;

public class ResourceRegistry : MonoBehaviour, IResourceProvider
{
    private readonly HashSet<Resource> _resources = new HashSet<Resource>();
    private readonly HashSet<Resource> _occupiedResources = new HashSet<Resource>();

    public IEnumerable<Resource> Resources => _resources;

    public void Register(Resource resource)
    {
        _resources.Add(resource);
    }

    public void Unregister(Resource resource)
    {
        _resources.Remove(resource);
        _occupiedResources.Remove(resource);
    }

    public bool IsOccupied(Resource resource)
    {
        return _occupiedResources.Contains(resource);
    }

    public bool TryOccupy(Resource resource)
    {
        if (_resources.Contains(resource) == false)
            return false;

        return _occupiedResources.Add(resource);
    }

    public void Release(Resource resource)
    {
        _occupiedResources.Remove(resource);
    }

    public bool IsPositionOccupied(Vector3 position, float minDistance)
    {
        float distanceSqr = minDistance * minDistance;

        foreach (Resource resource in _resources)
        {
            Vector3 offset = resource.transform.position - position;

            if (offset.sqrMagnitude <= distanceSqr)
                return true;
        }

        return false;
    }
}