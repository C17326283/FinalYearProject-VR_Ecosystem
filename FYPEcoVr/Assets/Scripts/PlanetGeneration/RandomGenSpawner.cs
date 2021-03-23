using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//for spawning the random objects onto the terrain 
//I used a pool system i had in an old project and adapted it to this, since im not despawnin objects they dont go back into the pool but it would be useful in future for animals
public class RandomGenSpawner : MonoBehaviour
{
    public GetPointOnPlanet getPointOnPlanetFinder;
    
    public bool waitUntillTriggered = true;
    [SerializeField]//Make private visible in inspector, need private so doesnt give error
    public ObjectPool normalObjectPoolObj;//Pool of the objects to pull from
    public ObjectPool coldObjectPoolObj;//Pool of the objects to pull from
    public ObjectPool warmObjectPoolObj;//Pool of the objects to pull from


    public GameObject parentObject;
    public GameObject planetObject;
    private Vector3 core;//for raycasting to for spawn points
    
    public int amountToSpawn = 10000;//also limited by pool max
    public int spawnerDistanceFromCore = 5000;
    public float heightFromHitPoint = 0;
    public String tagToSpawnOn = "Ground";
    public bool isRotatingObject;//for clouds

    //For adding variation
    [Header("Only randomises if condition is true")]
    public bool RandomiseScaleAndRotation = false;
    public float minScale = 1f;
    public float maxScale = 2f;
    public float randomXZTilt = 2f;

    private GameObject newObj;//declare here so can edit in reposition
    public GameObject[] biomeObjs;
    public float biomeDist=1000;//set dynamicallu

    // Start is called before the first frame update


    public bool usesLOD = true;
    public float screenPercentLODCull = 4;
    
    

    
    void Awake()
    {
        //start spawning objects
        if(waitUntillTriggered == false)
            StartCoroutine(Spawn());
    }

    public void TriggerSpawn()
    {
        //Move the spawner object
        //Resposition();
        /*
        //multObjectPoolObj = GetAssociatedPool();
        if (multObjectPoolObj == null)
            multObjectPoolObj = this.gameObject.GetComponent<ObjectPool>();
        */
        
        if (planetObject == null)
        {
            planetObject = this.transform.root.gameObject; //get the base object which will be the planet
            parentObject = new GameObject("Holder");
            if (isRotatingObject)
                parentObject.AddComponent<RotateEnvironment>();//for making the clouds move

        }

        core = planetObject.transform.position;
        normalObjectPoolObj.InitPool();//force pool to be initiated instead of waiting for awake
        coldObjectPoolObj.InitPool();
        warmObjectPoolObj.InitPool();

        
        StartCoroutine(Spawn());
    }
    


    //Spawn objects from pool in loop
    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(.1f);//to make sure the mesh was spawned correctly
        for (int i = 0; i < amountToSpawn; i++)
        {
            newObj = null;

            //getPointOnPlanet will return a point on planet(uses nullable incase you didnt get any hits)
            RaycastHit? hitOnPlanet = getPointOnPlanetFinder.GetPoint(tagToSpawnOn, 1);
            if (hitOnPlanet != null)
            {
                RaycastHit hit = hitOnPlanet.Value;
                //Find which biome object to spawn
                bool foundSpecificBiome = false;
                for (int j = 0; j < biomeObjs.Length - 1; j++)
                {
                    float dist = Vector3.Distance(biomeObjs[j].transform.position, hit.point);
                    if (dist < biomeDist)
                    {
                        foundSpecificBiome = true;
                        if (j < 2) //winter
                        {
                            newObj = coldObjectPoolObj.GetObj();
                        }
                        else
                        {
                            newObj = warmObjectPoolObj.GetObj();
                        }
                    }
                }

                if (foundSpecificBiome == false)
                    newObj = normalObjectPoolObj.GetObj();



                if (newObj != null)
                {
                    newObj.SetActive(true);

                    newObj.transform.position = hit.point; //place object at hit
                    newObj.transform.up = newObj.transform.position - core; //set rotation so orients properly
                    newObj.transform.position = newObj.transform.position + newObj.transform.up * heightFromHitPoint; //repoisition to correct height from hit


                    if (RandomiseScaleAndRotation)
                    {
                        //temp
                        newObj.transform.parent =
                            gameObject.transform.parent; //make its own parent so that scaling works after reactivating
                        //newObj.transform.parent = hit.transform; //make its own parent so that scaling works after reactivating


                        float scale = Random.Range(minScale, maxScale);
                        newObj.transform.localScale = Vector3.one * scale; //.one for all round scale
                        newObj.transform.Rotate(Random.Range(-randomXZTilt, randomXZTilt), Random.Range(0, 360),
                            Random.Range(-randomXZTilt, randomXZTilt));
                    }

                    newObj.transform.parent = parentObject.transform; //set parent to correct obj
                    //temp
                    //newObj.transform.parent = hit.transform; //make its own parent so that scaling works after reactivating
                    if (usesLOD)
                        AddLOD(newObj);
                }
            }
        }
    }
    
    //Move the spawner to a different positon around the globe
    public void Resposition()
    {
        GameObject obj = gameObject;
        obj.transform.position = core;
        obj.transform.rotation  = Random.rotation;
        gameObject.transform.position = transform.forward * spawnerDistanceFromCore;
    }

    public void AddLOD(GameObject obj)
    {
        LODGroup group = obj.AddComponent<LODGroup>();//component
        
        // Add 4 LOD levels
        LOD[] lods = new LOD[1];//amount of LODs excluding Cull
        Renderer[] renderers = new Renderer[1];//needs a list of renderer but we only have one
        renderers[0] = obj.GetComponent<MeshRenderer>();//set renderer to current one set, other will be cull
        lods[0] = new LOD(screenPercentLODCull/100, renderers);//set renderers and trigger %
        
        group.SetLODs(lods);
        //group.RecalculateBounds();
    }
}


/*
    //Get the pool based on the index number, this allows for infinite pools to be definted
    public ObjectPool GetAssociatedPool()
    {
        //See if any of the pools match and return that else return null
        foreach (var Pool in transform.root.GetComponents<ObjectPool>())
        {
            print("Pool.poolIndex"+Pool.poolIndex+"poolIndexNumber"+poolIndexNumber);
            if(poolIndexNumber == Pool.poolIndex)//if have matching index numbers. This is so clouds can have cloud settings etc.
                return Pool;
        }
        return null;
    }
    */