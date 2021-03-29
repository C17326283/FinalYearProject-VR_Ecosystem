using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public GameObject[] biomeObjs;
    public float biomeDist=500;

    public String GetClosestBiome(Vector3 posToCompare)
    {
        String biome = "Normal";
        for (int j = 0; j < biomeObjs.Length - 1; j++)
        {
            float dist = Vector3.Distance(biomeObjs[j].transform.position, posToCompare);
            
            //Check if within distance of certain biome
            if (dist < biomeDist)
            {
                if (j < 2) //Winter biomes are generated first 
                {
                    biome = "Winter";
                }
                else
                {
                    biome = "Desert";
                }
            }
        }

        return biome;
    }
}
