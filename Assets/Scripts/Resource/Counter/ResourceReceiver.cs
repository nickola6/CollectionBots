using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceReceiver : MonoBehaviour
{
    [SerializeField] private ResourcePool _resourcePool;

    public event Action<Resource> ResourceReceived;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void Receive(Resource resource)
    {
        ResourceReceived?.Invoke(resource);
        _resourcePool.Return(resource);
    }
}