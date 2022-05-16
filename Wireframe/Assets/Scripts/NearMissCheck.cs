using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearMissCheck : MonoBehaviour
{
    public PlayerMove playerMoveScript;
    public Transform target;
    void Update()
    {
        transform.position = target.position;
    }

    void OnTriggerExit(Collider other)
    {
        if(PlayerMove.invinsibleTimer < 0)
        {
            playerMoveScript.NearMiss();
        }
    }
}
