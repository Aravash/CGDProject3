using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * weapon aesthetic rotations towards objects you are holding, (soon) or objects that you are in range to pickup
 */

public class WeaponLook : MonoBehaviour
{
    [SerializeField] private float lookAtSpeed = 1.0f;
    [SerializeField] private Player _player;
    [SerializeField] private Transform muzzleSpot;
    [SerializeField] private Transform muzzleSpot2;
    [SerializeField] private Transform spot; 
    private LineRenderer _line;

    private void Start()
    {
        _line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player.hasHeldObject())
        {
            Vector3 pos = _player.getHeldObjectTransform().position;
            RotateTo(pos); //inefficient? might want alternative
            _line.enabled = true;
            _line.SetPosition(2, transform.InverseTransformPoint(pos));
            _line.SetPosition(1, transform.InverseTransformPoint(muzzleSpot2.position));
            _line.SetPosition(0, transform.InverseTransformPoint(muzzleSpot.position));
        }
        else
        {
            RotateTo(spot.position);
            _line.enabled = false;
        }
    }

    void RotateTo(Vector3 target)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = target - transform.position;

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDirection, lookAtSpeed*Time.deltaTime, 0.0f));
    }
}
