using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsTracker : MonoBehaviour
{

    public TextMeshProUGUI positionText;
    public TextMeshProUGUI topSpeedText;
    public TextMeshProUGUI averageSpeedText;
    public TextMeshProUGUI closeCallsText;
    public TextMeshProUGUI takedownsText;
    public TextMeshProUGUI difficultyText;

    public static int topSpeed;
    public static int averageSpeed;
    public static int closeCalls;
    public static int takedowns;

    // Start is called before the first frame update
    void Start()
    {
        topSpeed = 0;
        averageSpeed = 0;
        closeCalls = 0;
        takedowns = 0;
    }

    public void UpdateStatsText()
    {
        positionText.text = "" + RaceAITrackerManager.playerPosition;
        topSpeedText.text = "" + topSpeed;
        averageSpeedText.text = "" + averageSpeed;
        closeCallsText.text = "" + closeCalls;
        takedownsText.text = "" + takedowns;
        if (SettingsManager.difficulty == 0)
            difficultyText.text = "Novice";
        else if (SettingsManager.difficulty == 1)
            difficultyText.text = "Standard";
        else if (SettingsManager.difficulty == 2)
            difficultyText.text = "Expert";
    }
}
