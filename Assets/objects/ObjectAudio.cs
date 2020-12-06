using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAudio : MonoBehaviour
{
    

    [SerializeField][FMODUnity.EventRef]
    private string FMODFallEvent = null;

    // Start is called before the first frame update
    void Start()
    {
        //eventEmitter.Event = FMODFallEvent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
