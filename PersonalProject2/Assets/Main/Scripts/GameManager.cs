using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ScoreManager scoreManager;
    public PlayerControlls playerControlls;
    public ItemsBehaviour itemsBehaviour;
    public UIController uiController;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
