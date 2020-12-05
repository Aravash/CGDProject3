using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float maxScore = 10;
    private float currentScore = 5;
    private float currentVisualScore = 1;
    private float scoreChangeSpeed = 2.0f;

    [SerializeField] private Image mask;

    private static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Math.Abs(currentVisualScore - currentScore) > 0.01f)
        {
            float changeSpeed = scoreChangeSpeed * Time.deltaTime;
            currentVisualScore = Mathf.Lerp(currentVisualScore, currentScore, changeSpeed);
            UpdateFillAmount();
        }

        if (currentVisualScore >= maxScore)
        {
            CanvasManager.ShowWin();
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
