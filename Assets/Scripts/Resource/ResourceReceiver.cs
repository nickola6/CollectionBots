using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceReceiver : MonoBehaviour
{
    public event Action<Resource> ResourceReceived;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void Receive(Resource resource)
    {
        if (resource == null)
            return;

        ResourceReceived?.Invoke(resource);
    }
}