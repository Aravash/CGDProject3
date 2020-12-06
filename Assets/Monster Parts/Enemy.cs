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
    public Transform move;

    bool grabbed;

    // Start is called before the first frame update
    void Start()
    {
        legs = transform.Find("Legs n Eyes V2").gameObject;
        rb = GetComponent<Rigidbody>();
        //controller = GetComponent<CharacterController>();
        //box = GetComponent<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();

        fixRot = false;
        grabbed = false;

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

        if(Vector3.Distance(move.position, transform.position) < 0.25f)
        {
            Debug.Log("SDDSSF");
        }


    }

    private void FixedUpdate()
    {
        if(active && !grabbed)
        {

            agent.destination = move.position;




            /*
            if(controller != null)
            {
                if(controller.isGrounded == false)
                {
                    moveVector.y += -9.81f * Time.deltaTime * 2;
                }

                Vector3 asdf = new Vector3(randomPos.x, transform.position.y, randomPos.z);
                Vector3 dir = asdf - transform.position;

                Vector3 gogo = dir.normalized * 2;
                moveVector.x = gogo.x;
                moveVector.z = gogo.z;

                controller.Move(moveVector * Time.deltaTime);
                //transform.rotation = Quaternion.LookRotation(gogo);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(gogo), 1000 * Time.deltaTime);
            }
            */
            /*
            Vector3 dir = randomPos - transform.position;
            Vector3 asdf = new Vector3(randomPos.x, transform.position.y, randomPos.z);
            transform.LookAt(asdf);
            rb.AddRelativeForce(Vector3.forward * Time.deltaTime* 500, ForceMode.Force);
            */


        }

        if(fixRot)
        {
            rb.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, Time.deltaTime * 500);
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

    void DeactivateEnemy()
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
}
