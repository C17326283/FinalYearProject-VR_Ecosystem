using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object for holding all the settings
[CreateAssetMenu]
public class PlanetSettings : ScriptableObject
{
    //public Gradient[] gradients;
    //Have a list of the gradients and the materials to put them on for each biome
    public Gradient[] biomeGradients;
    public Material planetMaterial;
    public Material waterMaterial;

    [HideInInspector]
    public float planetRadius =100;//always public, could change in future but scaling works for other planets
    public NoiseLayer[] noiseLayers;//For all the height variation of the verticies
    public Boolean havePoles = true;
}
