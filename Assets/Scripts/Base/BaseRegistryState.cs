using System.Collections.Generic;

public class BaseRegistryState
{
    private readonly List<BaseController> _bases = new List<BaseController>();

    public IReadOnlyList<BaseController> Bases => _bases;
    public int Count => _bases.Count;

    public void Add(BaseController baseController)
    {
        _bases.Add(baseController);
    }

    public bool Remove(BaseController baseController)
    {
        return _bases.Remove(baseController);
    }

    public void Clear()
    {
        _bases.Clear();
    }

    public int IndexOf(BaseController baseController)
    {
        return _bases.IndexOf(baseController);
    }
}