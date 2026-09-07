using System.Collections.Generic;
using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    [SerializeField] private Resource _prefab;
    [SerializeField] private ResourceRegistry _resourceRegistry;
    [SerializeField] private Transform _container;
    [SerializeField] private int _prewarmCount = 10;

    private readonly HashSet<Resource> _activeResources = new HashSet<Resource>();

    private ObjectPool<Resource> _pool;

    public int ActiveCount => _activeResources.Count;

    private void Awake()
    {
        _pool = new ObjectPool<Resource>(CreateResource, _container);
        _pool.Prewarm(_prewarmCount);
    }

    public Resource Spawn(Vector3 position, Quaternion rotation)
    {
        if (_prefab == null || _resourceRegistry == null)
            return null;

        Resource resource = _pool.Rent(position, rotation);

        _activeResources.Add(resource);
        _resourceRegistry.Register(resource);

        return resource;
    }

    public void Return(Resource resource)
    {
        if (resource == null)
            return;

        if (_activeResources.Remove(resource) == false)
            return;

        _resourceRegistry.Release(resource);
        _resourceRegistry.Unregister(resource);
        _pool.Return(resource);
    }

    private Resource CreateResource()
    {
        return Instantiate(_prefab, _container);
    }
}