using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    public AudioSource audioSrc;
    float[] spectrumData = new float[64];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spectrumData = audioSrc.GetSpectrumData(64, 0, FFTWindow.Rectangular);
    }

}
