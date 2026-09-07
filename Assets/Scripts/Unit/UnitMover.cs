using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnitMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;

    public event Action<Transform> TargetReached;

    private Rigidbody _rigidbody;
    private WaitForFixedUpdate _waitForFixedUpdate;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;

        _waitForFixedUpdate = new WaitForFixedUpdate();
    }

    public IEnumerator MoveToRoutine(Transform target)
    {
        if (target == null)
            yield break;

        bool targetReached = false;

        while (targetReached == false && target != null)
        {
            Vector3 destination = target.position;
            destination.y = transform.position.y;

            Vector3 direction = destination - transform.position;
            float distance = direction.magnitude;

            if (distance <= _moveSpeed * Time.fixedDeltaTime)
            {
                _rigidbody.MovePosition(destination);
                TargetReached?.Invoke(target);
                targetReached = true;

                continue;
            }

            Move(direction);

            yield return _waitForFixedUpdate;
        }
    }

    private void Move(Vector3 direction)
    {
        Vector3 normalizedDirection = direction.normalized;
        Vector3 movement = normalizedDirection * _moveSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(transform.position + movement);

        if (normalizedDirection.sqrMagnitude > 0f)
            _rigidbody.MoveRotation(
                Quaternion.LookRotation(normalizedDirection));
    }
}