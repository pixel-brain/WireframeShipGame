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
    //[Header("Other settings")]

    public static int difficulty;
    public static bool created;

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
}
