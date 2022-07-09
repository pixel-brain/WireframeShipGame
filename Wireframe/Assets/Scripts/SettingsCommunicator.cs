using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsCommunicator : MonoBehaviour
{
    public Slider gameSpeedSlider;
    public GameObject qualityMenu;
    public TextMeshProUGUI toggleQualityText;
    public QualityUpdater qualityUpdaterScript;

    void Awake()
    {
        gameSpeedSlider.value = (SettingsManager.gameSpeed - 1.1f) * 10f;
        if(SettingsManager.setPerformance == false)
        {
            if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Time.timeScale = 0f;
                qualityMenu.SetActive(true);
            }
            else
            {
                SettingsManager.setPerformance = true;
                SettingsManager.qualityMode = true;
            }
        }
        else
        {
            if(SettingsManager.qualityMode == true)
            {
                toggleQualityText.text = "Best Visuals";
            }
            else
            {
                toggleQualityText.text = "Best Performance";
            }
        }
    }

    public void ChangeColor()
    {
        GameObject.Find("SettingsManager").GetComponent<SettingsManager>().ChangeColor();
    }

    public void ChangeGameSpeed()
    {
        SettingsManager.gameSpeed = 1.1f + ((float)gameSpeedSlider.value / 10f);
    }

    public void SetQuality(bool quality)
    {
        SettingsManager.qualityMode = quality;
        qualityMenu.SetActive(false);
        Time.timeScale = 1f;
        Screen.lockCursor = true;
        SettingsManager.setPerformance = true;
        if(quality == true)
        {
            toggleQualityText.text = "Best Visuals";
            qualityUpdaterScript.SetHighGraphics();
        }
        else
        {
            toggleQualityText.text = "Best Performance";
            qualityUpdaterScript.SetLowGraphics();
        }
    }

    public void ChangeQuality()
    {
        bool quality = !SettingsManager.qualityMode;
        SettingsManager.qualityMode = quality;
        if (quality == true)
        {
            toggleQualityText.text = "Best Visuals";
            qualityUpdaterScript.SetHighGraphics();
        }
        else
        {
            toggleQualityText.text = "Best Performance";
            qualityUpdaterScript.SetLowGraphics();
        }
    }
}
