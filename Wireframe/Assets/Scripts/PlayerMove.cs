using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    [Header("Move Variables")]
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

    [Header("Fly Variables")]
    public float rampLaunchSpeed;

    [Header("Boost Variables")]
    [Range(0f, 30f)]
    public float boostStartAccel;
    [Range(0f, 8f)]
    public float boostTime;

    [Header("NearMiss Variables")]
    [Range(0f, 30f)]
    public float nearMissStartAccel;
    [Range(0f, 5f)]
    public float nearMissBoostTime;

    [Header("References")]
    public Transform model;
    public TextMeshProUGUI speedGuageText;
    public ParticleSystem boostParticles;


    int moveInput;
    [HideInInspector]
    public float boostTimer;
    [HideInInspector]
    public Rigidbody rigi;
    // Start is called before the first frame update
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
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
        //Move or fly
        Move();
        boostTimer -= Time.fixedDeltaTime;
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
        if(boostTimer > 0)
        {
            zVel = Mathf.Clamp(zVel + forwardAccelSpeed, forwardSpeed, maxForwardSpeed);
            boostParticles.Play();
        }
        else
        {
            zVel = Mathf.Clamp(zVel - forwardDecelSpeed, forwardSpeed, maxForwardSpeed);
            boostParticles.Stop();
        }
        speedGuageText.text = "" + Mathf.Round(zVel);
        //apply velocity
        rigi.velocity = new Vector3(xVel, rigi.velocity.y, zVel);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Ramp")
        {
            rigi.velocity = new Vector3(rigi.velocity.x, rampLaunchSpeed, rigi.velocity.z);
        }
        else if(other.transform.tag == "Boost")
        {
            //Set Boost Time
            if(boostTimer < 0)
            {
                boostTimer = boostTime;
            }
            else
            {
                boostTimer += boostTime;
            }
            rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + boostStartAccel);
            Destroy(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "NearMiss")
        {
            //Set NearMiss Boost Time
            if (boostTimer < 0)
            {
                boostTimer = nearMissBoostTime;
            }
            else
            {
                boostTimer += nearMissBoostTime;
            }
            rigi.velocity = new Vector3(rigi.velocity.x, rigi.velocity.y, rigi.velocity.z + nearMissStartAccel);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "Obstacle")
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
