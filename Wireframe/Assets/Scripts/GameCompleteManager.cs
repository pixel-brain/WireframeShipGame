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
        Screen.lockCursor = true;
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
        Screen.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
        stateText.text = "WRECKED";
        statsTrackerScript.UpdateStatsText();
        gameCompleteScreen.SetActive(true);
    }

    public void RaceOver()
    {
        Screen.lockCursor = false;
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
        Screen.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Screen.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        pauseScreen.SetActive(false);
        Time.timeScale = SettingsManager.gameSpeed;
    }

}
