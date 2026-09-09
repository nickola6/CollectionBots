using System;
using UnityEngine;

public class BaseFactory : MonoBehaviour
{
    [SerializeField] private BaseController _basePrefab;
    [SerializeField] private ResourceSpawnService _resourceSpawnService;
    [SerializeField] private BaseRegistry _baseRegistry;
    [SerializeField] private BaseFoundationService _foundationService;

    public BaseController Create(Vector3 position, Quaternion rotation)
    {
        if (_basePrefab == null)
            throw new InvalidOperationException(
                $"{nameof(_basePrefab)} is not assigned.");

        if (_resourceSpawnService == null)
            throw new InvalidOperationException(
                $"{nameof(_resourceSpawnService)} is not assigned.");

        if (_baseRegistry == null)
            throw new InvalidOperationException(
                $"{nameof(_baseRegistry)} is not assigned.");

        if (_foundationService == null)
            throw new InvalidOperationException(
                $"{nameof(_foundationService)} is not assigned.");

        BaseController baseController = Instantiate(
            _basePrefab,
            position,
            rotation);

        baseController.Initialize(
            _resourceSpawnService,
            _baseRegistry,
            _foundationService);

        return baseController;
    }
}