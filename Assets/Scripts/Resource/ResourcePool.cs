using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    [SerializeField] private Resource _prefab;
    [SerializeField] private ResourceRegistry[] _registries;
    [SerializeField] private Transform _container;

    private ObjectPool<Resource> _pool;

    public int ActiveCount { get; private set; }

    private void Awake()
    {
        _pool = new ObjectPool<Resource>(CreateResource, _container, OnResourceGet, OnResourceReturn);
    }

    public void Prewarm(int count)
    {
        _pool.Prewarm(count);
    }

    public void Spawn(Vector3 position, Quaternion rotation)
    {
        _pool.Rent(position, rotation);
        ActiveCount++;
    }

    public void Return(Resource resource)
    {
        _pool.Return(resource);
        ActiveCount--;
    }

    private Resource CreateResource()
    {
        Resource resource = Instantiate(_prefab, _container);
        resource.gameObject.SetActive(false);

        return resource;
    }

    private void OnResourceGet(Resource resource)
    {
        foreach (ResourceRegistry registry in _registries)
            registry.Register(resource);
    }

    private void OnResourceReturn(Resource resource)
    {
        foreach (ResourceRegistry registry in _registries)
        {
            registry.Release(resource);
            registry.Unregister(resource);
        }
    }
}