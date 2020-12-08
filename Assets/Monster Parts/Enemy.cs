using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    GameObject legs;
    Rigidbody rb;

    //public GameObject Arena;
    //Vector3 randomPos;

    public bool active;
    //CharacterController controller;
    //BoxCollider box;
    //NavMesh navmesh;
    NavMeshAgent agent;

    Vector3 moveVector;

    bool fixRot;
    GameObject movingTarget;

    public GameObject target;

    GameObject[] endPoints;
    float waitTime;
    float minTime = 5.0f;
    float maxTime = 10.0f;
    public bool endChosen;
    public GameObject ep;

    bool yeeting;

    bool grabbed;

    float idle_timer = IDLE_TIME;
    const float IDLE_TIME = 2;

    bool doonce;
    bool tryingToEnable;

    bool tryingToActivate;

    const float maxFailTime = 5;
    public float jumpFailTimer;

    public bool tryingToRight;

    bool yeetSelfed;
    bool selfRighted;

    //bool doJump;
    //bool doFlip;

    // Start is called before the first frame update
    void Start()
    {

        legs = Instantiate(Resources.Load("Enemy/Legs n Eyes V2") as GameObject);
        legs.transform.localEulerAngles = new Vector3(0, 180, 0);
        legs.transform.parent = gameObject.transform;

        //legs = transform.Find("Legs n Eyes V2").gameObject;
        rb = GetComponent<Rigidbody>();

        //agent = GetComponent<NavMeshAgent>();
        movingTarget = new GameObject("target");
        target = movingTarget;
        ep = new GameObject("endpoint");
        ChooseWanderPoint();
        fixRot = false;
        grabbed = false;
        endPoints = GameObject.FindGameObjectsWithTag("EnemyWaypoints");
        waitTime = Random.Range(minTime, maxTime);
        endChosen = false;
        yeeting = false;
        doonce = false;
        tryingToEnable = false;
        tryingToActivate = false;
        jumpFailTimer = maxFailTime;
        tryingToRight = false;
        yeetSelfed = false;
        selfRighted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ActivateEnemy();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DeactivateEnemy();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            Grab();
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            Ungrab();
        }



        if (rb.velocity.magnitude > 0f)
        {
            idle_timer = IDLE_TIME;
        }
        else
        {
            idle_timer -= Time.deltaTime;
        }
        if (idle_timer <= 0 && !doonce)
        {
            //Debug.Log("I AWAKEN!!!!");
            doonce = true;
            ActivateEnemy();
            
        }

        // abort here if we aren't active
        if (!active)
            return;

        /*
        ground = controller.isGrounded;

        Vector3 targPos = new Vector3(randomPos.x, 0, randomPos.z);
        Vector3 currPos = new Vector3(transform.position.x, 0, transform.position.z);

        if(Vector3.Distance(targPos, currPos) < 0.25f)
        {
            Collider arena = Arena.GetComponent<Collider>();
            randomPos = new Vector3(Random.Range(arena.bounds.min.x, arena.bounds.max.x),
            transform.position.y,
            Random.Range(arena.bounds.min.z, arena.bounds.max.z));
        }

        g.transform.position = randomPos;
        */
        if(target != null)
        {
            Vector3 enemyPos = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 tar = new Vector3(target.transform.position.x, 0, target.transform.position.z);



            if (Vector3.Distance(tar, enemyPos) < 0.5f)
            {
                if (endChosen && target.transform.position == ep.transform.position && !yeetSelfed)
                {
                    //and target = the chosen endpoint
                    YeetSelf();
                    yeetSelfed = true;
                }
                else
                {
                    ChooseWanderPoint();
                }
            }
        }


        if (active && !grabbed && !yeeting && EvenCloserCheck() &&agent.enabled)
        {
            agent.destination = target.transform.position;

            waitTime -= Time.deltaTime;
        }

        if (fixRot)
        {
            rb.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, Time.deltaTime * 500);
        }

        if (waitTime <= 0 && !endChosen)
        {
            ChooseEndpoint();
        }

        if(tryingToEnable && CloseEnoughCheck())
        {
            EnableAgent();
        }
        if(tryingToActivate)
        {
            ActivateEnemy();
        }

        if(yeeting)
        {
            jumpFailTimer -= Time.deltaTime;
        }
        if(jumpFailTimer<= 0)
        {
            tryingToRight = true;
            yeetSelfed = false;
        }
        if(tryingToRight)
        {
            JumpFailure();
        }

    }
/*
    private void FixedUpdate()
    {
        if(doJump)
        {
            rb.velocity = transform.forward * 2 + new Vector3(0, 5, 0);
            doJump = false;
        }

        if(doFlip)
        {
            rb.AddForce(0, 150, 0);
            doFlip = false;
        }
    }
    */

    void ActivateEnemy()
    {
        // rb.AddForce(0, 150, 0);
        // rb.rotation = Quaternion.identity;
        //rb.constraints = RigidbodyConstraints.FreezeRotationX;
        //rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        // rb.isKinematic = true;

        //agent.enabled = true;

        /*  controller.enabled = true;
          box.enabled = false;
          if (rb != null)
          {
              Destroy(rb);
          }
          */

        //jump
        // moveVector.y = Vector3.up.y * 3;
        // transform.rotation = Quaternion.identity;


        //legs.GetComponent<leg>().ActivateLeg();

        /* Collider arena = Arena.GetComponent<Collider>();
         randomPos = new Vector3(Random.Range(arena.bounds.min.x, arena.bounds.max.x),
         transform.position.y,
         Random.Range(arena.bounds.min.z, arena.bounds.max.z)); */
         if(agent == null)
        {
            CreateNavAgent();
        }

         if(CloseEnoughCheck())
        {
            tryingToActivate = false;
            StartCoroutine("Activation");

        }
         else
        {
            tryingToActivate = true;
        }


        
    }

    public void DeactivateEnemy()
    {
        agent.enabled = false;
        /*
        box.enabled = true;
        controller.enabled = false;
        if(rb == null)
        {
            rb = this.gameObject.AddComponent<Rigidbody>();
        }
        */

        // rb.constraints = RigidbodyConstraints.None;
        // rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        rb.isKinematic = false;
        legs.GetComponent<leg>().DeactivateLeg();
        active = false;
        doonce = false;
        waitTime = Random.Range(minTime, maxTime);
        ChooseWanderPoint();
    }

    IEnumerator Activation()
    {
        rb.AddForce(0, 150, 0);
        //doFlip = true;
        fixRot = true;
        legs.GetComponent<leg>().ActivateLeg();
        yield return new WaitForSeconds(0.25f);
        fixRot = false;
        rb.rotation = Quaternion.identity;
        
        yield return new WaitForSeconds(0.3f);
        rb.isKinematic = true;

        //agent.enabled = true;
        tryingToEnable = true;
        active = true;
    }

    public void Grab()
    {
        if(active && !grabbed)
        {
            //readd rb, remove nav, wait, reactivate????
            grabbed = true;
            legs.GetComponent<leg>().Flail();
            agent.enabled = false;
            rb.isKinematic = false;
            yeeting = false;

            //after grab start jumptimer and stuff it'll work just as well
        }

        
    }

    public void Ungrab()
    {
        //maybe wait 5s?
        if(grabbed)
        {
            StartCoroutine("Wait5s");
        }

    }

    IEnumerator Wait5s()
    {
        yield return new WaitForSeconds(5);
        
        legs.GetComponent<leg>().UnFlail();

        rb.AddForce(0, 150, 0);
        //doFlip = true;
        fixRot = true;
        //legs.GetComponent<leg>().ActivateLeg();
        yield return new WaitForSeconds(0.25f);
        fixRot = false;
        rb.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.3f);
        rb.isKinematic = true;

        //agent.enabled = true;
        tryingToEnable = true;
        grabbed = false;
    }

    void ChooseEndpoint()
    {
        int index = Random.Range(0, endPoints.Length);
        ep.transform.position = endPoints[index].transform.position;
        ep.transform.rotation = endPoints[index].transform.rotation;
        target = ep;
        endChosen = true;
    }

    void ChooseWanderPoint()
    {
        ///???
        ///
        /*
        float walkRadius = 24;
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        target.position = finalPosition;
        movingTarget.position = finalPosition;
        */

        float walkRadius = 12;
        Vector3 newRandomPos = new Vector3 (Random.insideUnitSphere.x * walkRadius, transform.position.y, Random.insideUnitSphere.z * walkRadius);
        NavMeshHit hit;
        if(NavMesh.SamplePosition(newRandomPos, out hit, walkRadius, 1))
        {
            Vector3 finalPosition = hit.position;
            target.transform.position = finalPosition;
            movingTarget.transform.position = finalPosition;
        }
    }

    void YeetSelf()
    {
        jumpFailTimer = maxFailTime;
        yeeting = true;
        agent.enabled = false;
        transform.rotation = ep.transform.rotation;

        rb.isKinematic = false;
        rb.velocity = (transform.forward * 2) + new Vector3(0, 7, 0);
        //doJump = true;
        legs.GetComponent<leg>().Flail();
        selfRighted = false;


        //destroy target and ep
        Destroy(target);
        Destroy(ep);
    }

    bool CloseEnoughCheck()
    {
        //do this every time we enable agent, repeat until true
        //only move if this is true
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.5f, 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void EnableAgent()
    {
        tryingToEnable = false;
        agent.enabled = true;
    }

    void JumpFailure()
    {
        yeeting = false;
        //basically do activate 
        if (CloseEnoughCheck())
        {
            tryingToRight = false;
            if(!selfRighted)
            {
                StartCoroutine("RightSelf");
                //remaketarget????
                movingTarget = new GameObject("target");
                target = movingTarget;
                ep = new GameObject("endpoint");
                selfRighted = true;
            }

        }
        else
        {
            tryingToRight = true;
        }
    }

    IEnumerator RightSelf()
    {
        rb.AddForce(0, 150, 0);
        
        fixRot = true;
        legs.GetComponent<leg>().UnFlail();
        yield return new WaitForSeconds(0.25f);
        fixRot = false;
        rb.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.3f);
        rb.isKinematic = true;

        //agent.enabled = true;
        tryingToEnable = true;
        active = true;
        jumpFailTimer = maxFailTime;
        waitTime = Random.Range(minTime, maxTime);
        endChosen = false;
        target.transform.position = movingTarget.transform.position;
        ChooseWanderPoint();
        ep.transform.position = new Vector3(0,0,0);

    }

    bool EvenCloserCheck()
    {
        //do this every time we enable agent, repeat until true
        //only move if this is true
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CreateNavAgent()
    {
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.enabled = false;
        agent.baseOffset = 0.3f;
        agent.height = 0.5f;
        agent.angularSpeed = 500f;
    }

}
