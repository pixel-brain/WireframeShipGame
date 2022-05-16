using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    [Header("Move Variables")]
    public AnimationCurve accelScaling;
    public AnimationCurve decelScaling;
    public AnimationCurve decelDelaying;
    [Range(0f, 110f)]
    public float forwardSpeed;
    [Range(0f, 500f)]
    public float maxForwardSpeed;
    [Range(0f, 3f)]
    public float forwardDecelSpeed;
    [Range(0f, 3f)]
    public float forwardAccelSpeed;
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
    public float invinsibleTime;
    public float damageStartDecel;
    public Vector3 damageCamShake;

    [Header("Fly Variables")]
    public float rampLaunchSpeed;

    [Header("Boost Variables")]
    [Range(0f, 30f)]
    public float boostStartAccel;
    [Range(0f, 8f)]
    public float boostTime;

    [Header("Ring Variables")]
    [Range(0f, 30f)]
    public float ringLandAccel;
    [Range(0f, 8f)]
    public float ringBoostTime;

    [Header("NearMiss Variables")]
    [Range(0f, 30f)]
    public float nearMissStartAccel;
    [Range(0f, 5f)]
    public float nearMissBoostTime;

    [Header("References")]
    public Transform model;
    public TextMeshProUGUI speedGuageText;
    public ParticleSystem boostParticles;
    public CameraShake camShakeScript;
    Animator anim;


    int moveInput;
    [HideInInspector]
    public float boostTimer;
    float forwardDecelDelayTimer;
    [HideInInspector]
    public Rigidbody rigi;
    bool grounded;
    public static float invinsibleTimer;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody>();
        rigi.velocity = new Vector3(0, 0, forwardSpeed);
    }

    void FixedUpdate()
    {
        //Get Input
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
        CheckGrounded();
        //Move or fly
        Move();
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
        model.transform.localEulerAngles = new Vector3(model.transform.localEulerAngles.x, xVel / 6f, -xVel / 1.5f);
        //Set forward speed
        float zVel = rigi.velocity.z;
        if(invinsibleTimer > 0)
        {
            anim.SetBool("Invinsible", true);
        }
        else if(boostTimer > 0 && grounded == true)
        {
            zVel = Mathf.Clamp(zVel + (forwardAccelSpeed * accelScaling.Evaluate(zVel)), forwardSpeed, maxForwardSpeed);
            boostParticles.Play();
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
        if(grounded == true)
        {
            boostTimer -= Time.fixedDeltaTime;
        }
    }

    void CheckGrounded()
    {
        if(transform.position.y < 0.75f)
        {
            //Boost accel on land
            if(grounded == false && boostTimer >= ringBoostTime)
            {
                rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + (ringLandAccel * accelScaling.Evaluate(rigi.velocity.z)));
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
        rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + (nearMissStartAccel * accelScaling.Evaluate(rigi.velocity.z)));
    }

    public void TakeDamage()
    {
        camShakeScript.Shake(damageCamShake.x, damageCamShake.y, (int)damageCamShake.z);
        rigi.velocity = new Vector3(0, 0, 50);
        boostTimer = 0;
        invinsibleTimer = invinsibleTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Ramp" && invinsibleTimer < 0)
        {
            rigi.velocity = new Vector3(rigi.velocity.x, rampLaunchSpeed, rigi.velocity.z);
        }
        else if(other.transform.tag == "Boost" && invinsibleTimer < 0)
        {
            //Set Boost Time
            boostTimer = Mathf.Clamp(boostTimer + boostTime, boostTime, 5000);
            rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + (boostStartAccel * accelScaling.Evaluate(rigi.velocity.z)));
            Destroy(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "Ring")
        {
            boostTimer = Mathf.Clamp(boostTimer + ringBoostTime, ringBoostTime, 5000);
            Destroy(other.gameObject);
        }
    }
}
