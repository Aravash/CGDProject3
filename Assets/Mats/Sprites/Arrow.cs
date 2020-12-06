using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    private float maxBobTime = .5f;
    private float bobTime = .5f;
    private int dir = -1;
    public float speed = .5f;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerCam);
        bob();
    }

    void bob()
    {
        if (bobTime <= 0)
        {
            bobTime = maxBobTime;
            dir *= -1;
        }
        else bobTime -= Time.deltaTime;

        transform.position = Vector3.Lerp( transform.position, transform.position + Vector3.up * dir, Time.deltaTime * speed);
    }
}
