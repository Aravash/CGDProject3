using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappingHandler : MonoBehaviour
{
    void Awake()
    {
        // Add wrapping around object
        GameObject wrapping = Instantiate(Resources.Load("Wrapping"), transform) as GameObject;
        wrapping.name = "Wrapping";

        // Resize wrapping
        Collider transCol = transform.transform.GetComponent<Collider>();
        Collider wrapCol = wrapping.transform.Find("Mesh").GetComponent<Collider>();
        
        float xDif = transCol.bounds.size.x - wrapCol.bounds.size.x;
        float yDif = transCol.bounds.size.y - wrapCol.bounds.size.y;
        float zDif = transCol.bounds.size.z - wrapCol.bounds.size.z;

        float wrapPadding = 1.2f;
        float ratioX = ((xDif + wrapCol.bounds.size.x) * wrapPadding) / wrapCol.bounds.size.x;
        float ratioY = ((yDif + wrapCol.bounds.size.y) * wrapPadding) / wrapCol.bounds.size.y;
        float ratioZ = ((zDif + wrapCol.bounds.size.z) * wrapPadding) / wrapCol.bounds.size.z;

        float highest = ratioX;
        if (ratioY > highest) highest = ratioY;
        if (ratioZ > highest) highest = ratioZ;

        wrapping.transform.localScale = new Vector3(highest, highest, highest);

        wrapCol.enabled = false;
    }
    void Update()
    {
        //If hit against wall with force:
        //Test code
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P pressed");
            //shread wrapping (remove mesh layer, create pop particles, etc.) & remove this script

            Destroy(transform.Find("Wrapping").gameObject);
            Destroy(this);
        }
    }
}
