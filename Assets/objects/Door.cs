using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float hingeSpeed = 15f;
    private Transform hinge;
    private BoxCollider exitZone;

    private void Start()
    {
        exitZone = GetComponent<BoxCollider>();
        hinge = transform.GetChild(0).GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            if(hinge.transform.rotation.y <= 150)
            {
                hinge.transform.Rotate(0, Time.deltaTime * hingeSpeed, 0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            CanvasManager.loadMenu();
    }

    public void open()
    { 
        isOpen = true; 
        exitZone.enabled = true;
    }
}
