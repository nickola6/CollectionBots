using System;
using System.Collections.Generic;

public class BaseWorkforce
{
    private readonly List<UnitWorker> _workers = new List<UnitWorker>();

    public event Action<UnitWorker> WorkerAvailable;

    public int WorkersCount => _workers.Count;

    public IReadOnlyList<UnitWorker> Workers => _workers;

    public void Add(UnitWorker worker)
    {
        if (worker == null)
            return;

        if (_workers.Contains(worker))
            return;

        _workers.Add(worker);
        worker.Available += OnWorkerAvailable;
    }

    public void Remove(UnitWorker worker)
    {
        if (worker == null)
            return;

        if (_workers.Remove(worker) == false)
            return;

        worker.Available -= OnWorkerAvailable;
    }

    private void OnWorkerAvailable(UnitWorker worker)
    {
        WorkerAvailable?.Invoke(worker);
    }
}