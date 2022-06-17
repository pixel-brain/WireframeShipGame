using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class PlayerMove : MonoBehaviour
{
    [Header("Effects Variables")]
    public float minFOV;
    public float maxFOV;
    public float minCamDist;
    public float maxCamDist;
    public float camLagTime;
    public float camLagAmount;
    public AnimationCurve camLag;
    public float minSpeedCamShakeIntensity;
    public float maxSpeedCamShakeIntensity;
    public float shakeCamFrequency;
    public float speedLinesMinTransparency;
    public float speedLinesMaxTransparency;
    public float speedLinesMinSpawnRate;
    public float speedLinesMaxSpawnRate;
    public float speedLinesMinRadius;
    public float speedLinesMaxRadius;

    [Header("Move Variables")]
    public AnimationCurve accelScaling;
    public AnimationCurve decelScaling;
    public AnimationCurve decelDelaying;
    [Range(0f, 200f)]
    public float startSpeed;
    [Range(0f, 110f)]
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

    [Header("Damage Variables")]
    [Range(0f, 3f)]
    public float invinsibleTime;
    [Range(0f, 110f)]
    public float damagedForwardSpeed;
    public Vector3 damageCamShake;

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

    [Header("Takedown Variables")]
    [Range(0f, 8f)]
    public float takedownBoostTime;

    [Header("Ring Variables")]
    [Range(0f, 8f)]
    public float ringBoostTime;

    [Header("NearMiss Variables")]
    [Range(0f, 5f)]
    public float nearMissBoostTime;
    public Vector3 nearMissStartPos;
    public Vector3 nearMissStartScale;
    public Vector3 nearMissEndPos;
    public Vector3 nearMissEndScale;

    [Header("References")]
    public Transform model;
    public TextMeshProUGUI speedGuageText;
    public ParticleSystem boostParticles;
    public CameraShake camShakeScript;
    public CinemachineVirtualCamera vCam;
    public ParticleSystem speedLinesParticles;
    public BoostMessager boostMessageScript;
    public BoxCollider nearMissCheckCol;


    Animator anim;


    int moveInput;
    [HideInInspector]
    public float boostTimer;
    [HideInInspector]
    public float boostInitialTimer;
    float forwardDecelDelayTimer;
    [HideInInspector]
    public Rigidbody rigi;
    bool grounded;
    public static float invinsibleTimer;
    float offsetFollowDistance;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody>();
        rigi.velocity = new Vector3(0, 0, startSpeed);
    }

    void FixedUpdate()
    {
        //Get Input
        /*
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.position.x > Screen.width / 2)
            {
                moveInput = 1;
            }
            else
            {
                moveInput = -1;
            }
        }
        else
        {
            moveInput = 0;
        }
        */
        moveInput = (int)Input.GetAxisRaw("Horizontal");
        CheckGrounded();
        //Move
        Move();

        //----------------Apply speed feel effects--------------------
        float speedLerp = Mathf.Clamp((rigi.velocity.z - forwardSpeed) / (maxForwardSpeed - forwardSpeed), 0, 1);
        //FOV
        vCam.m_Lens.FieldOfView = Mathf.Lerp(minFOV, maxFOV, speedLerp);
        //Camera Shake
        if(camShakeScript.shakeTimer <= 0)
        {
            CinemachineBasicMultiChannelPerlin noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_FrequencyGain = shakeCamFrequency;
            noise.m_AmplitudeGain = Mathf.Clamp(Mathf.Lerp(minSpeedCamShakeIntensity, maxSpeedCamShakeIntensity, speedLerp), 0, Mathf.Infinity);
        }
        //Camera Follow Distance
        CinemachineComponentBase componentBase = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        float targetFollowDistance = -Mathf.Lerp(minCamDist, maxCamDist, speedLerp);

        //Camera lag behind on boost
        if(grounded)
        {
            float initialBoostLerp = 1f - Mathf.Clamp((boostInitialTimer + camLagTime - forwardInitialAccelTime) / camLagTime, 0, 1);
            offsetFollowDistance += camLag.Evaluate(initialBoostLerp) * camLagAmount;
            offsetFollowDistance = Mathf.Clamp(offsetFollowDistance, 0f, 100f);
            (componentBase as CinemachineFramingTransposer).m_TrackedObjectOffset.z = targetFollowDistance - offsetFollowDistance;
        }

        //SpeedLines
        var main = speedLinesParticles.main;
        Color32 lerpedColor = new Color32(200, 200, 200, (byte)Mathf.Clamp(Mathf.Lerp(speedLinesMinTransparency, speedLinesMaxTransparency, speedLerp), 0, Mathf.Infinity));
        main.startColor = new ParticleSystem.MinMaxGradient(new Color32(255,255,255,4), lerpedColor);
        var emission = speedLinesParticles.emission;
        emission.rateOverTime = Mathf.Lerp(speedLinesMinSpawnRate, speedLinesMaxSpawnRate, speedLerp);

        ParticleSystem.ShapeModule shape = speedLinesParticles.GetComponent<ParticleSystem>().shape;
        shape.radius = Mathf.Lerp(speedLinesMinRadius, speedLinesMaxRadius, speedLerp);

        invinsibleTimer -= Time.fixedDeltaTime;
        if(invinsibleTimer < 0)
        {
            anim.SetBool("Invinsible", false);
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
        else if(Mathf.Abs(moveInput) > 0.35f)
        {
            xVel *= Mathf.Pow((1f - turnDamping), 0.02f);
        }
        //no input
        else
        {
            xVel *= Mathf.Pow((1f - stopDamping), 0.02f);
        }
        //stop slight drift
        if(Mathf.Abs(xVel) < 0.5f)
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
        if(invinsibleTimer > 0)
        {
            anim.SetBool("Invinsible", true);
            zVel = damagedForwardSpeed;
        }
        else if(boostTimer > 0 && grounded == true)
        {
            if(boostInitialTimer > 0)
            {
                zVel += accelScaling.Evaluate(zVel) * forwardInitialAccelSpeed;
                boostParticles.Play();
            }
            zVel = Mathf.Clamp(zVel + (forwardAccelSpeed * accelScaling.Evaluate(rigi.velocity.z)), forwardSpeed, maxForwardSpeed);
            forwardDecelDelayTimer = 0;
        }
        else if(grounded == true)
        {
            zVel = Mathf.Clamp(zVel - (forwardDecelSpeed * decelScaling.Evaluate(zVel) * decelDelaying.Evaluate(forwardDecelDelayTimer)), forwardSpeed, maxForwardSpeed);
            boostParticles.Stop();
            forwardDecelDelayTimer += Time.fixedDeltaTime;
        }
        else
        {
            boostParticles.Stop();
        }
        speedGuageText.text = "" + Mathf.Round(zVel);
        //apply velocity
        rigi.velocity = new Vector3(xVel, rigi.velocity.y, zVel);
        //update topSpeed
        if(Mathf.Round(zVel) > StatsTracker.topSpeed)
        {
            StatsTracker.topSpeed = (int)Mathf.Round(zVel);
        }
        //Update near miss collider
        nearMissCheckCol.center = Vector3.Lerp(nearMissStartPos, nearMissEndPos, (zVel - forwardSpeed) / (maxForwardSpeed - forwardSpeed));
        nearMissCheckCol.size = Vector3.Lerp(nearMissStartScale, nearMissEndScale, (zVel - forwardSpeed) / (maxForwardSpeed - forwardSpeed));

        if (grounded == true)
        {
            boostTimer -= Time.fixedDeltaTime;
            boostInitialTimer -= Time.fixedDeltaTime;
        }
    }

    void CheckGrounded()
    {
        if(transform.position.y < 0.75f)
        {
            //Boost accel on land
            if(grounded == false)
            {
                anim.SetTrigger("Land");
                if(boostTimer >= ringBoostTime)
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

    public void NearMiss()
    {
        //Set NearMiss Boost Time
        boostTimer = Mathf.Clamp(boostTimer + nearMissBoostTime, nearMissBoostTime, 5000);
        boostInitialTimer = forwardInitialAccelTime;
        StatsTracker.closeCalls++;
        boostMessageScript.SpawnMessage("Close Call");
        //rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + (nearMissInitialAccel * accelScaling.Evaluate(rigi.velocity.z)));
    }

    public void Ring()
    {
        boostTimer = Mathf.Clamp(boostTimer + ringBoostTime, ringBoostTime, 5000);
        boostMessageScript.SpawnMessage("Ring");
    }

    void Takedown()
    {
        boostTimer = Mathf.Clamp(boostTimer + nearMissBoostTime, nearMissBoostTime, 5000);
        boostInitialTimer = forwardInitialAccelTime;
        StatsTracker.takedowns++;
        boostMessageScript.SpawnMessage("Takedown");
    }

    public void TakeDamage()
    {
        camShakeScript.Shake(damageCamShake.x, damageCamShake.y, (int)damageCamShake.z);
        rigi.velocity = new Vector3(0, 0, damagedForwardSpeed);
        boostTimer = 0;
        invinsibleTimer = invinsibleTime;
    }
    void OnCollisionEnter(Collision other)
    {
        //Takedown AI racer
        if (other.transform.tag == "AIRacer")
        {
            other.transform.GetComponent<AIRacer>().Wreck();
            Takedown();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Ramp" && invinsibleTimer < 0)
        {
            rigi.velocity = new Vector3(rigi.velocity.x, rampLaunchSpeed, rigi.velocity.z);
        }
        else if(other.transform.tag == "Boost" && invinsibleTimer < 0)
        {
            //Set Boost Time
            boostTimer = Mathf.Clamp(boostTimer + boostTime, boostTime, 5000);
            boostInitialTimer = forwardInitialAccelTime;
            //rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + (boostInitialAccel * accelScaling.Evaluate(rigi.velocity.z)));
            Destroy(other.gameObject);
            boostMessageScript.SpawnMessage("Boost Pad");
        }
    }
}
