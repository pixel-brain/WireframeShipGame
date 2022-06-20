using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameCompleteManager : MonoBehaviour
{
    public StatsTracker statsTrackerScript;
    public GameObject gameCompleteScreen;
    public TextMeshProUGUI stateText;
    public GameObject pauseScreen;

    void Start()
    {
        Time.timeScale = SettingsManager.gameSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }
    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        stateText.text = "WRECKED";
        statsTrackerScript.UpdateStatsText();
        gameCompleteScreen.SetActive(true);
    }

    public void RaceOver()
    {
        Cursor.lockState = CursorLockMode.None;
        if (RaceAITrackerManager.playerPosition <= 1)
        {
            stateText.text = "VICTORY";
        }
        else
        {
            stateText.text = "DEFEAT";
        }
        statsTrackerScript.UpdateStatsText();
        gameCompleteScreen.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseScreen.SetActive(false);
        Time.timeScale = SettingsManager.gameSpeed;
    }

}
