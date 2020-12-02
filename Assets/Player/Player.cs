using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region vars
    // Camera
    private Transform playerView;
    const float playerViewYOffset = 0.7f; // The height at which the camera is bound to
    [SerializeField] float xMouseSensitivity = 30.0f;
    [SerializeField] float yMouseSensitivity = 30.0f;
    private float rotX = 0.0f;
    private float rotY = 0.0f;

    const float MV_ACCEL = 2f;
    const float MV_FRICTION = 0.5f;
    const float MV_AIR_FRICTION = 0.3f;

    bool grounded = true;

    Rigidbody rb;

    // GravityGun
    Rigidbody held_object = null;
    Vector3 grip_offset = Vector3.forward * 1.5f;
    const float PUSH_FORCE = 10;
    const float PULL_FORCE = 0.5f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Put the camera inside the capsule collider
        playerView = Camera.main.transform;
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);


        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        camUpdate();
        if (Input.GetMouseButtonDown(0))
        {
            launch();
        }
        if (Input.GetMouseButton(1) && (held_object == null))
        {
            grab();
        }
        if (Input.GetMouseButtonUp(1))
        {
            drop();
        }
    }

    private void LateUpdate()
    {
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);
    }

    private void FixedUpdate()
    {
        move();
        if (held_object)
        {
            pull();
        }
    }

    // Player movement
    private void move()
    {
        bool wish_jump = false;

        applyFriction();

        Vector3 wish_dir = new Vector3(0, 0, 0);
        if (Input.GetKey("w"))
        {
            wish_dir.z++;
        }
        if (Input.GetKey("s"))
        {
            wish_dir.z--;
        }
        if (Input.GetKey("d"))
        {
            wish_dir.x++;
        }
        if (Input.GetKey("a"))
        {
            wish_dir.x--;
        }
        wish_dir.Normalize();
        Vector3 acceleration = wish_dir;
        acceleration.x *= MV_ACCEL;
        acceleration.z *= MV_ACCEL;
        acceleration = transform.rotation * acceleration;
        rb.velocity += acceleration;
    }

    private void applyFriction()
    {
        Vector3 vel = rb.velocity;
        float speed = vel.magnitude;
        if (speed < 0.01)
        {
            vel.x = 0;
            vel.z = 0;
            rb.velocity = vel;
            return;
        }

        float friction = grounded ? MV_FRICTION : MV_AIR_FRICTION;
        float drop = speed * friction;

        float newspeed = speed - drop;
        if (newspeed < 0)
            newspeed = 0;
        newspeed /= speed;

        vel.x *= newspeed;
        vel.z *= newspeed;
        rb.velocity = vel;
    }

    private void camUpdate()
    {
        /* Camera rotation stuff, mouse controls this stuff */
        rotX -= Input.GetAxis("Mouse Y") * xMouseSensitivity * 0.02f;
        rotY += Input.GetAxis("Mouse X") * yMouseSensitivity * 0.02f;
        // Clamp the X rotation
        if (rotX < -90)
            rotX = -90;
        else if (rotX > 90)
            rotX = 90;
        transform.rotation = Quaternion.Euler(0, rotY, 0); // Rotates the collider
        playerView.rotation = Quaternion.Euler(rotX, rotY, 0); // Rotates the camera
    }

    // Gravity gun
    private void grab()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerView.position, playerView.forward, out hit, 100.0f))
        {
            Debug.DrawRay(playerView.position, playerView.forward * 100.0f, Color.white, 1);
            Rigidbody other = hit.collider.gameObject.GetComponent<Rigidbody>();
            if(other)
            {
                held_object = other;
            }
        }
    }

    private void drop()
    {
        held_object = null;
    }

    private void pull()
    {
        Vector3 dest = playerView.position + playerView.rotation * grip_offset;
        Vector3 diff = dest - held_object.gameObject.transform.position;
        held_object.AddForce(diff * PULL_FORCE, ForceMode.Impulse);

        Debug.DrawRay(held_object.gameObject.transform.position, diff, Color.green, Time.fixedDeltaTime);
    }

    private void launch()
    {
        if(held_object == null)
        {
            return;
        }
        Vector3 dir = playerView.transform.rotation * Vector3.forward * PUSH_FORCE;
        held_object.AddForce(dir, ForceMode.Impulse);
        held_object = null;
        Debug.DrawRay(playerView.transform.position, dir, Color.green, 1.5f);
    }
}
