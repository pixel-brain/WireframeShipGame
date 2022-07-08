using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AIRacer : MonoBehaviour
{
    [SerializeField]
    EventReference enemydeathSFX;
    [SerializeField]
    EventReference enemyboostSFX;
    [SerializeField]
    EventReference enemyrampSFX;
    [Header("AI Variables")]
    public float activateCollisionDist;
    public float forwardRaycastOffset;
    public float raycastLength;
    public int raycastCount;
    public float maxRaycastAngle;
    public float turnEvalTime;
    float turnEvalTimer;
    public LayerMask obstacleLayers;
    Transform player;

    [Header("Racer Icon Variables")]
    public float minYPos;
    public float maxYPos;
    public GameObject racerIconPrefab;
    bool aheadOfPlayer;
    GameObject racerIcon;


    [Header("Move Variables")]
    public AnimationCurve accelScaling;
    public AnimationCurve decelScaling;
    public AnimationCurve decelDelaying;
    [Range(0f, 200f)]
    public float forwardSpeed;
    [Range(0f, 500f)]
    public float maxForwardSpeed;
    [Range(0f, 3f)]
    public float forwardDecelSpeed;
    [Range(0f, 3f)]
    public float forwardAccelSpeed;
    [Range(0f, 3f)]
    public float forwardInitialAccelSpeed;
    [Range(0f, 3f)]
    public float forwardInitialAccelTime;
    [Range(0f, 8f)]
    public float accelSpeed;
    [Range(0f, 40f)]
    public float maxSpeed;
    [Range(0f, 1f)]
    public float moveDamping;
    [Range(0f, 1f)]
    public float turnDamping;
    [Range(0f, 1f)]
    public float stopDamping;

    [Header("Ramp Variables")]
    [Range(0f, 50f)]
    public float rampLaunchSpeed;
    [Range(0f, 1f)]
    public float angleSmoothing;
    [Range(0f, 45f)]
    public float maxAngle;

    [Header("Boost Variables")]
    [Range(0f, 8f)]
    public float boostTime;

    [Header("Ring Variables")]
    [Range(0f, 8f)]
    public float ringBoostTime;

    [Header("NearMiss Variables")]
    [Range(0f, 5f)]
    public float nearMissBoostTime;

    [Header("References")]
    public Transform model;
    public ParticleSystem boostParticles;
    public GameObject wreckEffectPrefab;


    int moveInput;
    [HideInInspector]
    public float boostTimer;
    [HideInInspector]
    public float boostInitialTimer;
    float forwardDecelDelayTimer;
    [HideInInspector]
    public Rigidbody rigi;
    bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        rigi = GetComponent<Rigidbody>();
        rigi.velocity = new Vector3(0, 0, forwardSpeed);
        aheadOfPlayer = true;

        //Spawn Icon
        racerIcon = Instantiate(racerIconPrefab, Vector3.zero, Quaternion.identity);
        racerIcon.transform.parent = GameObject.Find("RacerPositionBar").transform;
        racerIcon.transform.localScale = Vector3.one;
    }

    void FixedUpdate()
    {

        GetDirection();
        CheckGrounded();
        Move();
        //update racer icon
        float distFromPlayer = transform.position.z - player.position.z;
        racerIcon.transform.localPosition = new Vector3(0, Mathf.Lerp(minYPos, maxYPos, distFromPlayer/320f), 0);
        //Update position if behind player
        if(distFromPlayer < 0 && aheadOfPlayer == true)
        {
            RaceAITrackerManager.playerPosition--;
            aheadOfPlayer = false;
        }
        else if(distFromPlayer >= 0 && aheadOfPlayer == false)
        {
            aheadOfPlayer = true;
            RaceAITrackerManager.playerPosition++;
        }

        //Destroy if too far behind
        if(distFromPlayer < -250f)
        {
            Destroy(racerIcon);
            Destroy(gameObject);
        }
    }

    void GetDirection()
    {
        int clearFrontRays = 0;
        //Check forward direction for collision
        for(int i = 0; i < 3; i++)
        {
            if (!Physics.Raycast(transform.position + new Vector3(-forwardRaycastOffset + i * forwardRaycastOffset, 0, 0), transform.forward, raycastLength * forwardSpeed / 70f, obstacleLayers, QueryTriggerInteraction.Collide))
            {
                clearFrontRays++;
            }
        }
        if(clearFrontRays == 3)
        {
            moveInput = 0;
            return;
        }
        if(turnEvalTimer < 0)
        {
            turnEvalTimer = turnEvalTime;
            int leftRightCount = 0;
            //Otherwise compare raycasts on the sides
            for (int i = 0; i < raycastCount; i++)
            {
                if (Physics.Raycast(transform.position, transform.forward + new Vector3(-Mathf.Deg2Rad * maxRaycastAngle + i * (Mathf.Deg2Rad * maxRaycastAngle * 2 / raycastCount), 0, 0), raycastLength * forwardSpeed/70f, obstacleLayers, QueryTriggerInteraction.Collide))
                {
                    if (i < raycastCount / 2)
                    {
                        leftRightCount++;
                    }
                    else
                    {
                        leftRightCount--;
                    }
                }
            }
            moveInput = (int)Mathf.Sign(leftRightCount);
        }
        turnEvalTimer -= Time.fixedDeltaTime;
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < raycastCount; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, (transform.forward + new Vector3(-Mathf.Deg2Rad * maxRaycastAngle + i * (Mathf.Deg2Rad * maxRaycastAngle * 2 / raycastCount), 0, 0)) * raycastLength);
        }
        for (int i = 0; i < 3; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + new Vector3(-forwardRaycastOffset + i * forwardRaycastOffset, 0, 0), transform.forward * raycastLength);
        }
    }

    void Move()
    {
        float xVel = rigi.velocity.x;
        xVel += moveInput * Time.fixedDeltaTime * 50f * accelSpeed;
        //input in direction of movement
        if (Mathf.Abs(moveInput) > 0.35f && Mathf.Sign(rigi.velocity.x) - Mathf.Sign(moveInput) == 0)
        {
            xVel *= Mathf.Pow((1f - moveDamping), 0.02f);
        }
        //input in opposite direction of movement
        else if (Mathf.Abs(moveInput) > 0.35f)
        {
            xVel *= Mathf.Pow((1f - turnDamping), 0.02f);
        }
        //no input
        else
        {
            xVel *= Mathf.Pow((1f - stopDamping), 0.02f);
        }
        //stop slight drift
        if (Mathf.Abs(xVel) < 0.5f)
        {
            xVel = 0f;
        }
        //limit max speed
        xVel = Mathf.Clamp(xVel, -maxSpeed, maxSpeed);
        //Rotation
        float angle = Vector3.Angle(transform.forward, new Vector3(0, rigi.velocity.y, rigi.velocity.z));
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
        angle = Mathf.Sign(rigi.velocity.y) * Mathf.LerpAngle(model.eulerAngles.x, -angle, angleSmoothing);
        model.localEulerAngles = new Vector3(angle, xVel / 6f, -xVel / 1.5f);
        //Set forward speed
        float zVel = rigi.velocity.z;
        if (boostTimer > 0 && grounded == true)
        {
            if (boostInitialTimer > 0)
            {
                zVel += accelScaling.Evaluate(zVel) * forwardInitialAccelSpeed;
                boostParticles.Play();
            }
            zVel = Mathf.Clamp(zVel + (forwardAccelSpeed * accelScaling.Evaluate(rigi.velocity.z)), forwardSpeed, maxForwardSpeed);
            forwardDecelDelayTimer = 0;
        }
        else if (grounded == true)
        {
            zVel = Mathf.Clamp(zVel - (forwardDecelSpeed * decelScaling.Evaluate(zVel) * decelDelaying.Evaluate(forwardDecelDelayTimer)), forwardSpeed, maxForwardSpeed);
            boostParticles.Stop();
            forwardDecelDelayTimer += Time.fixedDeltaTime;
        }
        else
        {
            boostParticles.Stop();
        }
        //apply velocity
        rigi.velocity = new Vector3(xVel, rigi.velocity.y, zVel);
        if (grounded == true)
        {
            boostTimer -= Time.fixedDeltaTime;
            boostInitialTimer -= Time.fixedDeltaTime;
        }
    }

    void CheckGrounded()
    {
        if (transform.position.y < 0.75f)
        {
            //Boost accel on land
            if (grounded == false)
            {
                if (boostTimer >= ringBoostTime)
                {
                    boostInitialTimer = forwardInitialAccelTime;
                    //rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + (ringInitialAccel * accelScaling.Evaluate(rigi.velocity.z)));
                }
            }
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    public void Wreck()
    {
        RuntimeManager.PlayOneShot(enemydeathSFX, gameObject.transform.position);
        if (aheadOfPlayer == true)
        {
            RaceAITrackerManager.playerPosition--;
            aheadOfPlayer = false;
        }
        GameObject wreckEffect = Instantiate(wreckEffectPrefab, transform.position, Quaternion.identity);
        wreckEffect.GetComponent<WreckEffect>().speed = rigi.velocity.z;
        Destroy(racerIcon);
        Destroy(gameObject);
    }

    public void NearMiss()
    {
        //Set NearMiss Boost Time
        boostTimer = Mathf.Clamp(boostTimer + nearMissBoostTime, nearMissBoostTime, 5000);
        boostInitialTimer = forwardInitialAccelTime;
        //rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + (nearMissInitialAccel * accelScaling.Evaluate(rigi.velocity.z)));
    }

    public void Ring()
    {
        boostTimer = Mathf.Clamp(boostTimer + ringBoostTime, ringBoostTime, 5000);
    }

    void OnTriggerEnter(Collider other)
    {
        float distFromPlayer = transform.position.z - player.position.z;
        if (other.transform.tag == "Obstacle" && distFromPlayer < activateCollisionDist)
        {
            Wreck();
        }
        if (other.transform.tag == "Ramp")
        {
            rigi.velocity = new Vector3(rigi.velocity.x, rampLaunchSpeed, rigi.velocity.z);
            RuntimeManager.PlayOneShot(enemyrampSFX, gameObject.transform.position);

        }
        else if (other.transform.tag == "Boost")
        {
            //Set Boost Time
            boostTimer = Mathf.Clamp(boostTimer + boostTime, boostTime, 5000);
            boostInitialTimer = forwardInitialAccelTime;
            //rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + (boostInitialAccel * accelScaling.Evaluate(rigi.velocity.z)));
            Destroy(other.gameObject);
            RuntimeManager.PlayOneShot(enemyboostSFX, gameObject.transform.position);
        }
    }
}
