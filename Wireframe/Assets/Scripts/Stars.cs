using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class StarBand
{
    [SerializeField] public float proportion, fromXZ, toXZ, fromY, toY;
}

public class Stars : MonoBehaviour
{
    [SerializeField] ParticleSystem starsParticleSystem;
    [SerializeField] int starCount = 1000;
    [SerializeField] StarBand[] spreads;

    private ParticleSystem.Particle[] particles;
    private byte[] alphas;
    private Color32 color32;
    private float radius, spreadSun, spreadChooser, sum, angleXZ, angleY;
    private int chosenSpread;
    private Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        alphas = new byte[starCount];
        particles = new ParticleSystem.Particle[starCount];

        starsParticleSystem.Emit(starCount);
        starsParticleSystem.GetParticles(particles, starCount, 0);
        radius = starsParticleSystem.shape.radius;
        spreadSun = 0f;
        foreach(StarBand spread in spreads)
        {
            spreadSun += spread.proportion;
        }
        for(int i=0; i<particles.Length; i++)
        {
            alphas[i] = particles[i].startColor.a;
            spreadChooser = Random.Range(0f, spreadSun);
            sum = 0f;
            for (int ii=0; ii<spreads.Length; i++)
            {
                sum += spreads[ii].proportion;
                if(spreadChooser < sum)
                {
                    chosenSpread = ii;
                    ii = spreads.Length;
                }
            }
            angleXZ = Random.Range(Mathf.Deg2Rad * spreads[chosenSpread].fromXZ, Mathf.Deg2Rad * spreads[chosenSpread].toXZ);
            angleY = Random.Range(Mathf.Deg2Rad * spreads[chosenSpread].fromY, Mathf.Deg2Rad * spreads[chosenSpread].toY);

            position.x = radius * Mathf.Cos(angleXZ) * Mathf.Sin(angleY);
            position.z = radius * Mathf.Sin(angleXZ) * Mathf.Sin(angleY);
            position.y = radius * Mathf.Cos(angleY);
            particles[i].position = position;
        }
        starsParticleSystem.SetParticles(particles, starCount);
    }

    /*
    void Update()
    {
        for (int i=0; i< particles.Length; i++)
        {
            color32 = particles[i].startColor;
            color32.a = (byte)Mathf.Clamp(alphas[i] * (starsParticleSystem.transform.TransformPoint(particles[i].position).y - starsParticleSystem.transform.position.y) / radius, 0, alphas[i]);
            particles[i].startColor = color32;
        }
        starsParticleSystem.SetParticles(particles, particles.Length, 0);
    }
    */
}
