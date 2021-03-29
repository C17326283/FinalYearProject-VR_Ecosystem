using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalLife : MonoBehaviour
{
    public BiomeManager biomeManager;
    public String debuff;

    public AnimalBrain brain;
    public GameObject core;

    public float timeToCheckBiome = 2;
    
    // Start is called before the first frame update
    void Start()
    {
        biomeManager = core.GetComponent<BiomeManager>();
    }

    private void OnEnable()
    {
        StartCoroutine(CheckForBiomeDebuff());
    }
    
    private void OnDisable()
    {
        StopCoroutine(CheckForBiomeDebuff());
    }

    IEnumerator CheckForBiomeDebuff()
    {
        yield return new WaitForSeconds(timeToCheckBiome);

        while (true)
        {
            yield return new WaitForSeconds(timeToCheckBiome);
            String biome = biomeManager.GetClosestBiome(transform.position);
                
            if(biome =="Desert")
            {
                debuff = "Hot";
            }
            else if(biome =="Winter")
            {
                debuff = "Cold";
            }
            else
            {
                debuff = "None";
            }
        }
    }

}
