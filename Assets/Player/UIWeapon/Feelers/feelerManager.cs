using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feelerManager : MonoBehaviour
{

    [SerializeField] private Animator[] feelers;
    private static readonly int Grabbing = Animator.StringToHash("Grabbing");
    private static readonly int Shoot = Animator.StringToHash("Shoot");

    public static feelerManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static void SetHold(bool b)
    {
        foreach (var feeler in instance.feelers)
        {
            feeler.SetBool(Grabbing, b);
        }
    }

    public static void Fire()
    {
        foreach (var feeler in instance.feelers)
        {
            feeler.SetTrigger(Shoot);
        }
    }
}
