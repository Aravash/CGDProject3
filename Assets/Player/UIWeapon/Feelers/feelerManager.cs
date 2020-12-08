using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feelerManager : MonoBehaviour
{

    [SerializeField] private Animator[] feelers;
    private static readonly int Grabbing = Animator.StringToHash("Grabbing");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private bool grabbing = false;


    private float origin_rot;
    private float rot_speed;
    [SerializeField] const float ROT_FIRE_SPEED = 50f;
    [SerializeField] const float ROT_MAX_SPEED = 20f;
    [SerializeField] const float ROT_ACCEL = 1f;
    [SerializeField] const float ROT_DECEL = 1f;

    public static feelerManager instance;

    private void Awake()
    {
        instance = this;
        origin_rot = transform.rotation.eulerAngles.x;
        rot_speed = 0;
    }

    private void Update()
    {
        if(grabbing)
        {
            rot_speed += ROT_ACCEL;
            if (rot_speed > ROT_MAX_SPEED)
                rot_speed = ROT_MAX_SPEED;
            setXRot(transform.rotation.x + rot_speed);
        }
        else
        {
            rot_speed -= ROT_DECEL;
            if(rot_speed < 10)
            {
                float x = Mathf.SmoothStep(transform.rotation.eulerAngles.x, origin_rot, Time.deltaTime);
                setXRot(x);
            }
            else
            {
                setXRot(transform.rotation.x + rot_speed);
            }
        }
    }

    public static void SetHold(bool b)
    {
        foreach (var feeler in instance.feelers)
        {
            feeler.SetBool(Grabbing, b);
        }
        instance.grabbing = b;
    }

    public static void Fire()
    {
        foreach (var feeler in instance.feelers)
        {
            feeler.SetTrigger(Shoot);
        }
        instance.rot_speed = ROT_FIRE_SPEED;
    }

    void setXRot(float x)
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = new Vector3(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = q;
    }
}
