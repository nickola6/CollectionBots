using System.Collections.Generic;
using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    [SerializeField] private BaseRegistry _baseRegistry;
    [SerializeField] private BaseController _basePrefab;

    private readonly HashSet<BaseController> _subscribedBases = new HashSet<BaseController>();

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
        if (_baseRegistry != null)
        {
            _baseRegistry.BaseAdded -= OnBaseAdded;
            _baseRegistry.BaseRemoved -= OnBaseRemoved;
        }

        foreach (BaseController baseController in _subscribedBases)
        {
            if (baseController != null)
            {
                baseController.BaseFounded -= OnBaseFounded;
            }
        }

        _subscribedBases.Clear();
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

        if (_subscribedBases.Add(baseController))
            baseController.BaseFounded += OnBaseFounded;
    }

    private void Unsubscribe(BaseController baseController)
    {
        if (baseController == null)
            return;

        if (_subscribedBases.Remove(baseController))
            baseController.BaseFounded -= OnBaseFounded;
    }

    private void OnBaseFounded(UnitWorker founder, Vector3 position)
    {
        if (_basePrefab == null || founder == null)
            return;

        BaseController newBase = Instantiate(_basePrefab, position, Quaternion.identity);
        newBase.ReceiveFoundingWorker(founder);
    }
}