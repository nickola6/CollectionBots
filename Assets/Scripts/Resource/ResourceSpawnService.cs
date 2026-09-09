using System;
using UnityEngine;

public class ResourceSpawnService : MonoBehaviour
{
    [SerializeField] private ResourcePool _pool;

    private readonly ResourceRegistry _registry = new ResourceRegistry();

    public int ActiveCount => _pool.ActiveCount;
    public ResourceRegistry Registry => _registry;

    private void Awake()
    {
        if (_pool == null)
            throw new InvalidOperationException($"{nameof(_pool)} is not assigned.");
    }

    public Resource Spawn(Vector3 position)
    {
        if (_pool == null)
            return null;

        Resource resource = _pool.Spawn(position, Quaternion.identity);

        if (resource == null)
            return null;

        _registry.Register(resource);
        return resource;
    }

    public void Return(Resource resource)
    {
        if (resource == null)
            return;

        _registry.Release(resource);
        _registry.Unregister(resource);
        _pool.Return(resource);
    }
}
