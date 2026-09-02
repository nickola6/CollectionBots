using System.Collections;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    private const int SpawnAttempts = 20;

    [SerializeField] private ResourcePool _pool;
    [SerializeField] private ResourceRegistry _registry;
    [SerializeField] private BoxCollider _spawnArea;
    [SerializeField] private int _initialSpawnCount = 2;
    [SerializeField] private int _maxActiveCount = 5;
    [SerializeField] private float _spawnInterval = 8f;
    [SerializeField] private float _minSpawnDistance = 2f;

    private Coroutine _spawnRoutine;
    private bool _isSpawning;
    private bool _isInitialized;

    private void OnEnable()
    {
        _isSpawning = true;

        if (_isInitialized == true)
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
        _pool.Prewarm(_maxActiveCount);
        SpawnInitialResources();

        _isInitialized = true;
        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_spawnInterval);

        while (_isSpawning == true)
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
        if (_pool.ActiveCount >= _maxActiveCount)
            return;

        for (int attempt = 0; attempt < SpawnAttempts; attempt++)
        {
            Vector3 position = GetRandomPosition();

            if (_registry.IsPositionOccupied(position, _minSpawnDistance) == true)
                continue;

            _pool.Spawn(position, Quaternion.identity);
            return;
        }
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