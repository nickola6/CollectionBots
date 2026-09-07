using UnityEngine;

public class ResourceDeliveryHandler : MonoBehaviour
{
    [SerializeField] private ResourcePool _resourcePool;
    [SerializeField] private BaseRegistry _baseRegistry;

    private void OnEnable()
    {
        if (_baseRegistry == null)
            return;

        _baseRegistry.BaseAdded += OnBaseAdded;
        _baseRegistry.BaseRemoved += OnBaseRemoved;

        foreach (BaseController baseController in _baseRegistry.Bases)
            Subscribe(baseController);
    }

    private void OnDisable()
    {
        if (_baseRegistry == null)
            return;

        _baseRegistry.BaseAdded -= OnBaseAdded;
        _baseRegistry.BaseRemoved -= OnBaseRemoved;

        foreach (BaseController baseController in _baseRegistry.Bases)
            Unsubscribe(baseController);
    }

    private void OnBaseAdded(BaseController baseController)
    {
        Subscribe(baseController);
    }

    private void OnBaseRemoved(BaseController baseController)
    {
        Unsubscribe(baseController);
    }

    private void Subscribe(BaseController baseController)
    {
        if (baseController == null)
            return;

        baseController.ResourceReceiver.ResourceReceived += OnResourceReceived;
    }

    private void Unsubscribe(BaseController baseController)
    {
        if (baseController == null)
            return;

        baseController.ResourceReceiver.ResourceReceived -= OnResourceReceived;
    }

    private void OnResourceReceived(Resource resource)
    {
        if (_resourcePool == null)
            return;

        _resourcePool.Return(resource);
    }
}