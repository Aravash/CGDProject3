using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : MonoBehaviour
{
    //COMPONENTS
    GameObject legs;
    Rigidbody rb;
    NavMeshAgent agent;

    //TARGETS
    GameObject[] endPoints;
    GameObject ep;
    GameObject wanderTarget;

    //target related bools
    bool endChosen;

    //TIMERS
    //activate/reactivate
    float idle_timer = IDLE_TIME;
    const float IDLE_TIME = 2;

    //how long after jumping until I right myself
    const float maxFailTime = 5;
    public float jumpFailTimer = maxFailTime;

    //how long do I wander until going to end
    float waitTime;
    float minTime = 5.0f;
    float maxTime = 10.0f;

    //states
    bool activating;
    bool active;
    bool haveJumped;

    bool fixRot;

    bool tryingToCreateAgent;
    bool tryingToEnable;

    void Start()
    {
        //turn into enemy
        legs = Instantiate(Resources.Load("Enemy/Legs n Eyes V2") as GameObject);
        legs.transform.localEulerAngles = new Vector3(0, 180, 0);
        legs.transform.parent = gameObject.transform;

        rb = GetComponent<Rigidbody>();

        ep = new GameObject("endpoint");
        wanderTarget = new GameObject("wander target");
        endPoints = GameObject.FindGameObjectsWithTag("EnemyWaypoints");

        tryingToCreateAgent = true;

    }

    // Update is called once per frame
    void Update()
    {
        //do idle -> activate
        if(!active && !activating &&!haveJumped)
        {
            if (rb.velocity.magnitude > 0f)
            {
                idle_timer = IDLE_TIME;
            }
            else
            {
                idle_timer -= Time.deltaTime;
            }
            if (idle_timer <= 0 && CloseEnoughCheck())
            {
                //Debug.Log("I AWAKEN!!!!");
                ActivateEnemy();
            }
        }

        //if I'm active I move
        //if endpoint chosen go to end, otherwise go to wanderpoint
        if (active && agent.enabled && !haveJumped)
        {
            if(endChosen)
            {
                agent.destination = ep.transform.position;
                //if I get there do jump
                Vector3 enemyPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 tar = new Vector3(ep.transform.position.x, 0, ep.transform.position.z);
                if (Vector3.Distance(tar, enemyPos) < 0.5f)
                {
                    Jump();
                }
            }
            else
            {
                agent.destination = wanderTarget.transform.position;
                //if I get there change target
                Vector3 enemyPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 tar = new Vector3(wanderTarget.transform.position.x, 0, wanderTarget.transform.position.z);
                if(Vector3.Distance(tar, enemyPos) < 0.5f)
                {
                    ChooseWanderPoint();
                }

                //countdown wander timer
                waitTime -= Time.deltaTime;
                if(waitTime <= 0)
                {
                    //choose end and go there
                    ChooseEndpoint();
                }


            }
        }

        if(haveJumped)
        {
            //timer for righting self
            //IF GRABBED MUST BE RESET OR SOMETHING
            jumpFailTimer -= Time.deltaTime;
            if(jumpFailTimer <= 0 && CloseEnoughCheck())
            {
                RightSelf();
            }
        }


        //deal with rotation for anim
        if (fixRot)
        {
            rb.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, Time.deltaTime * 500);
        }

        //add agent as soon as close enough
        if(tryingToCreateAgent && CloseEnoughCheck())
        {
            CreateNavAgent();
            tryingToCreateAgent = false;
        }


    }





    void ActivateEnemy()
    {
        //create nav agent, set active, reset timers, choose wander point, 
        //do jump, fix rotation, activate legs 
        activating = true;
 
        rb.AddForce(0, 150, 0);
        StartCoroutine("ActivateAnim");

        ResetIdleTimer();
        SetWanderTimer();
        ChooseWanderPoint();
        endChosen = false;
        EnableAgent();
        active = true;
        activating = false;
    }

    IEnumerator ActivateAnim()
    {

        //doFlip = true;
        fixRot = true;
        legs.GetComponent<leg>().ActivateLeg();
        yield return new WaitForSeconds(0.25f);
        fixRot = false;
        rb.rotation = Quaternion.identity;
        yield return new WaitForSeconds(0.3f);
        rb.isKinematic = true;
    }

    public void DeactivateEnemy()
    {

        DeactivateLogic();
    }

    void DeactivateLogic()
    {
        //disable agent, enable rb, deactivate leg anim, active = false
        //also endpoint isn't chosen
        //happens when grabbed!!! so reset jump timers and stuff
        haveJumped = false;
        ResetJumpTimer();
        agent.enabled = false;
        rb.isKinematic = false;
        legs.GetComponent<leg>().DeactivateLeg();
        legs.GetComponent<leg>().UnFlail();
        endChosen = false;
        active = false;
    }

    void Jump()
    {
        //disable agent, fix up rb, set anim, do jump
        haveJumped = true;
        ResetJumpTimer();

        agent.enabled = false;
        transform.rotation = ep.transform.rotation;

        rb.isKinematic = false;
        rb.velocity = (transform.forward * 2) + new Vector3(0, 7, 0);
        legs.GetComponent<leg>().Flail();
    }

    void RightSelf()
    {
        //I tried to jump but failed
        //prettymuch reactivate w different anim
        haveJumped = false;
        activating = true;
        if (agent == null)
        {
            CreateNavAgent();
        }

        StartCoroutine("RightSelfAnim");

        ResetIdleTimer();
        SetWanderTimer();
        ChooseWanderPoint();
        endChosen = false;
        EnableAgent();
        active = true;
        activating = false;
    }

    IEnumerator RightSelfAnim()
    {
        rb.AddForce(0, 150, 0);
        //doFlip = true;
        fixRot = true;
        yield return new WaitForSeconds(0.25f);
        fixRot = false;
        rb.rotation = Quaternion.identity;
        yield return new WaitForSeconds(0.3f);
        rb.isKinematic = true;
        legs.GetComponent<leg>().UnFlail();
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

    void ChooseEndpoint()
    {
        int index = Random.Range(0, endPoints.Length);
        ep.transform.position = endPoints[index].transform.position;
        ep.transform.rotation = endPoints[index].transform.rotation;
        endChosen = true;
    }

    void ChooseWanderPoint()
    {
            float walkRadius = 12;
            Vector3 newRandomPos = new Vector3(Random.insideUnitSphere.x * walkRadius, transform.position.y, Random.insideUnitSphere.z * walkRadius);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(newRandomPos, out hit, walkRadius, 1))
            {
                Vector3 finalPosition = hit.position;
                wanderTarget.transform.position = finalPosition;
            }
    }

    void ResetIdleTimer()
    {
        idle_timer = IDLE_TIME;
    }

    void SetWanderTimer()
    {
        waitTime = Random.Range(minTime, maxTime);
    }

    void ResetJumpTimer()
    {
        jumpFailTimer = maxFailTime;
    }

    void CreateNavAgent()
    {
        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.enabled = false;
        agent.baseOffset = 0.3f;
        agent.height = 0.5f;
        agent.angularSpeed = 500f;
    }

    void EnableAgent()
    {
        tryingToEnable = false;
        agent.enabled = true;
    }


}
