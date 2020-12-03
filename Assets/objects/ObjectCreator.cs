using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectCreator : MonoBehaviour
{
    private Object[] models;

    void Start()
    {
        models = Resources.LoadAll("obj", typeof(Object));

        if(models.Length < 3) Debug.Log("Fucked up");
        
        foreach (var t in models)
        {
            Debug.Log(t.name);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("K pressed");
            int index = UnityEngine.Random.Range(0, models.Length);
            GameObject obj = (GameObject)Instantiate(models[index], transform.position, transform.rotation);
        }
    }
}
