using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseFoundationService : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private BoxCollider _mapBounds;
    [SerializeField] private BaseRegistry _baseRegistry;
    [SerializeField] private Transform _flagPrefab;

    public event Action<BaseController, Vector3> FoundationRequested;

    private readonly Dictionary<BaseController, BaseFlag> _flags = new Dictionary<BaseController, BaseFlag>();
    private readonly Dictionary<BaseController, Transform> _flagInstances = new Dictionary<BaseController, Transform>();
    private readonly Dictionary<BaseController, Action<UnitWorker, Vector3>> _baseFoundedHandlers = new Dictionary<BaseController, Action<UnitWorker, Vector3>>();
    private BaseController _selectedBase;

    private void OnEnable()
    {
        if (_inputReader != null)
        {
            _inputReader.BaseClicked += OnBaseClicked;
            _inputReader.GroundClicked += OnGroundClicked;
        }

        if (_baseRegistry != null)
        {
            _baseRegistry.BaseAdded += OnBaseAdded;
            _baseRegistry.BaseRemoved += OnBaseRemoved;

            foreach (BaseController baseController in _baseRegistry.Bases)
                Subscribe(baseController);
        }
    }

    private void OnDisable()
    {
        if (_inputReader != null)
        {
            _inputReader.BaseClicked -= OnBaseClicked;
            _inputReader.GroundClicked -= OnGroundClicked;
        }

        if (_baseRegistry != null)
        {
            _baseRegistry.BaseAdded -= OnBaseAdded;
            _baseRegistry.BaseRemoved -= OnBaseRemoved;
        }

        foreach (BaseController baseController in new List<BaseController>(_flags.Keys))
            Unsubscribe(baseController);

        foreach (Transform instance in _flagInstances.Values)
        {
            if (instance != null)
                Destroy(instance.gameObject);
        }

        _flags.Clear();
        _flagInstances.Clear();
        _selectedBase = null;
    }

    private void OnBaseClicked(BaseController baseController)
    {
        _selectedBase = baseController;
    }

    private void OnGroundClicked(Vector3 position)
    {
        if (_selectedBase == null)
            return;

        if (_mapBounds == null || _mapBounds.bounds.Contains(position) == false)
            return;

        PlaceFlag(_selectedBase, position);
        _selectedBase = null;
    }

    private void PlaceFlag(BaseController baseController, Vector3 position)
    {
        if (baseController == null || _flagPrefab == null)
            return;

        if (_flags.TryGetValue(baseController, out BaseFlag flag) == false)
        {
            flag = new BaseFlag();
            _flags.Add(baseController, flag);
        }

        flag.Place(position);

        if (_flagInstances.TryGetValue(baseController, out Transform instance) == false || instance == null)
        {
            instance = Instantiate(_flagPrefab, position, Quaternion.identity);
            _flagInstances[baseController] = instance;
        }
        else
        {
            instance.position = position;
        }

        FoundationRequested?.Invoke(baseController, position);
    }

    private void OnBaseFounded(BaseController baseController, UnitWorker worker, Vector3 position)
    {
        ClearFlag(baseController);
    }

    private void ClearFlag(BaseController baseController)
    {
        if (baseController == null)
            return;

        if (_flagInstances.Remove(baseController, out Transform instance) && instance != null)
            Destroy(instance.gameObject);

        if (_flags.TryGetValue(baseController, out BaseFlag flag))
            flag.Clear();
    }

    private void OnBaseAdded(BaseController baseController)
    {
        Subscribe(baseController);
    }

    private void OnBaseRemoved(BaseController baseController)
    {
        Unsubscribe(baseController);
        ClearFlag(baseController);

        if (_selectedBase == baseController)
            _selectedBase = null;
    }

    private void Subscribe(BaseController baseController)
    {
        if (baseController == null)
            return;

        if (_flags.ContainsKey(baseController) == false)
            _flags.Add(baseController, new BaseFlag());

        if (_baseFoundedHandlers.ContainsKey(baseController))
            return;

        Action<UnitWorker, Vector3> handler = (worker, position) => OnBaseFounded(baseController, worker, position);

        _baseFoundedHandlers.Add(baseController, handler);
        baseController.BaseFounded += handler;
    }

    private void Unsubscribe(BaseController baseController)
    {
        if (baseController == null)
            return;

        if (_baseFoundedHandlers.Remove(baseController, out Action<UnitWorker, Vector3> handler))
            baseController.BaseFounded -= handler;
    }
}