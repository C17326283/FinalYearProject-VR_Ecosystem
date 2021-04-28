using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GetPointOnPlanet : MonoBehaviour
{

    public Transform core;
    public int spawnerDistanceFromCore = 5000;
    public int maxAttempts = 10;
    public GameObject spawnerObject;

    private void Start()
    {
        core = this.transform;
        spawnerObject = new GameObject("PointOnPlanetObject");
    }

    //Called from anything that needs to get a point on the planet
    public RaycastHit? GetPoint(String tagToSpawnOn, int maxAttempts)//use nullable
    {
        int attempts = 0;
        RaycastHit hitToReturn;
        
        //make multiple attempts to find a position but dont get stuck in a loop if its not possible
        while (attempts<maxAttempts)
        {
            Resposition();//put the spawner at a new position around the wordl to start the raycast
            
            RaycastHit hit; //shoot ray and if its ground then spawn at that location
            if (Physics.Raycast(spawnerObject.transform.position,
                core.position - spawnerObject.transform.position, out hitToReturn, 10000))
            {
                if (hitToReturn.transform.CompareTag(tagToSpawnOn)) //Checks its allowed spawn there
                {
                    attempts = maxAttempts + 1;//exit while
                    return hitToReturn;
                }
            }
            attempts++;
        }
        return null;
    }
    
    public void Resposition()
    {
        spawnerObject.transform.position = core.position;
        spawnerObject.transform.rotation  = Random.rotation;
        spawnerObject.transform.position = spawnerObject.transform.forward * spawnerDistanceFromCore;
    }
    
}
