using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Player : MonoBehaviour
{
    #region vars
    // Camera
    private Transform playerView;
    const float playerViewYOffset = 0.7f; // The height the camera is bound to
    [SerializeField] float xMouseSensitivity = 30.0f;
    [SerializeField] float yMouseSensitivity = 30.0f;
    private float rotX = 0.0f;
    private float rotY = 0.0f;
    [SerializeField] private float recoilOffset;
    [SerializeField] private float recoilForce = .3f;

    const float MV_ACCEL = 2.5f;
    const float MV_FRICTION = 0.5f;
    private const float MV_AIR_FRICTION = 0.3f;

    bool grounded = true;

    Rigidbody rb;

    // GravityGun
    Rigidbody held_object = null;

    GameObject held_item = null;
    Vector3 grip_offset = Vector3.forward * 2f;
    const float PUSH_FORCE = 15;
    const float PULL_FORCE = 0.1f;
    const float PULL_MAX_SPEED = 20f;
    const float ESCAPE_DRAG_MULT = 0.1f;
    const float FIRE_RANGE = 5;

    [SerializeField]private WeaponSway gunSway;
    [SerializeField]private ParticleSystem burst;
    [SerializeField]private ParticleSystem punt;

    // GravGun timers
    float grab_cd = 0;
    const float GRAB_CD = 0.3f;
    float launch_cd = 0;
    const float LAUNCH_CD = 0.5f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; //delete this
        // Put the camera inside the capsule collider
        playerView = Camera.main.transform;
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);
        
        rb = GetComponent<Rigidbody>();

        xMouseSensitivity = SensWriter.GetSens();
        yMouseSensitivity = SensWriter.GetSens();
    }

    void Update()
    {
        camUpdate();
        // GravGun inputs
        if (Input.GetMouseButtonDown(0) && launch_cd == 0)
        {
            fire();
        }
        if (Input.GetMouseButton(1) && (held_object == null) && grab_cd == 0)
        {
            grab();
        }
        if (Input.GetMouseButtonUp(1))
        {
            drop();
        }
        // GravGun timers
        if (launch_cd > 0)
        {
            launch_cd -= Time.deltaTime;
            if (launch_cd < 0)
                launch_cd = 0;
        }
        if (grab_cd > 0)
        {
            grab_cd -= Time.deltaTime;
            if (grab_cd < 0)
                grab_cd = 0;
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
        //wish_dir.Normalize();
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
        float changeX = Input.GetAxis("Mouse Y") * xMouseSensitivity * 0.02f;
        float changeY = Input.GetAxis("Mouse X") * yMouseSensitivity * 0.02f;
        rotX -= changeX;
        rotY += changeY;
        // Clamp the X rotation
        if (rotX < -90)
            rotX = -89;
        else if (rotX > 90)
            rotX = 89;
        transform.rotation = Quaternion.Euler(0, rotY, 0); // Rotates the collider
        playerView.rotation = Quaternion.Euler(rotX-recoilOffset, rotY, 0); // Rotates the camera
        recoilOffset = Mathf.Lerp(recoilOffset, 0, Time.deltaTime * 2f);
        gunSway.gunUpdate(new Vector3(changeY, changeX, 0));
    }

    // Gravity gun
    private void grab()
    {
        RaycastHit hit;
        Ray ray = new Ray(playerView.position, playerView.forward);
        if (Physics.Raycast(ray, out hit, 100.0f, 1))
        {
            Debug.DrawRay(playerView.position, playerView.forward * 100.0f, Color.white, 1);
            if(hit.collider.gameObject.GetComponent<Enemy>())
            {
                hit.collider.gameObject.GetComponent<Enemy>().DeactivateEnemy();
            }

            Rigidbody other = hit.collider.gameObject.GetComponent<Rigidbody>();
            
            if (other)
            {
                Renderer item = hit.rigidbody.gameObject.GetComponent<Renderer>();
                held_object = other;
                held_item = item.gameObject;
                held_item.GetComponent<Renderer>().materials[1].SetFloat("_Outline", 0.05f);
                held_object.useGravity = false;
                feelerManager.SetHold(true);
            }
        }
    }

    private void drop()
    {
        if (held_object)
        {
            held_object.useGravity = true;
            held_item.GetComponent<Renderer>().materials[1].SetFloat("_Outline", 0f);
        }
        
        held_object = null;
        feelerManager.SetHold(false);
    }

    private void pull()
    {
        Vector3 dest = playerView.position + playerView.rotation * grip_offset;
        Vector3 diff = dest - held_object.gameObject.transform.position;

        held_object.AddForce(diff * PULL_FORCE, ForceMode.Impulse);

        Debug.DrawRay(held_object.gameObject.transform.position, diff, Color.green, Time.fixedDeltaTime);

        //temporary solution
        if (held_object.GetComponent<Enemy>())
        {
            held_object.GetComponent<Enemy>().DeactivateEnemy();
        }
        
        // Truncate the object's vel
        float mag = Vector3.Dot(held_object.velocity, diff.normalized);
        if (mag > PULL_MAX_SPEED)
        {
            held_object.velocity *= PULL_MAX_SPEED / mag;
        }
       else if (mag < 0)
        {
            held_object.velocity *= ESCAPE_DRAG_MULT;
        }
    }

    private void fire()
    {
        // Throw the held object
        if (held_object != null)
        {
            float diff = (held_object.position - playerView.position).magnitude;
            if(diff < FIRE_RANGE)
            {
                held_object.useGravity = true;
                held_object.velocity *= 0;

                if (held_object.gameObject.GetComponent<ImpactDetector>())
                {
                    held_object.gameObject.GetComponent<ImpactDetector>().allowBreaking = true;
                }

                Vector3 dir = playerView.transform.rotation * Vector3.forward * PUSH_FORCE;
                held_object.AddForce(dir, ForceMode.Impulse);
                held_object = null;
                recoilOffset += recoilForce;
                burst.Play();
                feelerManager.SetHold(false);
                feelerManager.Fire();
                Debug.DrawRay(playerView.transform.position, dir, Color.green, 1.5f);
                // set the timers
                grab_cd = GRAB_CD;
                launch_cd = LAUNCH_CD;

                held_item.GetComponent<Renderer>().materials[1].SetFloat("_Outline", 0f);
            }
        }
        // else Punt the object in front
        else
        {
            RaycastHit hit;
            Ray ray = new Ray(playerView.position, playerView.forward);
            Debug.DrawRay(playerView.position, playerView.forward * FIRE_RANGE, Color.yellow, 1);
            if (Physics.Raycast(ray, out hit, FIRE_RANGE, 1))
            {
                Rigidbody other = hit.collider.gameObject.GetComponent<Rigidbody>();
                if (other)
                {
                    Vector3 dir = playerView.transform.rotation * Vector3.forward * PUSH_FORCE;
                    other.AddForceAtPosition(dir, hit.point, ForceMode.Impulse);
                    // set the timers
                    grab_cd = GRAB_CD;
                    launch_cd = LAUNCH_CD;
                    recoilOffset += recoilForce;
                    feelerManager.Fire();
                    punt.Play();
                }
            }
        }

        // TODO: make the launch force inversely proportional to dist
    }

    public bool hasHeldObject()
    {
        return held_object;
    }

    public Transform getHeldObjectTransform()
    {
        return held_object.transform;
    }
}
