using System;
using System.Collections;
using UnityEngine;

public class BaseWorkerDispatcher : MonoBehaviour
{
    [SerializeField] private float _dispatchInterval = 0.5f;

    public event Action<UnitWorker> DispatchRequested;

    private BaseWorkforce _workforce;
    private Coroutine _dispatchRoutine;
    private bool _isDispatching;

    public void StartDispatch(BaseWorkforce workforce)
    {
        StopDispatch();

        _isDispatching = true;
        _workforce = workforce;

        if (_workforce == null)
            return;

        _workforce.WorkerAvailable += OnWorkerAvailable;
        _dispatchRoutine = StartCoroutine(DispatchRoutine());
    }

    public void StopDispatch()
    {
        _isDispatching = false;

        if (_workforce != null)
            _workforce.WorkerAvailable -= OnWorkerAvailable;

        if (_dispatchRoutine != null)
            StopCoroutine(_dispatchRoutine);

        _dispatchRoutine = null;
        _workforce = null;
    }

    public void DispatchAvailableWorkers()
    {
        if (_workforce == null)
            return;

        for (int index = _workforce.Workers.Count - 1; index >= 0; index--)
        {
            UnitWorker worker = _workforce.Workers[index];

            if (worker == null || worker.IsAvailable == false)
                continue;

            DispatchRequested?.Invoke(worker);
        }
    }

    private IEnumerator DispatchRoutine()
    {
        yield return null;

        DispatchAvailableWorkers();

        WaitForSeconds wait = new WaitForSeconds(_dispatchInterval);

        while (_isDispatching)
        {
            yield return wait;
            DispatchAvailableWorkers();
        }
    }

    private void OnWorkerAvailable(UnitWorker worker)
    {
        if (worker == null || worker.IsAvailable == false)
            return;

        DispatchRequested?.Invoke(worker);
    }
}