using UnityEngine;

public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private BoxCollider _mapBounds;

    private BaseController _selectedBase;

    private void OnEnable()
    {
        _inputReader.BaseClicked += OnBaseClicked;
        _inputReader.GroundClicked += OnGroundClicked;
    }

    private void OnDisable()
    {
        _inputReader.BaseClicked -= OnBaseClicked;
        _inputReader.GroundClicked -= OnGroundClicked;
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

        if (_mapBounds.bounds.Contains(position) == false)
            return;

        _selectedBase.PlaceFlag(position);
        _selectedBase = null;
    }
}