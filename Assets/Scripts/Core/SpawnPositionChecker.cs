using UnityEngine;

public class SpawnPositionChecker : MonoBehaviour
{
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _checkRadius = 2f;

    public bool IsFree(Vector3 position)
    {
        return Physics.CheckSphere(position, _checkRadius, _obstacleMask) == false;
    }
}