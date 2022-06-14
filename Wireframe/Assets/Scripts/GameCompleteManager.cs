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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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

}
