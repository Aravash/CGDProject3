using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrappingHandler : MonoBehaviour
{
    [HideInInspector] public bool allowBreaking;

    private Collider transCol;
    private Rigidbody rb;
    private float impactBreakForce;

    void Awake()
    {
        allowBreaking = false;
        rb = GetComponent<Rigidbody>();
        impactBreakForce = 10;

        // Add wrapping around object
        GameObject wrapping = Instantiate(Resources.Load("Wrapping/Wrapping"), transform) as GameObject;
        wrapping.name = "Wrapping";

        // Resize wrapping
        transCol = transform.transform.GetComponent<Collider>();
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

    public void SetColour(Chute.col_ids colour)
    {
        // Texture wrapping
        Renderer WrapRenderer = transform.Find("Wrapping").transform.Find("Mesh").GetComponent<Renderer>();
        Material mat = Resources.Load("Wrapping/Wrapping-Cyan", typeof(Material)) as Material;

        switch (colour)
        {
            case Chute.col_ids.BLUE:
                mat = Resources.Load("Wrapping/Wrapping-Blue", typeof(Material)) as Material;
                break;
            case Chute.col_ids.CYAN:
                mat = Resources.Load("Wrapping/Wrapping-Cyan", typeof(Material)) as Material;
                break;
            case Chute.col_ids.LIME:
                mat = Resources.Load("Wrapping/Wrapping-Green", typeof(Material)) as Material;
                break;
            case Chute.col_ids.PURPLE:
                mat = Resources.Load("Wrapping/Wrapping-Purple", typeof(Material)) as Material;
                break;
            case Chute.col_ids.RED:
                mat = Resources.Load("Wrapping/Wrapping-Red", typeof(Material)) as Material;
                break;
            case Chute.col_ids.YELLOW:
                mat = Resources.Load("Wrapping/Wrapping-Yellow", typeof(Material)) as Material;
                break;
        }

        WrapRenderer.material = mat;
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
        GameAudioManager.WrappingDestroy(this.transform.position);
        //shread wrapping (remove mesh layer, create pop particles, etc.) & remove this script
        Destroy(transform.Find("Wrapping").gameObject);
        Destroy(this);
    }
}
