using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For editing the noise parameters
[System.Serializable]
public class NoiseLayer
{
    public bool enabled = true;
    public bool useFirstLayerAsMask;
    
    //Settings for the noise
    public float strength = 0.1f;
    public float roughness = 1;
    public int NumOfNoiseCycles = 5;
    public float persistance = .5f; //amplitude halves each layer
    public float baseRoughness = 1;
    public float minValue;

    public Vector3 centre;//for the center of the noise, essentially the randomness

    //use the noise from libnoise-dotnet.
    Noise noise = new Noise();
    
    //use the noise with all the filters applied
    public float AddNoise(Vector3 point)
    {
        //float noiseValue = (noise.Evaluate(point * settings.roughness + settings.centre) + 1) * .5f;
        float noiseValue = 0;
        float frequency = baseRoughness;
        float amplitude = 1;

        //Do multiple times at decreasing amplitude for better effect
        for (int i = 0; i < NumOfNoiseCycles; i++)
        {
            float v = noise.Evaluate(point * frequency + centre);
            noiseValue += (v + 1) * .5f * amplitude;
            frequency *= roughness;
            amplitude *= persistance;
        }

        noiseValue = Mathf.Max(0, noiseValue - minValue);//clamp so anything with noise doesnt is set to below water level, leaving space for other object
        
        return noiseValue * strength;
    }
}
