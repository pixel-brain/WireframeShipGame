using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [Header("Difficulty Settings")]
    public int[] lives;
    public int[] racers;
    public float[] maxAIHeadstart;
    public float[] minAISpeed;
    public float[] maxAISpeed;
    public float[] raceLength;
    public int[] boostSpawnDist;
    [Header("Color Schemes")]
    public Color[] baseColor;
    public Color[] glowColor;
    public Color[] sunColor;
    public Color[] skyColor1;
    public Color[] skyColor2;

    public Material baseMat;
    public Material glowMat;
    public Material sunMat;
    public Material floorMat;
    public Material skyMat;

    public static int difficulty;
    public static bool created;

    int colorScheme;

    // Start is called before the first frame update
    void Start()
    {
        if (created)
        {
            Destroy(gameObject);
        }
        else
        {
            created = true;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        //REMOVE THIS
        if (Input.GetButtonDown("Jump"))
        {
            ChangeColor();
        }
    }

    public void ChangeColor()
    {
        colorScheme++;
        if (colorScheme > baseColor.Length - 1)
        {
            colorScheme = 0;
        }
        baseMat.SetColor("_BaseColor", baseColor[colorScheme]);
        glowMat.SetColor("_EmissionColor", glowColor[colorScheme] * 4f);
        sunMat.SetColor("_EmissionColor", sunColor[colorScheme] * 2.1f);
        floorMat.SetColor("GridColor", glowColor[colorScheme] * 7f);
        skyMat.SetColor("Color1", skyColor1[colorScheme]);
        skyMat.SetColor("Color2", skyColor2[colorScheme]);
    }
}
