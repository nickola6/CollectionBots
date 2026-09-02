using System;
using UnityEngine;

public class ResourceCounter : MonoBehaviour
{
    [SerializeField] private ResourceReceiver _resourceReceiver;

    public event Action<int> CountChanged;

    public int Count { get; private set; }

    private void OnEnable()
    {
        _resourceReceiver.ResourceReceived += Add;
    }

    private void OnDisable()
    {
        _resourceReceiver.ResourceReceived -= Add;
    }

    private void Add(Resource resource)
    {
        Count++;
        CountChanged?.Invoke(Count);
    }
}