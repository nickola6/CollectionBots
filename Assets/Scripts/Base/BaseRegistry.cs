using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseRegistry : MonoBehaviour
{
    private readonly BaseRegistryState _state = new BaseRegistryState();

    public event Action<BaseController> BaseAdded;
    public event Action<BaseController> BaseRemoved;

    public IReadOnlyList<BaseController> Bases => _state.Bases;

    private void OnDisable()
    {
        _state.Clear();
    }

    public void Register(BaseController baseController)
    {
        if (baseController == null)
            return;

        if (_state.IndexOf(baseController) >= 0)
            return;

        _state.Add(baseController);
        BaseAdded?.Invoke(baseController);
    }

    public void Unregister(BaseController baseController)
    {
        if (baseController == null)
            return;

        if (_state.Remove(baseController) == false)
            return;

        BaseRemoved?.Invoke(baseController);
    }

    public int GetBaseNumber(BaseController baseController)
    {
        return _state.IndexOf(baseController) + 1;
    }
}
