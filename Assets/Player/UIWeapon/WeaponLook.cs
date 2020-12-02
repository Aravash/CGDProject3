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
    [SerializeField]private Player _player;
    [SerializeField]private Transform spot;

    // Update is called once per frame
    void Update()
    {
        if (_player.hasHeldObject())
        {
            rotateTo(_player.getHeldObjectTransform().position);//inefficient? might want alternative
        }
        else rotateTo(spot.position);
    }

    void rotateTo(Vector3 target)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = target - transform.position;

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDirection, lookAtSpeed*Time.deltaTime, 0.0f));
    }
}
