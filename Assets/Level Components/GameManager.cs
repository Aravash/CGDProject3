using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float score;
    private float scoreToAdd;
    private float scoreToLose;
    private float scoreChangeSpeed = 2.0f;
    
    private static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        float changeSpeed = scoreChangeSpeed * Time.deltaTime;
        
        if (scoreToAdd > 0)
        {
            score += scoreToAdd * changeSpeed;
            scoreToAdd -= changeSpeed;
        }
        else scoreToAdd = 0;
        
        if (scoreToLose < 0)
        {
            score -= scoreToLose * changeSpeed;
            scoreToLose -= changeSpeed;
        }
        else scoreToLose = 0;
    }

    public static void AddScore(float change)
    {
        instance.scoreToAdd += change;
    }
    
    public static void SubtractScore(float change)
    {
        instance.scoreToLose += change;
    }
}
