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
    [SerializeField] const float ROT_FIRE_SPEED = 6f;
    [SerializeField] const float ROT_MAX_SPEED = 3f;
    [SerializeField] const float ROT_ACCEL = 1.5f;
    [SerializeField] const float ROT_FRICTION = 1f;

    public static feelerManager instance;

    private void Awake()
    {
        instance = this;
        origin_rot = transform.localEulerAngles.x;
        rot_speed = 0;
    }

    private void Update()
    {
        Debug.Log(rot_speed);
        if(grabbing)
        {
            rot_speed += ROT_ACCEL * Time.deltaTime;
            if (rot_speed > ROT_MAX_SPEED)
                rot_speed = ROT_MAX_SPEED;
        }
        else
        {
            applyFriction();
            //if (rot_speed > 10)
            //{
            //    rot_speed -= ROT_FRICTION * Time.deltaTime;
            //    rotate();
            //}
            //else
            //{
            //    rot_speed = 0;
            //    float x = Mathf.Lerp(transform.localEulerAngles.x, origin_rot, Time.deltaTime);
            //    setRot(x);
            //}
        }
        rotate();
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

    void rotate()
    {
        Debug.Log("ROT: " + rot_speed);
        transform.Rotate(Vector3.left, rot_speed);
        //transform.localEulerAngles = new Vector3(
        //    transform.localEulerAngles.x + rot_speed * Time.deltaTime,
        //    transform.localEulerAngles.y,
        //    transform.localEulerAngles.z);
    }
    //void setRot(float x)
    //{
    //    Debug.Log("X: " + x);
    //    transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
    //}
    
    void applyFriction()
    {
        if (rot_speed < 0.01)
        {
            rot_speed = 0;
            return;
        }
        
        float drop = rot_speed * ROT_FRICTION * Time.deltaTime;

        float newspeed = rot_speed - drop;
        if (newspeed < 0)
            newspeed = 0;
        newspeed /= rot_speed;

        rot_speed *= newspeed;
    }
}
