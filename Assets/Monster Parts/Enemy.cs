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

    public bool fixRot;
    Transform movingTarget;

    Transform target;

    GameObject[] endPoints;
    float waitTime;
    float minTime = 5.0f;
    float maxTime = 10.0f;
    bool endChosen;
    Transform ep;

    public bool yeeting;

    bool grabbed;

    float idle_timer = IDLE_TIME;
    const float IDLE_TIME = 2;

    bool doonce;

    // Start is called before the first frame update
    void Start()
    {

        legs = Instantiate(Resources.Load("Enemy/Legs n Eyes V2") as GameObject);
        legs.transform.localEulerAngles = new Vector3(0, 180, 0);
        legs.transform.parent = gameObject.transform;

        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.baseOffset = 0.3f;
        agent.height = 0.5f;
        agent.angularSpeed = 500f;
        agent.enabled = false;

        //legs = transform.Find("Legs n Eyes V2").gameObject;
        rb = GetComponent<Rigidbody>();

        //agent = GetComponent<NavMeshAgent>();
        movingTarget = new GameObject("target").transform;
        target = movingTarget;
        ChooseWanderPoint();
        fixRot = false;
        grabbed = false;
        endPoints = GameObject.FindGameObjectsWithTag("EnemyWaypoints");
        waitTime = Random.Range(minTime, maxTime);
        endChosen = false;
        yeeting = false;
        doonce = false;
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
        Vector3 enemyPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 tar = new Vector3(target.position.x, 0, target.position.z);

        

        if(Vector3.Distance(tar, enemyPos) < 0.5f)
        {
            if(endChosen)
            {
                YeetSelf();
            }
            else
            {
                ChooseWanderPoint();
            }
        }

        if (active && !grabbed && !yeeting)
        {
            agent.destination = target.position;

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

    }


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

        StartCoroutine("Activation");

        
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
    }

    IEnumerator Activation()
    {
        rb.AddForce(0, 150, 0);
        fixRot = true;
        legs.GetComponent<leg>().ActivateLeg();
        yield return new WaitForSeconds(0.25f);
        fixRot = false;
        rb.rotation = Quaternion.identity;
        
        yield return new WaitForSeconds(0.3f);
        rb.isKinematic = true;

        agent.enabled = true;
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
        fixRot = true;
        //legs.GetComponent<leg>().ActivateLeg();
        yield return new WaitForSeconds(0.25f);
        fixRot = false;
        rb.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.3f);
        rb.isKinematic = true;

        agent.enabled = true;
        grabbed = false;
    }

    void ChooseEndpoint()
    {
        int index = Random.Range(0, endPoints.Length);
        ep = endPoints[index].transform;
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
            target.position = finalPosition;
            movingTarget.position = finalPosition;
        }
    }

    void YeetSelf()
    {
        yeeting = true;
        agent.enabled = false;
        transform.rotation = ep.rotation;
        
        rb.isKinematic = false;
        rb.velocity = transform.forward * 2  + new Vector3(0, 5, 0);
        Debug.Log("Time to die " + gameObject);
    }

}
