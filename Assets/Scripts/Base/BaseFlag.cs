using UnityEngine;

public class BaseFlag
{
    private Transform _prefab;
    private Transform _instance;

    public BaseFlag(Transform prefab)
    {
        _prefab = prefab;
    }

    public bool IsExist => _instance != null;
    public Transform Transform => _instance;

    public void Place(Vector3 position)
    {
        if (_prefab == null)
            return;

        if (_instance == null)
            _instance = Object.Instantiate(_prefab, position, Quaternion.identity);
        else
            _instance.position = position;
    }

    public void Clear()
    {
        if (_instance == null)
            return;

        Object.Destroy(_instance.gameObject);
        _instance = null;
    }
}