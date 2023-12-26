using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSky : MonoBehaviour
{
    public Material dayMat;
    public Material nightMat;
    public Material sunsetMat;
    public Material sunriseMat;

    private float currTime;

    public Color dayFog;
    public Color nightFog;
    public Color sunsetFog;
    public Color sunriseFog;

  
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.5f);
        currTime += Time.deltaTime;

        if (currTime > 70)
        {
            RenderSettings.skybox = nightMat;
            RenderSettings.fogColor = nightFog;
            if (currTime > 120)
            {
                currTime = 0;
            }
        }
        else if (currTime > 55)
        {
            RenderSettings.skybox = sunsetMat;
            RenderSettings.fogColor = sunsetFog;
        }
        else if (currTime > 15)
        {
            RenderSettings.skybox = dayMat;;
        }
        else
        {
            RenderSettings.skybox = sunriseMat;
            RenderSettings.fogColor = sunriseFog;
        }
    }
}
