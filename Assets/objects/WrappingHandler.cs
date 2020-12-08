using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappingHandler : MonoBehaviour
{
    //private Collider transCol;
    private Rigidbody rb;

    void Awake()
    {
        Destroy(GetComponent<Rigidbody>());
        GetComponent<MeshCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        // Add wrapping around object
        GameObject wrapping = Instantiate(Resources.Load("Wrapping/Wrapping"), transform.position, transform.rotation) as GameObject;
        rb = wrapping.GetComponent<Rigidbody>();
        transform.SetParent(wrapping.transform);
        wrapping.name = "Wrapping";
        
        //wrapping.transform.Find("Mesh").GetComponent<Collider>().enabled = false;
    }

    public void PopWrapping(Vector3 newVelocity)
    {
        Debug.Log("pop wrapping called");
        GameAudioManager.WrappingDestroy(this.transform.position);

        //shread wrapping (remove mesh layer, create pop particles, etc.) & remove this script
        GetComponent<MeshCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        gameObject.AddComponent<Rigidbody>();
        GetComponent<Rigidbody>().velocity = newVelocity;
        Destroy(transform.parent.gameObject);
        gameObject.transform.SetParent(null);
        Destroy(this);
    }
}
