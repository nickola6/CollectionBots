using System;
using System.Collections;
using UnityEngine;

public class UnitWorker : MonoBehaviour
{
    [SerializeField] private UnitMover _mover;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _handPoint;

    public event Action<UnitWorker> Available;
    public event Action<UnitWorker, Vector3> BaseFounded;

    private ResourceReceiver _resourceReceiver;
    private UnitAnimator _unitAnimator;
    private bool _isBusy;

    public bool IsAvailable => _isBusy == false;

    private void Awake()
    {
        _unitAnimator = new UnitAnimator(_animator);
    }

    public void SetHomeBase(ResourceReceiver resourceReceiver)
    {
        if (resourceReceiver == null)
            return;

        _resourceReceiver = resourceReceiver;
    }

    public void StartCollecting(Resource resource)
    {
        if (IsAvailable == false)
            return;

        if (resource == null || _resourceReceiver == null)
            return;

        _isBusy = true;

        StartCoroutine(CollectResourceRoutine(resource));
    }

    public void StartFoundingBase(Transform destination)
    {
        if (IsAvailable == false)
            return;

        if (destination == null)
            return;

        _isBusy = true;

        StartCoroutine(FoundBaseRoutine(destination));
    }

    private IEnumerator CollectResourceRoutine(Resource resource)
    {
        _unitAnimator.SetState(UnitAnimationState.Run);
        yield return _mover.MoveToRoutine(resource.transform);

        if (resource == null)
        {
            ReleaseWorker();
            yield break;
        }

        _unitAnimator.SetState(UnitAnimationState.Collect);
        yield return _unitAnimator.FinishedRoutine(UnitAnimationState.Collect);

        resource.transform.SetParent(_handPoint);
        resource.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _unitAnimator.SetState(UnitAnimationState.Run);
        yield return _mover.MoveToRoutine(_resourceReceiver.transform);
        
        _resourceReceiver.Receive(resource);
        ReleaseWorker();
    }

    private IEnumerator FoundBaseRoutine(Transform destination)
    {
        _unitAnimator.SetState(UnitAnimationState.Run);
        yield return _mover.MoveToRoutine(destination);

        BaseFounded?.Invoke(this, destination.position);
        ReleaseWorker();
    }

    private void ReleaseWorker()
    {
        _isBusy = false;
        _unitAnimator.SetState(UnitAnimationState.Sit);

        Available?.Invoke(this);
    }
}
