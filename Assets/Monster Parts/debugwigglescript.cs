using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugwigglescript : MonoBehaviour
{
    GameObject legs;
    bool active;
    // Start is called before the first frame update
    void Start()
    {
        legs = transform.Find("Legs n Eyes V2").gameObject;

        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!active)
        {
            legs.GetComponent<leg>().ActivateLeg();
            legs.GetComponent<leg>().DebugFlail();
            active = true;
        }
    }
}
