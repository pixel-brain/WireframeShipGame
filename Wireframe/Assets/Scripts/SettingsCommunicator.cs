using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsCommunicator : MonoBehaviour
{
    public Slider gameSpeedSlider;

    void Start()
    {
        gameSpeedSlider.value = (SettingsManager.gameSpeed - 1.1f) * 10f;
    }

    public void ChangeColor()
    {
        GameObject.Find("SettingsManager").GetComponent<SettingsManager>().ChangeColor();
    }

    public void ChangeGameSpeed()
    {
        SettingsManager.gameSpeed = 1.1f + ((float)gameSpeedSlider.value / 10f);
    }
}
