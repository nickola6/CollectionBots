using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseRegistry", menuName = "Scriptable Objects/Base Registry")]
public class BaseRegistry : ScriptableObject
{
    private readonly List<BaseController> _bases = new List<BaseController>();

    public event Action<BaseController> BaseAdded;
    public event Action<BaseController> BaseRemoved;

    public IReadOnlyList<BaseController> Bases => _bases;

    private void OnEnable()
    {
        Clear();
    }

    private void OnDisable()
    {
        Clear();
    }

    public void Register(BaseController baseController)
    {
        if (baseController == null)
            return;

        if (_bases.Contains(baseController))
            return;

        _bases.Add(baseController);
        BaseAdded?.Invoke(baseController);
    }

    public void Unregister(BaseController baseController)
    {
        if (baseController == null)
            return;

        if (_bases.Remove(baseController) == false)
            return;

        BaseRemoved?.Invoke(baseController);
    }

    public int GetBaseNumber(BaseController baseController)
    {
        return _bases.IndexOf(baseController) + 1;
    }

    private void Clear()
    {
        _bases.Clear();
    }
}