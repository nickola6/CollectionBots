using System;

public class ResourceBank
{
    public event Action<ResourceBank> CountChanged;

    public int Count { get; private set; }

    public void Add()
    {
        Count++;
        CountChanged?.Invoke(this);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0)
            return false;

        if (Count < amount)
            return false;

        Count -= amount;
        CountChanged?.Invoke(this);

        return true;
    }
}