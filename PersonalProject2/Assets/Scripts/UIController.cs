using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI scoreText;

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _tutorialMenu;

    [SerializeField] private GameObject _screenEndLevel;

    [SerializeField] private TextMeshProUGUI _coinsCollected;
    [SerializeField] private TextMeshProUGUI _coinsAtLevel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.instance.playerControlls.isDead && !_screenEndLevel.activeInHierarchy)
        {
            if(SceneManager.GetActiveScene().buildIndex != 0 && !_pauseMenu.activeInHierarchy)
            {
                PauseMenu();
            }
            else if (_tutorialMenu != null && _tutorialMenu.activeInHierarchy)
            {
                BackToMenu();
            }
            else if(SceneManager.GetActiveScene().buildIndex != 0 && _pauseMenu.activeInHierarchy)
            {
                Continue();
            }
        }
    }

    public void SetMaxHealth(int health)
    {
        healthBar.maxValue = health;
        healthBar.value = health;
    }
    public void SetHealth(int health)
    {
        healthBar.value = health;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void Tutorial()
    {
        _tutorialMenu.SetActive(true);
        _pauseMenu.SetActive(false);
    }

    public void PauseMenu()
    {
        _pauseMenu.SetActive(true);
        TurnOnCursor();
        Time.timeScale = 0;
    }

    public void Continue()
    {
        _pauseMenu.SetActive(false);
        TurnOffCursor();
        Time.timeScale = 1;
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void BackToMenu()
    {
        _tutorialMenu.SetActive(false);
        _pauseMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void EndLevel()
    {
        _screenEndLevel.SetActive(true);
        TurnOnCursor();
        Score();
    }

    private void Score()
    {
        _coinsCollected.text = GameManager.instance.scoreManager.Score.ToString();
        _coinsAtLevel.text = GameManager.instance.itemsBehaviour.coinsAtLevel.ToString();
    }

    public void TurnOffCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.instance.playerControlls.canShot = true;
        Debug.Log("cursor turned off");
    }

    public void TurnOnCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManager.instance.playerControlls.canShot = false;
        Debug.Log("cursor turned on");
    }
}
