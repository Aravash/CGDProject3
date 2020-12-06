using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leg : MonoBehaviour
{

    //public GameObject A;
   // public GameObject B;
   // public GameObject C;
   // public GameObject D;

   // public GameObject eyes;

    /*
        back
         B D
         A C
        front
     */

     GameObject body;

    Animator a;
    Animator b;
    Animator c;
    Animator d;
    Animator e;

    // Start is called before the first frame update
    void Start()
    {
        //// assign body

        body = transform.parent.gameObject;


        ////

        GameObject A = transform.Find("A").gameObject;
        GameObject B = transform.Find("B").gameObject;
        GameObject C = transform.Find("C").gameObject;
        GameObject D = transform.Find("D").gameObject;

        GameObject eyes = transform.Find("Eyes").gameObject;

        transform.position = body.transform.position;

        a = A.GetComponent<Animator>();
        b = B.GetComponent<Animator>();
        c = C.GetComponent<Animator>();
        d = D.GetComponent<Animator>();
        e = eyes.GetComponent<Animator>();

        float w = body.GetComponent<Renderer>().bounds.size.x / 8;
        float h = body.GetComponent<Renderer>().bounds.size.y / 4;
        float de = body.GetComponent<Renderer>().bounds.size.z / 2;

        A.transform.localPosition = new Vector3(-w, -h, de);
        B.transform.localPosition = new Vector3(-w, -h, -de);
        C.transform.localPosition = new Vector3(w, -h, de);
        D.transform.localPosition = new Vector3(w, -h, -de);

        eyes.transform.localPosition = new Vector3(0, 0, -de-de/4);

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateLeg()
    {
        a.SetBool("Active", true);
        b.SetBool("Active", true);
        c.SetBool("Active", true);
        d.SetBool("Active", true);
        e.SetBool("Active", true);
    }

    public void DeactivateLeg()
    {
        a.SetBool("Active", false);
        b.SetBool("Active", false);
        c.SetBool("Active", false);
        d.SetBool("Active", false);
        e.SetBool("Active", false);
    }

    public void Flail()
    {
        a.SetBool("Flail", true);
        b.SetBool("Flail", true);
        c.SetBool("Flail", true);
        d.SetBool("Flail", true);
    }

    public void UnFlail()
    {
        a.SetBool("Flail", false);
        b.SetBool("Flail", false);
        c.SetBool("Flail", false);
        d.SetBool("Flail", false);
    }
}
