using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirRingSpawner : MonoBehaviour
{
    Rigidbody playerRigi;
    public GameObject[] ringPrefabs;
    public float zOffset;
    public float minXOffset;
    public float maxXOffset;
    GameObject currentRing;
    [HideInInspector]
    public int state;
    int ringIndex;

    // Start is called before the first frame update
    void Start()
    {
        playerRigi = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(state > 0 && state <= 3 && currentRing == null)
        {
            currentRing = Instantiate(ringPrefabs[ringIndex], SpawnPos(), Quaternion.identity);
            currentRing.GetComponent<AirRing>().spawnerScript = GetComponent<AirRingSpawner>();
            state++;
            ringIndex++;
        }
    }

    public Vector3 SpawnPos()
    {
        float randX = Random.Range(minXOffset, maxXOffset) * ((Random.Range(0, 2) - 0.5f) * 2);
        float yTrajectoryOffset = (playerRigi.velocity.y * zOffset) + (0.5f * Physics.gravity.y * zOffset * zOffset);
        Vector3 spawnPos = playerRigi.position + new Vector3(minXOffset * Mathf.Sign(randX) + randX, yTrajectoryOffset, playerRigi.velocity.z * zOffset);
        return spawnPos;
    }

    void OnDestroy()
    {
        Destroy(currentRing);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            state = 1;
        }
    }
}
