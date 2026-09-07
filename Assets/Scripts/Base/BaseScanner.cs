using UnityEngine;

public class BaseScanner
{
    private IResourceProvider _provider;

    public void Initialize(IResourceProvider provider)
    {
        _provider = provider;
    }

    public bool FindNearestAvailable(Vector3 origin, out Resource resource)
    {
        resource = null;

        if (_provider == null)
            return false;

        float closestDistanceSqr = float.MaxValue;
        bool resourceFound = false;

        foreach (Resource candidate in _provider.Resources)
        {
            if (candidate == null)
                continue;

            if (_provider.IsOccupied(candidate))
                continue;

            Vector3 offset = candidate.transform.position - origin;
            float distanceSqr = offset.sqrMagnitude;

            if (distanceSqr >= closestDistanceSqr)
                continue;

            closestDistanceSqr = distanceSqr;
            resource = candidate;
            resourceFound = true;
        }

        return resourceFound;
    }
}