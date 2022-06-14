using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class NearMissCheck : MonoBehaviour
{
    [SerializeField]
    EventReference nearmissSFX;
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
            nearMissParticles.transform.localPosition = new Vector3(Mathf.Abs(nearMissParticles.transform.localPosition.x) * -Mathf.Sign(nearMissParticles.transform.position.x - other.transform.position.x), nearMissParticles.transform.localPosition.y, nearMissParticles.transform.localPosition.z);
            RuntimeManager.PlayOneShot(nearmissSFX, gameObject.transform.position);
            nearMissParticles.Play();
            playerMoveScript.NearMiss();
        }
    }
}
