using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]private float maxScore = 10;
    [SerializeField]private float currentScore = 5;
    private float currentVisualScore = 1;
    private float scoreChangeSpeed = 2.0f;
    
    [SerializeField] private Color gainColour;
    [SerializeField] private Color neutralColour;
    [SerializeField] private Color loseColour;
    [SerializeField] private Image bar;

    [SerializeField] private Image mask;
    [SerializeField] private Door door;
    [SerializeField] private Sprite jreCurse;

    private static GameManager instance;

    void Awake()
    {
        instance = this;
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
        instance.currentScore += change;
    }
}
