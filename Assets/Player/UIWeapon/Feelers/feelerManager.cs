using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feelerManager : MonoBehaviour
{

    [SerializeField] private Animator[] feelers;
    private static readonly int Grabbing = Animator.StringToHash("Grabbing");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    [SerializeField] private float normalRotateSpeed = 10f;
    [SerializeField] private float grabbedRotateSpeed = 20f;
    [SerializeField] private float shootRotateSpeed = 50f;
    private float originXRot;
    private float rotateSpeed;
    private bool grabbing = false;

    public static feelerManager instance;

    private void Awake()
    {
        instance = this;
        originXRot = transform.rotation.x;
        rotateSpeed = normalRotateSpeed;
    }

    private void Update()
    {
        if (transform.rotation.eulerAngles.x - originXRot > .5f)
        {
            transform.Rotate(Vector3.left * (rotateSpeed * Time.deltaTime));
        }

        if (transform.rotation.eulerAngles.x > 180)
        {
            Quaternion quat = new Quaternion();
            quat.eulerAngles = new Vector3( 90, transform.rotation.y, transform.rotation.z);
            transform.rotation = quat;

        }
        if (!grabbing) rotateSpeed = Mathf.Lerp(rotateSpeed, normalRotateSpeed, Time.deltaTime);
    }

    public static void SetHold(bool b)
    {
        foreach (var feeler in instance.feelers)
        {
            feeler.SetBool(Grabbing, b);
        }
        instance.grabbing = b;
        instance.rotateSpeed = instance.grabbedRotateSpeed;
    }

    public static void Fire()
    {
        foreach (var feeler in instance.feelers)
        {
            feeler.SetTrigger(Shoot);
        }

        instance.rotateSpeed = instance.shootRotateSpeed;
    }
}
