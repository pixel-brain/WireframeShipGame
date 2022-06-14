using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AirRing : MonoBehaviour
{
    [SerializeField]
    EventReference ringSFX;

    public float ringInsideDist;
    public float ringOutsideDist;
    public float moveSpeed;
    public int particlesCount;
    public float particlesSpeedFactor;
    public float particlesShapeSize;
    public ParticleSystem particlesPrefab;

    [HideInInspector]
    public AirRingSpawner spawnerScript;
    Transform player;
    Vector3 targetPos;
    Vector3 initialPos;
    float moveTimer;
    Animator anim;
    bool failed;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        initialPos = transform.position;
        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(failed == false && player.position.z > transform.position.z - 1.5f && moveTimer > 1f)
        {
            float xDist = Mathf.Abs(player.position.x - transform.position.x);
            if(xDist < ringInsideDist)
            {
                RuntimeManager.PlayOneShot(ringSFX);
                //Center
                player.GetComponent<PlayerMove>().Ring();
                ParticleSystem particles = Instantiate(particlesPrefab, transform.position, Quaternion.identity);
                //Set Particle Shape
                var shape = particles.shape;
                shape.scale = Vector3.one * particlesShapeSize;
                //Set Particle Velocity
                float vel = particlesSpeedFactor * GameObject.Find("Player").GetComponent<Rigidbody>().velocity.z;
                ParticleSystem.VelocityOverLifetimeModule pVelocity = particles.velocityOverLifetime;
                pVelocity.z = new ParticleSystem.MinMaxCurve(vel/3, vel);
                //Set Particle Burst
                particles.emission.SetBurst(0, new ParticleSystem.Burst(0f, particlesCount));

                Destroy(gameObject);
            }
            else if(xDist > ringOutsideDist)
            {
                //Miss
                failed = true;
            }
            else
            {
                //Hit
                if (spawnerScript.state <= 3)
                {
                    anim.SetTrigger("Knockback");
                }
                else
                {
                    anim.SetTrigger("HitLast");
                    failed = true;
                }
                spawnerScript.state++;
                targetPos = spawnerScript.SpawnPos();
                targetPos = new Vector3(targetPos.x, Mathf.Clamp(targetPos.y, 10, 100), targetPos.z);
                initialPos = transform.position;
                moveTimer = 0;
            }
        }
        transform.position = Vector3.Lerp(initialPos, targetPos, moveTimer);
        moveTimer += Time.deltaTime * moveSpeed;
    }
}
