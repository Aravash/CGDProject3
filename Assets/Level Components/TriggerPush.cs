using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPush : MonoBehaviour
{
    [SerializeField] Vector3 dir = Vector3.forward;
    [SerializeField] float force = 1;

    private void Start()
    {
        dir.Normalize();
        dir *= force;
    }

    private void OnTriggerStay(Collider col)
    {
        Rigidbody other = col.GetComponent<Rigidbody>();
        if (other)
        {
            other.AddForce(dir, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, dir.normalized);
    }
}
