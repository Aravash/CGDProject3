using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_FMOD_OneShot : MonoBehaviour
{
    public bool fire;

    [SerializeField][FMODUnity.EventRef]
    private string FMODOneshotEvent = null;

    // Start is called before the first frame update
    void Start()
    {
        fire = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fire) { FireOneShot(); fire=false; }
    }

    void FireOneShot()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODOneshotEvent, transform.position);
    }
}
