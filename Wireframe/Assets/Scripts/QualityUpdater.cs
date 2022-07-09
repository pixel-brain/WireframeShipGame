using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityUpdater : MonoBehaviour
{
    public ReflectionProbe reflectionProbe;
    public LayerMask everything;
    public LayerMask skySun;

    // Start is called before the first frame update
    void Start()
    {
        if(SettingsManager.qualityMode == true)
        {
            SetHighGraphics();
        }
        else
        {
            SetLowGraphics();
        }
    }

    public void SetHighGraphics()
    {
        reflectionProbe.cullingMask = everything;
        reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
    }

    public void SetLowGraphics()
    {
        reflectionProbe.cullingMask = skySun;
        reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
        reflectionProbe.RenderProbe();
    }
}
