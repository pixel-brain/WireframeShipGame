using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform playerPos;
    public float xSpawnOffset;
    public float zSpawnOffset;
    public float distBtwnSpawns;
    public GameObject mound1;
    public GameObject ramp;
    public GameObject boost;

    float lastSpawnPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerPos.position.z > lastSpawnPos + distBtwnSpawns)
        {
            lastSpawnPos = playerPos.position.z;
            Spawn();
        }
    }

    void Spawn()
    {
        Vector3 spawnPos = new Vector3(playerPos.position.x + Random.Range(-xSpawnOffset, xSpawnOffset), 0, playerPos.position.z + zSpawnOffset);
        float random = Random.Range(0, 100);
        if(random > 5)
        {
            Instantiate(mound1, spawnPos, Quaternion.Euler(new Vector3(0, 0, 0)));
        }
        else if(random > 2)
        {
            Instantiate(boost, spawnPos, Quaternion.Euler(new Vector3(45, 45, 0)));
        }
        else
        {
            Instantiate(ramp, spawnPos, Quaternion.Euler(new Vector3(-90, 180, 0)));
        }
    }
}
