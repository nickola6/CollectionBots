using System;
using UnityEngine;

public class ResourceSpawnService : MonoBehaviour
{
    [SerializeField] private ResourcePool _pool;

    public int ActiveCount => _pool.ActiveCount;

    private void Awake()
    {
        if (_pool == null)
            throw new InvalidOperationException($"{nameof(_pool)} is not assigned.");
    }

    public Resource Spawn(Vector3 position)
    {
        if (_pool == null)
            return null;

        return _pool.Spawn(position, Quaternion.identity);
    }
}