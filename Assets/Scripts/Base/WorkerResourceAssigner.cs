using UnityEngine;

public class WorkerResourceAssigner
{
    private readonly BaseScanner _scanner;
    private readonly ResourceRegistry _resourceRegistry;

    public WorkerResourceAssigner(BaseScanner scanner, ResourceRegistry resourceRegistry)
    {
        _scanner = scanner;
        _resourceRegistry = resourceRegistry;
    }

    public bool TryAssign(UnitWorker worker, Vector3 origin)
    {
        if (worker == null || worker.IsAvailable == false)
            return false;

        if (_scanner.FindNearestAvailable(origin, out Resource resource) == false)
            return false;

        if (_resourceRegistry.TryOccupy(resource) == false)
            return false;

        worker.StartCollecting(resource);

        return true;
    }
}