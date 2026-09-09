using System.Collections.Generic;

public sealed class BaseRegistryState
{
    private readonly List<BaseController> _bases = new List<BaseController>();

    public IReadOnlyList<BaseController> Bases => _bases;

    internal int Count => _bases.Count;

    internal void Add(BaseController baseController)
    {
        _bases.Add(baseController);
    }

    internal bool Remove(BaseController baseController)
    {
        return _bases.Remove(baseController);
    }

    internal void Clear()
    {
        _bases.Clear();
    }

    internal int IndexOf(BaseController baseController)
    {
        return _bases.IndexOf(baseController);
    }
}
