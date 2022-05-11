using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirRingSpawner : MonoBehaviour
{
    Rigidbody playerRigi;
    public GameObject ringPrefab;
    public float zOffset;
    public float minXOffset;
    public float maxXOffset;
    public float minYSpawnHeight;
    public GameObject currentRing;
    bool hitFirst;

    // Start is called before the first frame update
    void Start()
    {
        playerRigi = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(currentRing == null)
        {
            float randX = Random.Range(-(maxXOffset - minXOffset), (maxXOffset - minXOffset));
            float yTrajectoryOffset = (playerRigi.velocity.y * zOffset) + (0.5f * Physics.gravity.y * zOffset * zOffset);
            Vector3 spawnPos = playerRigi.position + new Vector3(minXOffset * Mathf.Sign(randX) + randX, yTrajectoryOffset, playerRigi.velocity.z * zOffset);
            if (spawnPos.y < minYSpawnHeight && hitFirst == true)
            {
                Destroy(this.gameObject);
                return;
            }
            currentRing = Instantiate(ringPrefab, spawnPos, Quaternion.identity);
            hitFirst = true;
        }
        if(hitFirst == false && playerRigi.position.z > transform.position.z + 100f)
        {
            Destroy(this.gameObject);
        }
    }
}
