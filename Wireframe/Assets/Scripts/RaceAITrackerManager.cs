using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceAITrackerManager : MonoBehaviour
{
    [Header("Racer Spawner Variables")]
    int numOfRacers;
    public float minRacerHeadstart;
    float maxRacerHeadstart;
    public float minRacerHeadstartRandomAmount;
    public float maxRacerHeadstartRandomAmount;
    float minRacerSpeed;
    float maxRacerSpeed;
    public float zSpawnOffset;
    public float xMaxSpawnOffset;
    [Header("Other Variables")]
    float finishLineDist;
    public float finishLineIconTop;
    public float finishLineIconBottom;
    public Transform finishLineIcon;

    public Transform player;
    public GameObject racerPrefab;
    public GameObject finishLinePrefab;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI winTimerText;
    public GameCompleteManager gameManagerScript;
    public Animator positionTextAnim;

    float timer;
    float randomHeadstartAmount;
    int racersSpawned;
    public static int playerPosition;
    bool spawnedFinish;
    bool halfway;
    float winTimer;

    // Start is called before the first frame update
    void Start()
    {
        SettingsManager settings = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
        int difficulty = SettingsManager.difficulty;
        maxRacerHeadstart = settings.maxAIHeadstart[difficulty];
        minRacerSpeed = settings.minAISpeed[difficulty];
        maxRacerSpeed = settings.maxAISpeed[difficulty];
        finishLineDist = settings.raceLength[difficulty];
        numOfRacers = settings.racers[difficulty];



        playerPosition = numOfRacers + 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Spawn racer
        if (racersSpawned < numOfRacers)
        {
            float lerpAmount = (float)racersSpawned / (float)numOfRacers;
            float nextRacerPos = Mathf.Lerp(minRacerHeadstart, maxRacerHeadstart, lerpAmount) + (timer * Mathf.Lerp(minRacerSpeed, maxRacerSpeed, lerpAmount)) + randomHeadstartAmount;
            if (player.transform.position.z > nextRacerPos)
            {
                Vector3 spawnPos = new Vector3(Random.Range(-xMaxSpawnOffset, xMaxSpawnOffset) + player.position.x, 0.4f, zSpawnOffset + player.position.z);
                GameObject newRacer = Instantiate(racerPrefab, spawnPos, Quaternion.identity);
                newRacer.GetComponent<AIRacer>().forwardSpeed = Mathf.Lerp(minRacerSpeed, maxRacerSpeed, lerpAmount);
                float randomMax = Mathf.Lerp(minRacerHeadstartRandomAmount, maxRacerHeadstartRandomAmount, lerpAmount);
                randomHeadstartAmount = Random.Range(-randomMax/2f, randomMax);
                racersSpawned++;
            }
        }
        //Spawn Finish Line
        if(player.position.z > finishLineDist && spawnedFinish == false)
        {
            spawnedFinish = true;
            Instantiate(finishLinePrefab, new Vector3(player.position.x, 0, finishLineDist + 800), Quaternion.identity);
        }
        //Player halfway
        if(player.position.z > finishLineDist / 2f + 100 && halfway == false)
        {
            finishLineIcon.gameObject.GetComponent<Animator>().SetTrigger("Halfway");
            halfway = true;
        }

        //Player remains in first for 10 seconds
        if(playerPosition == 1)
        {
            winTimer -= Time.deltaTime;
            winTimerText.text = "Win in " + (int)winTimer;
            if(winTimer < 0.5f)
            {
                winTimerText.text = "Win in 0";
                gameManagerScript.RaceOver();
                player.gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
        else if(winTimer < 11)
        {
            winTimer = 11f;
            winTimerText.text = "";
        }

        //Player cross finish line
        if(player.position.z > finishLineDist + 800)
        {
            gameManagerScript.RaceOver();
            player.gameObject.SetActive(false);
            Destroy(gameObject);
        }

        //Stats average speed
        StatsTracker.averageSpeed = (int)(player.position.z / timer);

        finishLineIcon.localPosition = new Vector3(finishLineIcon.localPosition.x, Mathf.Lerp(finishLineIconTop, finishLineIconBottom, player.position.z / (finishLineDist + 700)), finishLineIcon.localPosition.z);

        int previousTextPos = int.Parse(positionText.text);
        if (playerPosition != previousTextPos)
        {
            positionText.text = "" + playerPosition;
            if(previousTextPos > playerPosition)
            {
                positionTextAnim.SetTrigger("Leap");
            }
        }

        timer += Time.deltaTime;
    }
}
