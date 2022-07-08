using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoostTrail : MonoBehaviour
{ 
    public float offsetFromPlayer;
    public float checkObstaclesWidth;
    public Transform trail;
    public LayerMask avoidLayers;
    Transform player;

    bool started;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            float xOffset = 0;
            //Collider[] obstaclesInFront = Physics.OverlapBox(trail.position + new Vector3(0, 0, checkObstaclesSize.z), checkObstaclesSize, Quaternion.identity, avoidLayers, QueryTriggerInteraction.Collide);
            RaycastHit hitRight;
            if(Physics.Raycast(trail.position + new Vector3(checkObstaclesWidth, 0, 0), Vector3.forward, out hitRight, Mathf.Infinity, avoidLayers))
            {

            }


            Vector3 trailPos = new Vector3(trail.position.x + xOffset, trail.position.y, player.position.z) + Vector3.forward * offsetFromPlayer;
            trail.position = trailPos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            started = true;
        }
    }
}
