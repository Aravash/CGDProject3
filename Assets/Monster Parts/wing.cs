using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wing : MonoBehaviour
{
    public GameObject body;
    // Start is called before the first frame update
    void Start()
    {
        GameObject A = transform.Find("A").gameObject;
        GameObject B = transform.Find("B").gameObject;
        GameObject C = transform.Find("C").gameObject;
        GameObject D = transform.Find("D").gameObject;

        GameObject eyes = transform.Find("Eyes").gameObject;

        transform.position = body.transform.position;

        float w = body.GetComponent<Renderer>().bounds.size.x / 8;
        float h = body.GetComponent<Renderer>().bounds.size.y / 10;
        float de = body.GetComponent<Renderer>().bounds.size.z / 6;
        float eyepos = body.GetComponent<Renderer>().bounds.size.z / 2;

        A.transform.localPosition = new Vector3(-w, h, de);
        B.transform.localPosition = new Vector3(-w, h, -de);
        C.transform.localPosition = new Vector3(w, h, de);
        D.transform.localPosition = new Vector3(w, h, -de);

        eyes.transform.localPosition = new Vector3(0, 0, -eyepos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
