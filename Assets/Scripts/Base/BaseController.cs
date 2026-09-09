using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BaseController : MonoBehaviour
{
    private const int NewWorkerResourceCount = 3;
    private const int NewBaseResourceCount = 5;
    private const int MinWorkersRequiredToFoundBase = 2;

    [SerializeField] private ResourceReceiver _resourceReceiver;
    [SerializeField] private ResourceSpawnService _resourceSpawnService;

    [SerializeField] private BaseRegistry _baseRegistry;
    [SerializeField] private BaseWorkerDispatcher _workerDispatcher;
    [SerializeField] private BaseFoundationService _foundationService;

    [SerializeField] private WorkerSpawner _workerSpawner;
    [SerializeField] private UnitWorker[] _initialWorkers;

    public event Action<UnitWorker, Vector3> BaseFounded;

    private readonly ResourceBank _resourceBank = new ResourceBank();

    private BaseScanner _scanner;
    private BaseWorkforce _workforce;
    private WorkerResourceAssigner _resourceAssigner;

    private Vector3 _foundationPosition;
    private Transform _foundationDestination;

    private bool _hasFoundationRequest;
    private bool _isFounderDispatched;
    private bool _isStarted;
    private bool _isInitialized;
    
    public ResourceReceiver ResourceReceiver => _resourceReceiver;
    public ResourceBank ResourceBank => _resourceBank;

    private void Awake()
    {
        _scanner = new BaseScanner();
        _workforce = new BaseWorkforce();
    }

    private void OnEnable()
    {
        if (_isStarted == false)
            return;

        Subscribe();
        _workerDispatcher.StartDispatch(_workforce);
    }

    private void OnDisable()
    {
        if (_workerDispatcher != null)
        {
            _workerDispatcher.DispatchRequested -= OnDispatchRequested;
            _workerDispatcher.StopDispatch();
        }

        if (_foundationService != null)
            _foundationService.FoundationRequested -= OnFoundationRequested;

        if (_baseRegistry != null)
            _baseRegistry.Unregister(this);
    }

    private void Start()
    {
        if (_isInitialized == false)
            InitializeFromInspector();

        RegisterInitialWorkers();

        _resourceReceiver.ResourceReceived -= OnResourceReceived;
        _resourceReceiver.ResourceReceived += OnResourceReceived;

        Subscribe();

        _baseRegistry.Register(this);
        _workerDispatcher.StartDispatch(_workforce);

        _isStarted = true;
    }

    private void OnDestroy()
    {
        if (_resourceReceiver != null)
            _resourceReceiver.ResourceReceived -= OnResourceReceived;

        if (_foundationDestination != null)
            Destroy(_foundationDestination.gameObject);
    }

    public void Initialize(
    ResourceSpawnService resourceSpawnService,
    BaseRegistry baseRegistry,
    BaseFoundationService foundationService)
    {
        if (resourceSpawnService == null)
            throw new InvalidOperationException(
                $"{nameof(resourceSpawnService)} is not assigned.");

        if (baseRegistry == null)
            throw new InvalidOperationException(
                $"{nameof(baseRegistry)} is not assigned.");

        if (foundationService == null)
            throw new InvalidOperationException(
                $"{nameof(foundationService)} is not assigned.");

        _resourceSpawnService = resourceSpawnService;
        _baseRegistry = baseRegistry;
        _foundationService = foundationService;

        ResourceRegistry resourceRegistry =
            _resourceSpawnService.Registry;

        _scanner.Initialize(resourceRegistry);

        _resourceAssigner = new WorkerResourceAssigner(
            _scanner,
            resourceRegistry);

        _isInitialized = true;
    }

    private void InitializeFromInspector()
    {
        if (_resourceSpawnService == null)
            throw new InvalidOperationException(
                $"{nameof(_resourceSpawnService)} is not assigned.");

        if (_baseRegistry == null)
            throw new InvalidOperationException(
                $"{nameof(_baseRegistry)} is not assigned.");

        if (_foundationService == null)
            throw new InvalidOperationException(
                $"{nameof(_foundationService)} is not assigned.");

        ResourceRegistry resourceRegistry =
            _resourceSpawnService.Registry;

        _scanner.Initialize(resourceRegistry);

        _resourceAssigner = new WorkerResourceAssigner(
            _scanner,
            resourceRegistry);

        _isInitialized = true;
    }

    private void Subscribe()
    {
        if (_workerDispatcher != null)
            _workerDispatcher.DispatchRequested += OnDispatchRequested;

        if (_foundationService != null)
            _foundationService.FoundationRequested += OnFoundationRequested;
    }

    public void ReceiveFoundingWorker(UnitWorker worker)
    {
        RegisterWorker(worker);
    }

    private void OnFoundationRequested(
        BaseController baseController,
        Vector3 position)
    {
        if (baseController != this)
            return;

        _foundationPosition = position;
        _hasFoundationRequest = true;
        _isFounderDispatched = false;

        _workerDispatcher.DispatchAvailableWorkers();
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

        if (_resourceAssigner == null)
            return;

        _resourceAssigner.TryAssign(
            worker,
            transform.position);
    }

    private bool TryDispatchFounder(UnitWorker worker)
    {
        if (_hasFoundationRequest == false)
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
        worker.StartFoundingBase(
            GetFoundationDestination());

        return true;
    }

    private Transform GetFoundationDestination()
    {
        if (_foundationDestination == null)
        {
            GameObject destination =
                new GameObject("FoundationDestination");

            _foundationDestination = destination.transform;
        }

        _foundationDestination.position = _foundationPosition;

        return _foundationDestination;
    }

    private bool TrySpawnWorker()
    {
        if (_hasFoundationRequest)
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

        if (_hasFoundationRequest)
        {
            _workerDispatcher.DispatchAvailableWorkers();
            return;
        }

        TrySpawnWorker();

        _workerDispatcher.DispatchAvailableWorkers();
    }

    private void OnWorkerBaseFounded(
        UnitWorker worker,
        Vector3 position)
    {
        worker.BaseFounded -= OnWorkerBaseFounded;

        _hasFoundationRequest = false;
        _isFounderDispatched = false;

        TrySpawnWorker();

        BaseFounded?.Invoke(worker, position);
    }
}