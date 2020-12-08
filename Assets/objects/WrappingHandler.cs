using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappingHandler : MonoBehaviour
{
    public bool allowBreaking = false;

    //private Collider transCol;
    private Rigidbody rb;
    [SerializeField] private float impactBreakForce = 5;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<MeshRenderer>().enabled = false;

        // Add wrapping around object
        GameObject wrapping = Instantiate(Resources.Load("Wrapping/Wrapping"), transform) as GameObject;
        wrapping.name = "Wrapping";

        // Resize wrapping
        //transCol = transform.transform.GetComponent<Collider>();
        wrapping.transform.Find("Mesh").GetComponent<Collider>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detecting hard impact
        if (rb.velocity.magnitude >= impactBreakForce && allowBreaking)
        {
            PopWrapping();
        }
        
        allowBreaking = false;
    }

    void PopWrapping()
    {
        //shread wrapping (remove mesh layer, create pop particles, etc.) & remove this script
        GetComponent<MeshRenderer>().enabled = true;
        Destroy(transform.Find("Wrapping").gameObject);
        Destroy(this);
    }
}
