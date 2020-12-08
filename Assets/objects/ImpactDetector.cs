using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactDetector : MonoBehaviour
{
    public bool allowBreaking = false;
    [SerializeField] private float impactBreakForce = 2;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("velocity is " + GetComponent<Rigidbody>().velocity.magnitude + ", allow breaking is " + allowBreaking);
        // Detecting hard impact
        if (GetComponent<Rigidbody>().velocity.magnitude >= impactBreakForce && allowBreaking)
        {
            Debug.Log("pop condition met");
            transform.GetChild(0).GetComponent<WrappingHandler>().PopWrapping(GetComponent<Rigidbody>().velocity);
        }

        allowBreaking = false;
    }
}
