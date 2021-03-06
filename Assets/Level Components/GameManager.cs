﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]private float maxScore = 10;
    [SerializeField]private float currentScore = 5;
    [SerializeField]private float currentVisualScore = 1;
    private float scoreChangeSpeed = 2.0f;
    
    [SerializeField] private Color gainColour;
    [SerializeField] private Color neutralColour;
    [SerializeField] private Color loseColour;
    [SerializeField] private Image bar;

    [SerializeField] private Image mask;
    [SerializeField] private Door door;
    [SerializeField] private Sprite jreCurse;

    public static GameManager _i;

    void Awake()
    {
        _i = this;
    }

    private void Update()
    {
        if (Math.Abs(currentVisualScore - currentScore) > 0.2f)
        {
            float changeSpeed = scoreChangeSpeed * Time.deltaTime;
            currentVisualScore = Mathf.Lerp(currentVisualScore, currentScore, changeSpeed);
            UpdateFillAmount();
            bar.color = currentVisualScore > currentScore ? loseColour : gainColour; 
        }
        else bar.color = neutralColour;

        if (Input.GetKey("j") && Input.GetKey("r") && Input.GetKey("e"))
        {
            SpriteRenderer[] jre = GameObject.Find("ChuteSprites").transform.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer child in jre)
            {
                child.sprite = jreCurse;
                child.transform.localScale = Vector3.one;
            }
        }

        if (currentVisualScore >= maxScore)
        {
            CanvasManager.ShowWin();
            door.open();
            Destroy(this); // dont need gameManager when game is over
        }
        else if (currentVisualScore <= 0)
        {
            CanvasManager.ShowLose();
            Destroy(this);
        }
    }

    private void UpdateFillAmount()
    {
        mask.fillAmount = currentVisualScore / maxScore;
    }

    public static void ChangeScore(float change)
    {
        _i.currentScore += change;

        if(change >= 0)
        {
            GameAudioManager.GainScore();
        }
        else
        {
            GameAudioManager.LoseScore();
        }
    }

    #region stat tracking
    // Stat tracking
    int objects_in_play = 3; // start at 3 cuz Table, Chair and Dresser
    int mistakes_trash = 0;
    int mistakes_colour = 0;
    int mistakes_incinerated_good = 0;
    int enemies_missed = 0;
    int enemies_burnt = 0;

    public void counterInc()
    {
        ++objects_in_play;
    }
    public void counterDec()
    {
        --objects_in_play;
    }
    public void mistakeTrash()
    {
        ++mistakes_trash;
    }
    public void mistakeColour()
    {
        ++mistakes_colour;
    }
    public void mistakeGood()
    {
        ++mistakes_incinerated_good;
    }
    public void enemyMissed()
    {
        ++enemies_missed;
    }
    public void enemyBurnt()
    {
        ++enemies_burnt;
    }
    #endregion
}
