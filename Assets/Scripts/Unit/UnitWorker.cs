using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitWorker : MonoBehaviour
{
    [SerializeField] private UnitMover _mover;
    [SerializeField] private Transform _handPoint;

    public event Action<UnitWorker> Available;

    private Animator _animator;
    private UnitAnimator _unitAnimator;
    private ResourceReceiver _resourceReceiver;
    private bool _isBusy;

    public bool IsAvailable => _isBusy == false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _unitAnimator = new UnitAnimator();
        _unitAnimator.Initialize(_animator);
    }

    public void Initialize(ResourceReceiver resourceReceiver)
    {
        _resourceReceiver = resourceReceiver;
        _unitAnimator.SetState(UnitAnimationState.Sit);
    }

    public void StartCollecting(Resource resource)
    {
        if (IsAvailable == false)
            return;

        _isBusy = true;
        StartCoroutine(Collect(resource));
    }

    public IEnumerator Collect(Resource resource)
    {
        _unitAnimator.SetState(UnitAnimationState.Run);
        yield return _mover.MoveToRoutine(resource.transform);

        _unitAnimator.SetState(UnitAnimationState.Collect);
        yield return _unitAnimator.WaitForAnimationFinished(UnitAnimationState.Collect);

        resource.transform.SetParent(_handPoint);
        resource.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _unitAnimator.SetState(UnitAnimationState.Run);
        yield return _mover.MoveToRoutine(_resourceReceiver.transform);

        _resourceReceiver.Receive(resource);

        _isBusy = false;
        _unitAnimator.SetState(UnitAnimationState.Sit);
        Available?.Invoke(this);
    }
}