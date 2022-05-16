using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    CinemachineVirtualCamera vCam;
    float shakeTimer;
    float startingIntensity;
    float shakeTimerTotal;

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void Shake(float intensity, float time, int flashIntensity = 0)
    {
        CinemachineBasicMultiChannelPerlin noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = intensity;
        shakeTimer = time;
        shakeTimerTotal = time;
        startingIntensity = intensity;
    }

    void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_AmplitudeGain = Mathf.Lerp(0f, startingIntensity, shakeTimer / shakeTimerTotal);
        }
    }
}
