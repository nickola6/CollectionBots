using UnityEngine;

public sealed class BaseFlag
{
    public bool IsPlaced { get; private set; }
    public Vector3 Position { get; private set; }

    internal void Place(Vector3 position)
    {
        Position = position;
        IsPlaced = true;
    }

    internal void Clear()
    {
        IsPlaced = false;
        Position = default;
    }
}
