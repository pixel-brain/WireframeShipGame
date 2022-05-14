using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearMissCheck : MonoBehaviour
{
    public PlayerMove playerMoveScript;
    void OnTriggerExit(Collider other)
    {
        playerMoveScript.NearMiss();
    }
}
