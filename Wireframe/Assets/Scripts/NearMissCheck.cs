using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearMissCheck : MonoBehaviour
{
    public PlayerMove playerMoveScript;
    public Transform target;
    public ParticleSystem nearMissParticles;
    public Collider playerCol;
    void Update()
    {
        transform.position = target.position;
    }

    void OnTriggerExit(Collider other)
    {
        if(PlayerMove.invinsibleTimer < 0)
        {
            nearMissParticles.transform.position = playerCol.ClosestPoint(other.transform.position);
            nearMissParticles.Play();
            playerMoveScript.NearMiss();
        }
    }
}
