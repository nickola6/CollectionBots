using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class UnitMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;

    private Rigidbody _rigidbody;
    private WaitForFixedUpdate _waitForFixedUpdate;
    private Transform _target;
    private bool _targetReached;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;

        _waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_target == null)
            return;

        if (other.transform != _target)
            return;

        _targetReached = true;
    }

    public IEnumerator MoveToRoutine(Transform target)
    {
        _target = target;
        _targetReached = false;

        while (_targetReached == false)
        {
            Vector3 destination = target.position;
            destination.y = transform.position.y;

            Vector3 direction = destination - transform.position;

            if (direction.sqrMagnitude > 0f)
            {
                Vector3 normalizedDirection = direction.normalized;
                Vector3 movement = normalizedDirection * _moveSpeed * Time.fixedDeltaTime;

                _rigidbody.MovePosition(transform.position + movement);
                _rigidbody.MoveRotation(Quaternion.LookRotation(normalizedDirection));
            }

            yield return _waitForFixedUpdate;
        }

        _target = null;
    }
}