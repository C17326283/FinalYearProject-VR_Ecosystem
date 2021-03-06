using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//for spawning the random objects onto the terrain 
//I used a pool system i had in an old project and adapted it to this, since im not despawnin objects they dont go back into the pool but it would be useful in future for animals
public class AnimalsSpawner : MonoBehaviour
{
    [SerializeField]//Make private visible in inspector, need private so doesnt give error
    public GameObject parentObject;
    public GameObject planetObject;
    private Vector3 core;//for raycasting to for spawn points
    
    public int amountToSpawn = 10000;//also limited by pool max
    public int spawnerDistanceFromCore = 5000;
    public float heightFromHitPoint = 0;
    public String tagToSpawnOn = "Ground";

    public AnimalProfile[] animalProfiles;
    public TextAsset[] btTexts;
    
    private GameObject newObj;//declare here so can edit in reposition
    public int groupAmount = 3;

    // Start is called before the first frame update
    

    public void TriggerSpawn(FinGenSequence genSequenceScript)
    {
        //Move the spawner object
        Resposition();
        
        if (planetObject == null)
        {
            planetObject = this.transform.root.gameObject; //get the base object which will be the planet
            parentObject = new GameObject("Holder");
            
        }

        core = planetObject.transform.position;

        
        StartCoroutine(Spawn(genSequenceScript));
    }
    


    //Spawn objects from pool in loop
    IEnumerator Spawn(FinGenSequence genSequenceScript)
    {
        yield return new WaitForSeconds(.1f);//to make sure the mesh was spawned correctly
        GetPointOnPlanet pointFinder = GameObject.Find("Core").GetComponent<GetPointOnPlanet>();
        int i = 0;
        while (i < amountToSpawn)//Increments on successful spawn, allows group spawning
        {

            AnimalProfile animalToSpawn = animalProfiles[Random.Range(0, animalProfiles.Length)];
            
            if (i % 250 == 0)//Split processing into chunks because too many at once can cause issues
            {
                int amountOfSpawnGroups = amountToSpawn / 250;
                genSequenceScript.IncreaseLoadProgress(35/amountOfSpawnGroups,"Generating Animals");
                yield return new WaitForSeconds(.05f);
            }
            newObj = null;
            
            RaycastHit? hitPoint = pointFinder.GetPoint("Ground", 2);//get point
            if (hitPoint != null)
            {
                RaycastHit hit = hitPoint.Value;//get hit from possible hit
                if (hit.transform.CompareTag(tagToSpawnOn)) //Checks its allowed spawn there
                {
                    int groupSize = Random.Range(1, 3);//Spawn a group of random size up to 3
                    for (int j = 0; j < groupSize; j++)
                    {
                        newObj = new GameObject("Animalholder");
                        newObj.transform.up = hit.normal;
                        float randDist = Random.Range(-4, 4);//random dist from main spawned animal, spawn others close
                        //repoisition to correct height from hit
                        newObj.transform.position = hit.point + (newObj.transform.up* heightFromHitPoint)
                                                              +(newObj.transform.right*(j*randDist)
                                                                +(newObj.transform.forward*(j*randDist))); 
                        newObj.transform.parent = parentObject.transform;

                        //add the starting components
                        AnimalInitializer manager = newObj.AddComponent<AnimalInitializer>();
                        manager.animalDNA = animalToSpawn;
                        manager.btTexts = btTexts;
                        manager.InitialiseAnimal();
                        i++;
                    }
                }
            }
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
    
    //Move the spawner to a different positon around the globe
    public void Resposition()
    {
        gameObject.transform.position = core;
        gameObject.transform.rotation  = Random.rotation;
        gameObject.transform.position = transform.forward * spawnerDistanceFromCore;
    }
}
