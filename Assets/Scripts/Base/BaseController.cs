using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BaseController : MonoBehaviour
{
    private const int NewWorkerResourceCount = 3;
    private const int NewBaseResourceCount = 5;
    private const int MinWorkersRequiredToFoundBase = 2;

    [SerializeField] private ResourceRegistry _resourceRegistry;
    [SerializeField] private ResourceReceiver _resourceReceiver;
    [SerializeField] private BaseRegistry _baseRegistry;
    [SerializeField] private BaseWorkerDispatcher _workerDispatcher;

    [SerializeField] private WorkerSpawner _workerSpawner;
    [SerializeField] private UnitWorker[] _initialWorkers;
    [SerializeField] private Transform _flagPrefab;

    public event Action<UnitWorker, Vector3> BaseFounded;

    private readonly ResourceBank _resourceBank = new ResourceBank();
    private BaseScanner _scanner;
    private BaseWorkforce _workforce;
    private BaseFlag _flag;
    private WorkerResourceAssigner _resourceAssigner;

    private bool _isFounderDispatched;
    private bool _isStarted;

    public ResourceReceiver ResourceReceiver => _resourceReceiver;
    public ResourceBank ResourceBank => _resourceBank;
    private bool HasFlag => _flag.IsExist;

    private void Awake()
    {
        _scanner = new BaseScanner();
        _scanner.Initialize(_resourceRegistry);

        _workforce = new BaseWorkforce();
        _resourceAssigner = new WorkerResourceAssigner(_scanner, _resourceRegistry);
        _flag = new BaseFlag(_flagPrefab);
    }

    private void OnEnable()
    {
        if (_workerDispatcher != null)
            _workerDispatcher.DispatchRequested += OnDispatchRequested;

        if (_isStarted)
        {
            _baseRegistry.Register(this);
            _workerDispatcher.StartDispatch(_workforce);
        }
    }

    private void OnDisable()
    {
        if (_workerDispatcher != null)
        {
            _workerDispatcher.DispatchRequested -= OnDispatchRequested;
            _workerDispatcher.StopDispatch();
        }

        if (_baseRegistry != null)
            _baseRegistry.Unregister(this);
    }

    private void Start()
    {
        Initialize();
        RegisterInitialWorkers();

        _baseRegistry.Register(this);
        _workerDispatcher.StartDispatch(_workforce);
        _isStarted = true;
    }

    private void OnDestroy()
    {
        if (_resourceReceiver != null)
            _resourceReceiver.ResourceReceived -= OnResourceReceived;
    }

    public void Initialize()
    {
        _resourceReceiver.ResourceReceived -= OnResourceReceived;
        _resourceReceiver.ResourceReceived += OnResourceReceived;
    }

    public void PlaceFlag(Vector3 position)
    {
        _flag.Place(position);
    }

    public void ReceiveFoundingWorker(UnitWorker worker)
    {
        RegisterWorker(worker);
    }

    private void RegisterInitialWorkers()
    {
        foreach (UnitWorker worker in _initialWorkers)
        {
            if (worker == null)
                continue;

            RegisterWorker(worker);
        }
    }

    private void RegisterWorker(UnitWorker worker)
    {
        if (worker == null)
            return;

        worker.SetHomeBase(_resourceReceiver);
        _workforce.Add(worker);
    }

    private void OnDispatchRequested(UnitWorker worker)
    {
        if (worker == null || worker.IsAvailable == false)
            return;

        if (TryDispatchFounder(worker))
            return;

        _resourceAssigner.TryAssign(worker, transform.position);
    }

    private bool TryDispatchFounder(UnitWorker worker)
    {
        if (HasFlag == false)
            return false;

        if (_isFounderDispatched)
            return false;

        if (_workforce.WorkersCount < MinWorkersRequiredToFoundBase)
            return false;

        if (_resourceBank.TrySpend(NewBaseResourceCount) == false)
            return false;

        _isFounderDispatched = true;
        _workforce.Remove(worker);

        worker.BaseFounded += OnWorkerBaseFounded;
        worker.StartFoundingBase(_flag.Transform);

        return true;
    }

    private bool TrySpawnWorker()
    {
        if (HasFlag)
            return false;

        if (_workerSpawner == null)
            return false;

        if (_resourceBank.TrySpend(NewWorkerResourceCount) == false)
            return false;

        UnitWorker worker = _workerSpawner.Spawn();

        RegisterWorker(worker);

        return true;
    }

    private void OnResourceReceived(Resource resource)
    {
        _resourceBank.Add();

        if (HasFlag)
        {
            _workerDispatcher.DispatchAvailableWorkers();
            return;
        }

        TrySpawnWorker();

        _workerDispatcher.DispatchAvailableWorkers();
    }

    private void OnWorkerBaseFounded(UnitWorker worker, Vector3 position)
    {
        worker.BaseFounded -= OnWorkerBaseFounded;

        _flag.Clear();
        _isFounderDispatched = false;

        TrySpawnWorker();

        BaseFounded?.Invoke(worker, position);
    }
}