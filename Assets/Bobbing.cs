using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var x = Mathf.Sin(Time.realtimeSinceStartup);

        var pos = transform.position;

        transform.position = new Vector3(x, pos.y, pos.z);
    }
}
