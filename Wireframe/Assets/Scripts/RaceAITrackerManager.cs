using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceAITrackerManager : MonoBehaviour
{
    [Header("Racer Spawner Variables")]
    public int numOfRacers;
    public float minRacerHeadstart;
    public float maxRacerHeadstart;
    public float minRacerHeadstartRandomAmount;
    public float maxRacerHeadstartRandomAmount;
    public float minRacerSpeed;
    public float maxRacerSpeed;
    public float zSpawnOffset;
    public float xMaxSpawnOffset;
    [Header("Other Variables")]
    public float finishLineDist;
    public float finishLineIconTop;
    public float finishLineIconBottom;
    public Transform finishLineIcon;

    public Transform player;
    public GameObject racerPrefab;
    public GameObject finishLinePrefab;
    public TextMeshProUGUI positionText;
    public GameCompleteManager gameManagerScript;

    float timer;
    float randomHeadstartAmount;
    int racersSpawned;
    public static int playerPosition;
    bool spawnedFinish;

    // Start is called before the first frame update
    void Start()
    {
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
        //Player cross finish line
        if(player.position.z > finishLineDist + 800)
        {
            gameManagerScript.RaceOver();
            player.gameObject.SetActive(false);
            Destroy(gameObject);
        }
        StatsTracker.averageSpeed = (int)(player.position.z / timer);

        finishLineIcon.localPosition = new Vector3(finishLineIcon.localPosition.x, Mathf.Lerp(finishLineIconTop, finishLineIconBottom, player.position.z / (finishLineDist + 700)), finishLineIcon.localPosition.z);

        positionText.text = "" + playerPosition;
        timer += Time.deltaTime;
    }
}
