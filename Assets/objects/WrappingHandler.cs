using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappingHandler : MonoBehaviour
{
    void Awake()
    {
        // Add wrapping around object
        GameObject wrapping = Instantiate(Resources.Load("Wrapping"), transform) as GameObject;

        // Resize wrapping
        Collider transCol = transform.transform.Find("Mesh").GetComponent<Collider>();
        Collider wrapCol = wrapping.transform.Find("Mesh").GetComponent<Collider>();


        //float sizeDif = transCol.bounds.size - wrapCol.bounds.size;
        float xDif = transCol.bounds.max.x - wrapCol.bounds.max.x;
        float yDif = transCol.bounds.max.y - wrapCol.bounds.max.y;
        float zDif = transCol.bounds.max.z - wrapCol.bounds.max.z;
        Debug.Log(transCol.bounds.size);
        Debug.Log(wrapCol.bounds.size);

        float ratioX = (xDif + wrapCol.bounds.max.x) / wrapCol.bounds.max.x;
        float ratioY = (yDif + wrapCol.bounds.max.y) / wrapCol.bounds.max.y;
        float ratioZ = (zDif + wrapCol.bounds.max.z) / wrapCol.bounds.max.z;
        Debug.Log(ratioX);
        Debug.Log(ratioY);
        Debug.Log(ratioZ);

        float highest = ratioX;
        if (ratioY > highest) highest = ratioY;
        if (ratioZ > highest) highest = ratioZ;

        wrapping.transform.localScale = new Vector3(highest, highest, highest);
    }
    void Update()
    {
        // If hit against wall with force, shread wrapping & remove this script
    }
}
