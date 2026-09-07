using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceScore : MonoBehaviour
{
    [SerializeField] private BaseRegistry _baseRegistry;

    public event Action<int, int> ScoreChanged;
    public event Action<int> BaseRemoved;

    private readonly Dictionary<ResourceBank, int> _numbers = new Dictionary<ResourceBank, int>();

    private void OnEnable()
    {
        if (_baseRegistry == null)
            return;

        _baseRegistry.BaseAdded += OnBaseAdded;
        _baseRegistry.BaseRemoved += OnBaseRemoved;

        foreach (BaseController baseController in _baseRegistry.Bases)
            OnBaseAdded(baseController);
    }

    private void OnDisable()
    {
        if (_baseRegistry == null)
            return;

        _baseRegistry.BaseAdded -= OnBaseAdded;
        _baseRegistry.BaseRemoved -= OnBaseRemoved;

        foreach (ResourceBank resourceBank in _numbers.Keys)
            resourceBank.CountChanged -= OnCountChanged;

        _numbers.Clear();
    }

    private void OnBaseAdded(BaseController baseController)
    {
        if (baseController == null)
            return;

        ResourceBank resourceBank = baseController.ResourceBank;

        if (_numbers.ContainsKey(resourceBank))
            return;

        int number = _baseRegistry.GetBaseNumber(baseController);
        _numbers.Add(resourceBank, number);

        resourceBank.CountChanged += OnCountChanged;
        ScoreChanged?.Invoke(number, resourceBank.Count);
    }

    private void OnBaseRemoved(BaseController baseController)
    {
        if (baseController == null)
            return;

        ResourceBank resourceBank = baseController.ResourceBank;

        if (_numbers.Remove(resourceBank, out int number) == false)
            return;

        resourceBank.CountChanged -= OnCountChanged;
        BaseRemoved?.Invoke(number);
    }

    private void OnCountChanged(ResourceBank resourceBank)
    {
        if (_numbers.TryGetValue(resourceBank, out int number) == false)
            return;

        ScoreChanged?.Invoke(number, resourceBank.Count);
    }
}