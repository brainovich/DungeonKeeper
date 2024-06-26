using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; private set; }

    private void Start()
    {
        Score = 0;
        GameManager.instance.uiController.UpdateScore(Score);
    }

    public void IncrementScore()
    {
        Score++;
        GameManager.instance.uiController.UpdateScore(Score);
    }

    public void DecrementScore()
    {
        Score--;
        GameManager.instance.uiController.UpdateScore(Score);
    }
}
