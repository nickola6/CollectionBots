using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    [SerializeField] private Resource _prefab;
    [SerializeField] private Transform _container;
    [SerializeField] private int _prewarmCount = 10;

    private ObjectPool<Resource> _pool;

    public int ActiveCount { get; private set; }

    private void Awake()
    {
        _pool = new ObjectPool<Resource>(CreateResource, _container);
        _pool.Prewarm(_prewarmCount);
    }

    public Resource Spawn(Vector3 position, Quaternion rotation)
    {
        if (_prefab == null)
            return null;

        return _pool.Rent(position, rotation);
    }

    public void Return(Resource resource)
    {
        if (resource == null)
            return;

        _pool.Return(resource);
    }

    private Resource CreateResource()
    {
        return Instantiate(_prefab, _container);
    }
}