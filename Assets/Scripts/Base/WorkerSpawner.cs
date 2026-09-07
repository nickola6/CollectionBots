using UnityEngine;

public class WorkerSpawner : MonoBehaviour
{
    [SerializeField] private UnitWorker _prefab;
    [SerializeField] private Transform _spawnPoint;

    public UnitWorker Spawn()
    {
        if (_prefab == null || _spawnPoint == null)
            return null;

        return Instantiate(_prefab, _spawnPoint.position, _spawnPoint.rotation);
    }
}