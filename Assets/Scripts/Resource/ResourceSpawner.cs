using System.Collections;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    private const int SpawnAttempts = 20;

    [SerializeField] private ResourceSpawnService _spawnService;
    [SerializeField] private SpawnPositionChecker _positionChecker;
    [SerializeField] private BoxCollider _spawnArea;
    [SerializeField] private int _initialSpawnCount = 10;
    [SerializeField] private int _maxActiveCount = 10;
    [SerializeField] private float _spawnInterval = 3f;

    private bool _isSpawning;
    private bool _isInitialized;
    private Coroutine _spawnRoutine;

    private void OnEnable()
    {
        _isSpawning = true;

        if (_isInitialized)
            _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        _isSpawning = false;

        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);

        _spawnRoutine = null;
    }

    private void Start()
    {
        if (_spawnService == null || _positionChecker == null || _spawnArea == null)
            return;

        SpawnInitialResources();

        _isInitialized = true;
        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_spawnInterval);

        while (_isSpawning)
        {
            yield return wait;
            SpawnRandomResource();
        }
    }

    private void SpawnInitialResources()
    {
        for (int index = 0; index < _initialSpawnCount; index++)
            SpawnRandomResource();
    }

    private void SpawnRandomResource()
    {
        if (GetActiveCount() >= _maxActiveCount)
            return;

        for (int attempt = 0; attempt < SpawnAttempts; attempt++)
        {
            Vector3 position = GetRandomPosition();

            if (_positionChecker.IsFree(position) == false)
                continue;

            if (_spawnService.Spawn(position) == null)
                return;

            return;
        }
    }

    private int GetActiveCount()
    {
        return _spawnService.ActiveCount;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 center = _spawnArea.center;
        Vector3 size = _spawnArea.size;

        float x = Random.Range(center.x - size.x * 0.5f, center.x + size.x * 0.5f);
        float z = Random.Range(center.z - size.z * 0.5f, center.z + size.z * 0.5f);

        return _spawnArea.transform.TransformPoint(new Vector3(x, center.y, z));
    }
}