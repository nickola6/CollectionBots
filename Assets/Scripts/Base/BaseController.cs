using System.Collections;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    [SerializeField] private ResourceRegistry _resourceRegistry;
    [SerializeField] private ResourceReceiver _resourceReceiver;
    [SerializeField] private UnitWorker[] _workers;
    [SerializeField] private float _scanInterval = 0.5f;

    private BaseScanner _scanner;
    private Coroutine _scanRoutine;
    private bool _isScanning;

    private void Awake()
    {
        _scanner = new BaseScanner();
        _scanner.Initialize(_resourceRegistry);
    }

    private void Start()
    {
        foreach (UnitWorker worker in _workers)
        {
            worker.Initialize(_resourceReceiver);
            worker.Available += OnWorkerAvailable;
        }

        _isScanning = true;
        _scanRoutine = StartCoroutine(ScanRoutine());
    }

    private void OnDisable()
    {
        _isScanning = false;

        if (_scanRoutine != null)
            StopCoroutine(_scanRoutine);

        _scanRoutine = null;

        foreach (UnitWorker worker in _workers)
            worker.Available -= OnWorkerAvailable;
    }

    private IEnumerator ScanRoutine()
    {
        yield return null;

        DispatchWorkers();

        WaitForSeconds wait = new WaitForSeconds(_scanInterval);

        while (_isScanning == true)
        {
            yield return wait;
            DispatchWorkers();
        }
    }

    private void DispatchWorkers()
    {
        foreach (UnitWorker worker in _workers)
        {
            if (worker.IsAvailable == false)
                continue;

            AssignNextResource(worker);
        }
    }

    private void OnWorkerAvailable(UnitWorker worker)
    {
        AssignNextResource(worker);
    }

    private bool AssignNextResource(UnitWorker worker)
    {
        if (worker.IsAvailable == false)
            return false;

        if (_scanner.FindNearestAvailable(transform.position, out Resource resource) == false)
            return false;

        if (_resourceRegistry.TryOccupy(resource) == false)
            return false;

        worker.StartCollecting(resource);
        return true;
    }
}