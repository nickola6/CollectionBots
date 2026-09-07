using System;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    private const int LeftMouseButton = 0;

    [SerializeField] private Collider _ground;

    public event Action<BaseController> BaseClicked;
    public event Action<Vector3> GroundClicked;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(LeftMouseButton))
            HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit) == false)
            return;

        if (hit.collider.TryGetComponent(out BaseController baseController))
        {
            BaseClicked?.Invoke(baseController);
            return;
        }

        if (hit.collider == _ground)
            GroundClicked?.Invoke(hit.point);
    }
}