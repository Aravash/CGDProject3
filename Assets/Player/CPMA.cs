using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

struct Cmd { public float forwardMove; public float rightMove; public float upMove; }

public class CPMA : MonoBehaviour
{
    private Transform playerView; // Camera
    public float playerViewYOffset = 0.7f; // The height at which the camera is bound to
    public float xMouseSensitivity = 30.0f;
    public float yMouseSensitivity = 30.0f;
    // /Frame occuring factors/
    public float gravity = 20.0f;
    public float friction = 6; //Ground friction - default 6
                               /* Movement stuff */
    public float moveSpeed = 6f;                // Ground move speed - default 6
    public float airMoveSpeed = 4f;                // Air move speed - default 6
    public float crouchMoveSpeed = 4f;                // Ground move speed - default 6
    public float runAcceleration = 14.0f;         // Ground accel - default 14
    public float runDeacceleration = 14.0f;       // Deacceleration that occurs when running on the ground - default 10
    public float airAcceleration = 2f;          // Air accel - default 2
    public float airDecceleration = 2f;         // Deacceleration experienced when ooposite strafing - default 2
    public float airControl = 10f;               // How precise air control is - default 0.3
    public float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed when
    public float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing
    public float moveScale = 1.0f;
    /*print() style */
    public GUIStyle style;
    /* Sound stuff */
    public AudioClip[] jumpSounds;
    /* FPS Stuff */
    public float fpsDisplayRate = 4.0f; // 4 updates per sec
    private int frameCount = 0;
    private float dt = 0.0f;
    private float fps = 0.0f;
    private CharacterController _controller;
    //private Rigidbody _controller;
    private bool isGrounded = false;
    // Camera rotations
    private float rotX = 0.0f;
    private float rotY = 0.0f;
    private Vector3 moveDirectionNorm = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;
    private float playerTopVelocity = 0.0f;
    /* Jumping */
    private bool wishJump = false;          // The Player can queue the next jump just before they hit the ground
    private bool dj_able = false;           //keeps track if the player has touched the ground since the last double jump
    private bool dj_active = false;         
    private float dj_timer = 0;
    public float jumpSpeed = 5.0f;               // The speed at which the character's up axis gains when hitting jump - default 8
    public float dj_duration = 1.0f;            //How long the player can float before the DoubleJump boosts them
    public float dj_speed = 10f;
    public float dj_boost = 6.0f;         //The speed to add to the player after the hover has ended
    public float dj_upSpeed = 6.0f;     //The players vertical speed after hover has ended
    public float dj_drag = 0.0f;
    // Crouching
    private bool crouching = false;
    // Wallrunning
    public float wr_checkRadius = 0.501f;   // How long to make the raycasts checking for wall contact. Must be larger than the radius of the player, but shouldn't be so large that the tangents of the player at each cast intersect with other casts within the cast length radius. 
    public float wr_maxDuration = 2f;       // How long the player can stay on the wall before dropping
    public float wr_wallkickVelocity = 6f;  // Velocity to add perpendicular to the wall when jumping during a wallrun
    public float wr_jumpUpSpeed = 5f;       // Vertical Velocity gained from jumping
    private float wr_timer = 0;
    private GameObject lastWallRef;         // reference to the last wall the player was attached to.
    private float lastWalltimer = 0;
    public float lastWallResetTime = 2;     // How long it takes to be able to wall run on the same wall twice in a row
    private bool onWall = false;            // whether the player is currently wall running
    public float wr_acceleration = 10.0f;   // Wallrun accel - default 14
    public float wr_heightGain = 1f;         // the vertical velocity of the player upon starting a wallrun
    public float wr_stage1_gravity = 20f;    // the value of gravity during the ascent on the wall
    public float wr_stage2_gravity = 0.8f;     // the value of gravity as the wallrun decays
    public float wr_frictionModi = 0.5f;       // What to modify friction by
    public float wr_moveSpeed = 6f;         // Ground move speed - default 6
    public float wr_minimumSpd = 10f;

    // Used to display real time fricton values
    private float playerFriction = 0.0f;
    // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)
    private Cmd _cmd;
    private void Start()
    {
        // Hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerView = Camera.main.transform;
        // Put the camera inside the capsule collider
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);
        _controller = GetComponent<CharacterController>();
        //_controller = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Do FPS calculation
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / fpsDisplayRate)
        {
            fps = Mathf.Round(frameCount / dt);
            frameCount = 0;
            dt -= 1.0f / fpsDisplayRate;
        }
        /* Ensure that the cursor is locked into the screen */
        if (Screen.lockCursor == false)
        {
            if (Input.GetMouseButtonDown(0))
                Screen.lockCursor = true;
        }
        if (Cursor.visible == true)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Cursor.visible = false;
            }
        }

        // LOAD PRESETS
        if(Input.GetKeyDown(KeyCode.I))
        {
            loadJSON(1);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            loadJSON(2);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            loadJSON(3);
        }



        /* Camera rotation stuff, mouse controls this stuff */
        rotX -= Input.GetAxis("Mouse Y") * xMouseSensitivity * 0.02f;
        rotY += Input.GetAxis("Mouse X") * yMouseSensitivity * 0.02f;
        // Clamp the X rotation
        if (rotX < -90)
            rotX = -90;
        else if (rotX > 90)
            rotX = 90;
        this.transform.rotation = Quaternion.Euler(0, rotY, 0); // Rotates the collider
        playerView.rotation = Quaternion.Euler(rotX, rotY, 0); // Rotates the camera

        /* Movement, here's the important part */
        if (!crouching && Input.GetKey(KeyCode.LeftShift))
            Crouch();
        else if(crouching && !Input.GetKey(KeyCode.LeftShift))
            Uncrouch();
        QueueJump();
        CheckGrounded();
        SetMovementDir();
        if (isGrounded)
            GroundMove();
        else
        {
            int wallDirID = wallrunCheck();
            if (wallDirID != -1)
                WallMove(wallDirID);
            else
                AirMove();
        }
        // Move the controller
        _controller.Move(playerVelocity * Time.deltaTime);
        //_controller.velocity = playerVelocity;
        
        // Calculate top velocity
        Vector3 udp = playerVelocity;
        udp.y = 0.0f;
        if (udp.magnitude > playerTopVelocity)
            playerTopVelocity = udp.magnitude;

        //set camera pos LAST to prevent jitter
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);
    }
    /*******************************************************************************************************\
    |* MOVEMENT
    \*******************************************************************************************************/
    /**
    * Sets the movement direction based on player input
    */
    private void SetMovementDir()
    {
        if (Input.GetKey("w") && Input.GetKey("s"))
            _cmd.forwardMove = 0;
        else if (!Input.GetKey("w") && !Input.GetKey("s"))
            _cmd.forwardMove = 0;
        else if (Input.GetKey("s"))
            _cmd.forwardMove = -1;
        else if (Input.GetKey("w"))
            _cmd.forwardMove = 1;


        if (Input.GetKey("d") && Input.GetKey("a"))
            _cmd.rightMove = 0;
        else if (!Input.GetKey("d") && !Input.GetKey("a"))
            _cmd.rightMove = 0;
        else if (Input.GetKey("a"))
            _cmd.rightMove = -1;
        else if (Input.GetKey("d"))
            _cmd.rightMove = 1;
    }
    /**
     * Queues the next jump just like in Q3
     */
    private void QueueJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !wishJump)
            wishJump = true;
        if (Input.GetKeyUp(KeyCode.Space))
            wishJump = false;
    }
    /**
     * Execs when the player is in the air
     */
    private void AirMove()
    {
        Vector3 wishdir;
        float accel;
        float scale = CmdScale();
        wishdir = new Vector3(_cmd.rightMove, 0, _cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);
        float wishspeed = wishdir.magnitude;
        wishspeed *= airMoveSpeed;
        wishdir.Normalize();
        moveDirectionNorm = wishdir;
        wishspeed *= scale;
        float wishspeed_old = wishspeed;
        if (Vector3.Dot(playerVelocity, wishdir) < 0)
            accel = airDecceleration;
        else
            accel = airAcceleration;
        // If the player is ONLY strafing left or right
        if (_cmd.forwardMove == 0 && _cmd.rightMove != 0)
        {
            if (wishspeed > sideStrafeSpeed)
                wishspeed = sideStrafeSpeed;
            accel = sideStrafeAcceleration;
        }
        Accelerate(wishdir, wishspeed, accel);
        if (airControl > 0)
            AirControl(wishdir, wishspeed_old);
        //doubleJump
        if (dj_able && wishJump)
        {
            if(playerVelocity.y<0)
                playerVelocity.y = 0;
            wishJump = false;
            dj_able = false;
            dj_active = true;
        }
        // Apply gravity or continue double jumping
        if (!dj_active)
            playerVelocity.y -= gravity * Time.deltaTime;
        else
            DoubleJumpTicker(wishdir);
    }
    /**
     * Air control occurs when the player is in the air, it allows
     * players to move side to side much faster rather than being
     * 'sluggish' when it comes to cornering.
     */
    private void AirControl(Vector3 wishdir, float wishspeed)
    {
        if (Mathf.Abs(_cmd.forwardMove) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
            return;

        float zspeed = playerVelocity.y;
        playerVelocity.y = 0;
        //idTech VectorNormalize()
        float speed = playerVelocity.magnitude;
        playerVelocity.Normalize();
        float dot = Vector3.Dot(playerVelocity, wishdir);
        float k = 32;
        k *= airControl * dot * dot * Time.deltaTime;
        // Change direction while slowing down
        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;
            playerVelocity.Normalize();
            moveDirectionNorm = playerVelocity;
        }
        playerVelocity.x *= speed;
        playerVelocity.y = zspeed;
        playerVelocity.z *= speed;
    }
    /**
     * Called every frame when the engine detects that the player is on the ground
     */
    private void GroundMove()
    {
        Vector3 wishdir;
        // Do not apply friction if the player is queueing up the next jump
        if (!wishJump)
            ApplyFriction(1.0f);
        else
            ApplyFriction(0);
        float scale = CmdScale();
        wishdir = new Vector3(_cmd.rightMove, 0, _cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        moveDirectionNorm = wishdir;
        float wishspeed = wishdir.magnitude; // 1
        if (crouching)
            wishspeed *= crouchMoveSpeed;
        else
            wishspeed *= moveSpeed;
        Accelerate(wishdir, wishspeed, runAcceleration);
        // Reset the gravity velocity
        playerVelocity.y = 0;   // TODO: Try gravity * dTime
        if (wishJump)
        {
            playerVelocity.y = jumpSpeed;
            wishJump = false;
            PlayJumpSound();
            // The player is now able to doubleJump
            dj_able = true;
        }
    }
    /**
     * Applies friction to the player, called in both the air and on the ground
     */
    private void ApplyFriction(float modifier)
    {
        Vector3 vec = playerVelocity; // Equivalent to: VectorCopy();
        float vel;
        float speed;
        float newspeed;
        float control;
        float drop;
        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;
        /* Only if the player is on the ground then apply friction */
        if (isGrounded)
        {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * friction * Time.deltaTime * modifier;
        }
        newspeed = speed - drop;
        playerFriction = newspeed;
        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;
        playerVelocity.x *= newspeed;
        // playerVelocity.y *= newspeed;
        playerVelocity.z *= newspeed;
    }
    private void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addspeed;
        float accelspeed;
        float currentspeed;
        currentspeed = Vector3.Dot(playerVelocity, wishdir);
        addspeed = wishspeed - currentspeed;
        if (addspeed <= 0)
            return;
        accelspeed = accel * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
            accelspeed = addspeed;
        playerVelocity.x += accelspeed * wishdir.x;
        playerVelocity.z += accelspeed * wishdir.z;
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 400, 100), "FPS: " + fps, style);
        var ups = _controller.velocity;
        ups.y = 0;
        int row = 15;
        GUI.Label(new Rect(0, row, 400, 100), "Speed: " + Mathf.Round(ups.magnitude * 100) / 100 + "ups", style);
        GUI.Label(new Rect(0, 2*row, 400, 100), "Top Speed: " + Mathf.Round(playerTopVelocity * 100) / 100 + "ups", style);
        GUI.Label(new Rect(0, 3 * row, 400, 100), "Velocity: " + Mathf.Round(playerVelocity.x * 10) / 10 + ", " + Mathf.Round(playerVelocity.y * 10) / 10 + ", " + Mathf.Round(playerVelocity.z * 10) / 10 + ", ", style);
        GUI.Label(new Rect(0, 4 * row, 400, 100), "Velocity X: " + Mathf.Round(playerVelocity.x), style);
        GUI.Label(new Rect(0, 5 * row, 400, 100), "Velocity Y: " + Mathf.Round(playerVelocity.y), style);
        GUI.Label(new Rect(0, 6 * row, 400, 100), "Velocity Z: " + Mathf.Round(playerVelocity.z), style);
        Vector3 inputDir = new Vector3(_cmd.rightMove, 0, _cmd.forwardMove);
        GUI.Label(new Rect(0, 7 * row, 400, 100), "Wish Dir: " + Mathf.Round(inputDir.x * 10) / 10 + ", 0, " + Mathf.Round(inputDir.z * 10) / 10 + ", ", style);
        GUI.Label(new Rect(0, 8 * row, 400, 100), "Grounded (script): " + isGrounded, style);
        GUI.Label(new Rect(0, 9 * row, 400, 100), "Grounded (controller): " + _controller.isGrounded, style);
        GUI.Label(new Rect(0, 10 * row, 400, 100), "On Wall: " + onWall, style);
        GUI.Label(new Rect(0, 11 * row, 400, 100), "On Wall: " + Mathf.Round(wr_timer * 10) / 10, style);

    }
    /*
    ============
    PM_CmdScale
    Returns the scale factor to apply to cmd movements
    This allows the clients to use axial -127 to 127 values for all directions
    without getting a sqrt(2) distortion in speed.
    ============
    */
    private float CmdScale()
    {
        int max;
        float total;
        float scale;
        max = (int)Mathf.Abs(_cmd.forwardMove);
        if (Mathf.Abs(_cmd.rightMove) > max)
            max = (int)Mathf.Abs(_cmd.forwardMove);
        if (max <= 0)
            return 0;
        total = Mathf.Sqrt(_cmd.forwardMove * _cmd.forwardMove + _cmd.rightMove * _cmd.rightMove);
        scale = moveSpeed * max / (moveScale * total);
        return scale;
    }
    /**
     * Plays a random jump sound
     */
    private void PlayJumpSound()
    {
        // Don't play a new sound while the last hasn't finished
        /*
        if (GetComponent<AudioSource>().isPlaying)
            return;
        GetComponent<AudioSource>().clip = jumpSounds[Random.Range(0, jumpSounds.Length)];
        GetComponent<AudioSource>().Play();
        */
    }
    
    private void CheckGrounded()
    {
        Ray downCast = new Ray(transform.position, -Vector3.up);

        if (!crouching)
            isGrounded = Physics.SphereCast(downCast, 0.49f, 0.52f, 9);
        else
            isGrounded = _controller.isGrounded;


        if (dj_active && isGrounded)
        {
            dj_able = true;
            dj_active = false;
            dj_timer = 0;
            lastWalltimer = 0;
            lastWallRef = null;
        }

    }

    private void DoubleJumpTicker(Vector3 wishdir)
    {
        if(playerVelocity.magnitude!=0)
        {
            float velY_store = playerVelocity.y;
            float newSpeed = playerVelocity.magnitude - dj_drag;
            playerVelocity *= newSpeed / playerVelocity.magnitude;
            playerVelocity.y = velY_store - gravity/4 * Time.deltaTime;
        }
        else
        {
            playerVelocity.y -= gravity/4 * Time.deltaTime;
        }

        dj_timer += Time.deltaTime;
        if (dj_timer > dj_duration || Input.GetKeyUp("space"))
        {
            //jump
            playerVelocity += wishdir * dj_boost;
            playerVelocity.y = dj_upSpeed;
            dj_active = false;
            dj_timer = 0;
        }
    }
    
    private void Crouch()
    {
        crouching = true;
        gameObject.transform.localScale = new Vector3(1, 0.5f, 1);
        Vector3 currentPos = gameObject.transform.position;
        if (isGrounded)
        {
            currentPos.y -= 1f;
        }
        else
        {
            currentPos.y += 1f;
        }
        gameObject.transform.position = currentPos;
    }

    // fix ground embedding
    private void Uncrouch()
    {

        crouching = false;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        Vector3 currentPos = gameObject.transform.position;
        if (isGrounded)
            currentPos.y += 0.5f;
        else
        {
            RaycastHit hit;
            //Ray downCast = new Ray(, );
            float distanceToGround = 1f;
            if (Physics.SphereCast(transform.position, 0.5f, -Vector3.up, out hit, 0.5f, 9))
            {
                distanceToGround = hit.distance;
            }
            currentPos.y += 1 - distanceToGround;
        }
    }

    // fix last wall 
    private int wallrunCheck()
    {
        if (crouching)
        {
            onWall = false;
            return -1;
        }
        // Have to hold forward to wallrun
        if(_cmd.forwardMove <= 0)
        {
            onWall = false;
            return -1;
        }

        int highestPrioWall_castID = -1;
        Vector3 dir;
        RaycastHit hit;
        for (int i = 0; i<22; ++i)
        {
            dir = GetWallraycastVector(i);
            if (Physics.Raycast(transform.position, dir, out hit, wr_checkRadius, 9))
            {
                highestPrioWall_castID = i;
                if (!onWall)
                {
                    // ignore the last wall the player was attached to
                    //if(hit.collider.gameObject == lastWallRef)
                    //{
                    //    highestPrioWall_castID = -1;
                    //}
                    //else
                    //{
                        beginWallRun();
                    //}
                }
                lastWallRef = hit.collider.gameObject;
                Debug.DrawRay(transform.position, dir, Color.yellow);
            }
            else
                Debug.DrawRay(transform.position, dir, Color.cyan);
        }
        if (highestPrioWall_castID == -1)
            onWall = false;
        else
            Debug.DrawRay(transform.position, GetWallraycastVector(highestPrioWall_castID), Color.red);
        return highestPrioWall_castID;
    }

    private void beginWallRun()
    {
        playerVelocity.y += wr_heightGain;
        onWall = true;
        wr_timer = wr_maxDuration;
    }

    // fix accel
    private void WallMove(int wallNormalID)
    {
        wr_timer -= Time.deltaTime;

        bool side_right;
        Vector3 wallNormal = GetWallraycastVector(wallNormalID);
        // Check is wall is on left or right
        Vector3 moveDir; 
        if(wallNormalID % 2 == 1)
        {
            // left side
            side_right = false;
            moveDir = Quaternion.Euler(0, 90, 0) * wallNormal;
        }
        else
        {
            // right side
            side_right = true;
            moveDir = Quaternion.Euler(0, -90, 0) * wallNormal;
        }


#if false
        Vector3 wishdir;
        // Do not apply friction if the player is queueing up the next jump
        if (!wishJump)
            ApplyFriction(wr_frictionModi);
        else
            ApplyFriction(0);
        float scale = CmdScale();
        wishdir = new Vector3(_cmd.rightMove, 0, _cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        var wishspeed = wishdir.magnitude; // move this before or after the dot product
        wishdir = wishdir * Vector3.Dot(moveDir, wishdir);
        Accelerate(wishdir, wishspeed, wr_acceleration);
#endif
        if(playerVelocity.y > 0)
            playerVelocity.y -= wr_stage1_gravity * Time.deltaTime;
        else
            playerVelocity.y -= wr_stage2_gravity * Time.deltaTime;

        if (playerVelocity.magnitude < wr_minimumSpd)
        {
            // TODO test this wallmove accel
            Accelerate(moveDir, wr_minimumSpd, wr_acceleration);
        }


        if (wishJump)
        {
            // Boost off wall
            playerVelocity += -wallNormal.normalized * wr_wallkickVelocity;
            playerVelocity.y = jumpSpeed;
            wishJump = false;
            PlayJumpSound();
            dj_able = true;
        }
    }

    private void lastWallTicker()
    {
        if(lastWallRef != null)
        {
            lastWalltimer -= Time.deltaTime;
            if (lastWalltimer < 0)
                lastWallRef = null;
        }
    }

    private Vector3 GetWallraycastVector(int id)
    {
        Vector3 dir = Vector3.forward;
        switch (id)
        {
            case 0:
                dir = Quaternion.Euler(0, 110, 0) * dir;
                break;
            case 1:
                dir = Quaternion.Euler(0, -110, 0) * dir;
                break;
            case 2:
                dir = Quaternion.Euler(0, 105, 0) * dir;
                break;
            case 3:
                dir = Quaternion.Euler(0, -105, 0) * dir;
                break;
            case 4:
                dir = Quaternion.Euler(0, 100, 0) * dir;
                break;
            case 5:
                dir = Quaternion.Euler(0, -100, 0) * dir;
                break;
            case 6:
                dir = Quaternion.Euler(0, 95, 0) * dir;
                break;
            case 7:
                dir = Quaternion.Euler(0, -95, 0) * dir;
                break;
            case 8:
                dir = Quaternion.Euler(0, 90, 0) * dir;
                break;
            case 9:
                dir = Quaternion.Euler(0, -90, 0) * dir;
                break;
            case 10:
                dir = Quaternion.Euler(0, 85, 0) * dir;
                break;
            case 11:
                dir = Quaternion.Euler(0, -85, 0) * dir;
                break;
            case 12:
                dir = Quaternion.Euler(0, 80, 0) * dir;
                break;
            case 13:
                dir = Quaternion.Euler(0, -80, 0) * dir;
                break;
            case 14:
                dir = Quaternion.Euler(0, 75, 0) * dir;
                break;
            case 15:
                dir = Quaternion.Euler(0, -75, 0) * dir;
                break;
            case 16:
                dir = Quaternion.Euler(0, 70, 0) * dir;
                break;
            case 17:
                dir = Quaternion.Euler(0, -70, 0) * dir;
                break;
            case 18:
                dir = Quaternion.Euler(0, 65, 0) * dir;
                break;
            case 19:
                dir = Quaternion.Euler(0, -65, 0) * dir;
                break;
            case 20:
                dir = Quaternion.Euler(0, 60, 0) * dir;
                break;
            case 21:
                dir = Quaternion.Euler(0, -60, 0) * dir;
                break;
        }
        dir = transform.TransformDirection(dir);
        return dir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + new Vector3(0, -0.52f, 0), 0.49f);
    }


    // PRESET LOADING
    private void loadJSON(int presetID)
    {
        string fileAddress;
        switch(presetID)
        {
            case 1: fileAddress = "/preset1.json";
                break;
            case 2: fileAddress = "/preset2.json";
                break;
            case 3: fileAddress = "/preset3.json";
                break;
            default:fileAddress = "/preset1.json";
                break;
        }

        if(!File.Exists(Application.dataPath + fileAddress))
        {
            Debug.Log("could not locate file " + Application.dataPath + fileAddress);
            return;
        }

        moveVars loaded_vars = JsonUtility.FromJson<moveVars>(File.ReadAllText(Application.dataPath + fileAddress));
        Debug.Log("Loaded JSON: " + Application.dataPath + fileAddress);

        intakeNewValues(loaded_vars);
    }

    private class moveVars
    {
        // /Frame occuring factors/
        public float gravity;
        public float friction; //Ground friction - default 6
                                   /* Movement stuff */
        public float moveSpeed;                // Ground move speed - default 6
        public float airMoveSpeed;                // Air move speed - default 6
        public float crouchMoveSpeed;                // Ground move speed - default 6
        public float runAcceleration;         // Ground accel - default 14
        public float runDeacceleration;       // Deacceleration that occurs when running on the ground - default 10
        public float airAcceleration;          // Air accel - default 2
        public float airDecceleration;         // Deacceleration experienced when ooposite strafing - default 2
        public float airControl;               // How precise air control is - default 0.3
        public float sideStrafeAcceleration;  // How fast acceleration occurs to get up to sideStrafeSpeed when
        public float sideStrafeSpeed;          // What the max speed to generate when side strafing
        public float moveScale;
        /* Jumping */
        public float jumpSpeed;               // The speed at which the character's up axis gains when hitting jump - default 8
        public float dj_duration;            //How long the player can float before the DoubleJump boosts them
        public float dj_speed;
        public float dj_boost;         //The speed to add to the player after the hover has ended
        public float dj_upSpeed;     //The players vertical speed after hover has ended
        public float dj_drag;
        // Wallrunning
        public float wr_checkRadius;   // How long to make the raycasts checking for wall contact. Must be larger than the radius of the player, but shouldn't be so large that the tangents of the player at each cast intersect with other casts within the cast length radius. 
        public float wr_maxDuration;       // How long the player can stay on the wall before dropping
        public float wr_wallkickVelocity;  // Velocity to add perpendicular to the wall when jumping during a wallrun
        public float wr_jumpUpSpeed;       // Vertical Velocity gained from jumping
        public float lastWallResetTime;     // How long it takes to be able to wall run on the same wall twice in a row
        public float wr_acceleration;   // Wallrun accel - default 14
        public float wr_heightGain;         // the vertical velocity of the player upon starting a wallrun
        public float wr_stage1_gravity;    // the value of gravity during the ascent on the wall
        public float wr_stage2_gravity;     // the value of gravity as the wallrun decays
        public float wr_frictionModi;       // What to modify friction by
        public float wr_moveSpeed;         // Ground move speed - default 6
        public float wr_minimumSpd;
    }

    private void intakeNewValues(moveVars new_vars)
    {
        gravity = new_vars.gravity;
        friction = new_vars.friction; 
        moveSpeed = new_vars.moveSpeed;        
        airMoveSpeed = new_vars.airMoveSpeed;     
        crouchMoveSpeed = new_vars.crouchMoveSpeed;  
        runAcceleration = new_vars.runAcceleration;  
        runDeacceleration = new_vars.runDeacceleration;
        airAcceleration = new_vars.airAcceleration;  
        airDecceleration = new_vars.airDecceleration; 
        airControl = new_vars.airControl;               
        sideStrafeAcceleration = new_vars.sideStrafeAcceleration;  
        sideStrafeSpeed = new_vars.sideStrafeSpeed;          
        moveScale = new_vars.moveScale;
        jumpSpeed = new_vars.jumpSpeed;    
        dj_duration = new_vars.dj_duration;  
        dj_speed = new_vars.dj_speed;
        dj_boost = new_vars.dj_boost;     
        dj_upSpeed = new_vars.dj_upSpeed;   
        dj_drag = new_vars.dj_drag;  
        wr_checkRadius = new_vars.wr_checkRadius; 
        wr_maxDuration = new_vars.wr_maxDuration;     
        wr_wallkickVelocity = new_vars.wr_wallkickVelocity;
        wr_jumpUpSpeed = new_vars.wr_jumpUpSpeed;     
        lastWallResetTime = new_vars.lastWallResetTime;   
        wr_acceleration = new_vars.wr_acceleration; 
        wr_heightGain = new_vars.wr_heightGain;      
        wr_stage1_gravity = new_vars.wr_stage1_gravity; 
        wr_stage2_gravity = new_vars.wr_stage2_gravity;
        wr_frictionModi = new_vars.wr_frictionModi;  
        wr_moveSpeed = new_vars.wr_moveSpeed;       
        wr_minimumSpd = new_vars.wr_minimumSpd;
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               