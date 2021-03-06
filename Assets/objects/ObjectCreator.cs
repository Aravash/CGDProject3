﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

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

    float wave_timer = 5;
    [SerializeField] float AVRG_WAVE_TMR = 7;
    float spawn_timer = 0;
    const float SPAWN_TMR_MAX = 0.7f;
    const float SPAWN_TMR_MIN = 0.3f;
    int wave_size = 0;
    [SerializeField] int WAVE_SIZE_MAX = 5;
    [SerializeField] int ENEMY_CHANCE_RECIPROCAL = 6;
    [SerializeField] int WRAPPED_CHANCE_RECIPROCAL;

    // Audio Event to use
    [SerializeField] private FMODUnity.StudioEventEmitter eventEmitter;

    void Start()
    {
        models = Resources.LoadAll<GameObject>("objects");
        col = GetComponent<BoxCollider>();

        WRAPPED_CHANCE_RECIPROCAL = 4;
    }

    private void Update()
    {
        /*//Test code
        if (Input.GetKeyDown(KeyCode.K))
        {
            BuildObject();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            int currentChance = WRAPPED_CHANCE_RECIPROCAL;
            WRAPPED_CHANCE_RECIPROCAL = 0;
            BuildObject();
            WRAPPED_CHANCE_RECIPROCAL = currentChance;
        }*/

        // Automatic waves
        wave_timer -= Time.deltaTime;
        if(wave_timer <= 0)
        {
            waveTick();
        }
    }

    public void BuildObject()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "GameScene" || currentScene.name == "GameScene_ai")
        {
            GameManager._i.counterInc();
        }

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
                newObj.AddComponent<Enemy2>();
                Debug.Log("Enemy spawned!");
            }
        }

        // Set Colour
        Chute.col_ids colour = (Chute.col_ids)Random.Range(0, 6); // int rand is maximally exclusive
        newObj.GetComponent<Renderer>().material.color = Chute.getColour(colour);

        // Add wrapper if requested
        if (newObj.tag == "GOOD" && WRAPPED_CHANCE_RECIPROCAL >= 0)
        {
            if (Random.Range(0, WRAPPED_CHANCE_RECIPROCAL) == 0)
            {
                newObj.AddComponent<WrappingHandler>();
                //newObj.GetComponent<WrappingHandler>().SetColour(colour);
            }
        }

        // Attach Audio Effects
        newObj.AddComponent<FMODUnity.StudioEventEmitter>();
        var tmp = newObj.GetComponent<FMODUnity.StudioEventEmitter>();
        tmp.Event = eventEmitter.Event;
        tmp.CollisionTag = eventEmitter.CollisionTag;
        tmp.PlayEvent = eventEmitter.PlayEvent;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one);
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
