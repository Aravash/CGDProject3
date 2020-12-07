using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ObjectTypes
{
    REGULAR,
    TRASH,
    ENEMY,
}

public class ObjectCreator : MonoBehaviour
{
    private GameObject[] models;
    private BoxCollider col;

    float wave_timer = 0;
    [SerializeField] float AVRG_WAVE_TMR = 7;
    float spawn_timer = 0;
    const float SPAWN_TMR_MAX = 0.7f;
    const float SPAWN_TMR_MIN = 0.3f;
    int wave_size = 0;
    [SerializeField] int WAVE_SIZE_MAX = 5;
    [SerializeField] int ENEMY_CHANCE_RECIPROCAL = 6;

    void Start()
    {
        models = Resources.LoadAll<GameObject>("objects");
        col = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        //Test code
        if (Input.GetKeyDown(KeyCode.K))
        {
            BuildObject(false);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            BuildObject(true);
        }
        // Automatic waves
        wave_timer -= Time.deltaTime;
        if(wave_timer <= 0)
        {
            waveTick();
        }
    }

    public void BuildObject(bool wrapped = false)
    {
        Vector3 pos = new Vector3(Random.Range(col.bounds.min.x, col.bounds.max.x),
                                  transform.position.y,
                                  Random.Range(col.bounds.min.z, col.bounds.max.z));

        GameObject newObj = gameObject;
        newObj = Instantiate(models[Random.Range(0, models.Length)], pos, transform.rotation);

        // Only convert BAD props to enemies
        if(newObj.tag == "BAD")
        {
            if(Random.Range(0, ENEMY_CHANCE_RECIPROCAL) == 0)
            {
                // Attach enemy script
                newObj.AddComponent<Enemy>();
                Debug.Log("Enemy spawned!");
            }
        }

        // @Conrad, this is all yours -- thanks
        if (wrapped)
        {
            newObj.AddComponent<WrappingHandler>();
        }

        // Set Colour
        Chute.col_ids colour = (Chute.col_ids)Random.Range(0, 6); // int rand is maximally exclusive
        newObj.GetComponent<Renderer>().material.color = Chute.getColour(colour);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

    private void waveTick()
    {
        // init the wave
        if(wave_size == 0)
        {
            wave_size = Random.Range(1, WAVE_SIZE_MAX);
        }

        // Spawn tick
        spawn_timer -= Time.deltaTime;
        if (spawn_timer <= 0)
        {
            BuildObject();

            --wave_size;
            if(wave_size == 0)
            {
                // Set the timer for a new wave
                wave_timer = gaussDistribution(AVRG_WAVE_TMR);
            }
            else
            {
                // continue the wave
                spawn_timer = Random.Range(SPAWN_TMR_MIN, SPAWN_TMR_MAX);
            }
        }
    }
    
    // Gaussian (Normal) distribution
    // Max varience = 3 * sigma
    float gaussDistribution(float mid = 0, float sigma = 1)
    {
        float u, v, S;
        do
        {
            u = 2.0f * Random.value - 1.0f;
            v = 2.0f * Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0);
        float fac = Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
        float rand = u * fac;

        rand *= sigma;
        // flatten values beyond 3sigma
        if (Mathf.Abs(rand) > (3 * sigma))
        {
            rand = Random.Range(-sigma, sigma);
        }
        rand += mid;

        return rand;
    }
}
