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
    [SerializeField] private Transform spot; 
    private LineRenderer _line;

    private void Start()
    {
        _line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        /*var position = transform.position;
        Vector3[] offset = {new Vector3(position.x, position.y, position.z + 1f),new Vector3(position.x, position.y, position.z + 2f)};
        _line.SetPosition(0, position + Vector3.forward);
        _line.SetPosition(1, position + (Vector3.forward * 2));*/
        
        if (_player.hasHeldObject())
        {
            Vector3 pos = _player.getHeldObjectTransform().position;
            rotateTo(pos); //inefficient? might want alternative
            _line.enabled = true;
            _line.SetPosition(2, transform.InverseTransformPoint(pos));
        }
        else
        {
            rotateTo(spot.position);
            _line.enabled = false;
            _line.SetPosition(2, Vector3.forward);
        }
    }

    void rotateTo(Vector3 target)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = target - transform.position;

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetDirection, lookAtSpeed*Time.deltaTime, 0.0f));
    }
}
