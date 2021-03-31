using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//for spawning the random objects onto the terrain 
//I used a pool system i had in an old project and adapted it to this, since im not despawnin objects they dont go back into the pool but it would be useful in future for animals
public class AnimalsSpawner : MonoBehaviour
{
    public bool waitUntillTriggered = true;
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
    
    void Awake()
    {
        //start spawning objects
        if (waitUntillTriggered == false)
        {
            StartCoroutine(Spawn());
        }
    }

    public void TriggerSpawn()
    {
        //Move the spawner object
        Resposition();
        
        if (planetObject == null)
        {
            planetObject = this.transform.root.gameObject; //get the base object which will be the planet
            parentObject = new GameObject("Holder");
            
        }

        core = planetObject.transform.position;

        
        StartCoroutine(Spawn());
    }
    


    //Spawn objects from pool in loop
    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(.1f);//to make sure the mesh was spawned correctly
        GetPointOnPlanet pointFinder = GameObject.Find("Core").GetComponent<GetPointOnPlanet>();
        for (int i = 0; i < amountToSpawn/groupAmount; i++)
        {
            AnimalProfile animalToSpawn = animalProfiles[Random.Range(0, animalProfiles.Length)];
            for (int j = 0; j < groupAmount; j++)
            {
                if (i % 500 == 0)//Split processing into chunks because too many at once can cause issues
                {
                    //print("mod 500");
                    yield return new WaitForSeconds(.1f);
                }
                newObj = null;
                
                RaycastHit? hitPoint = pointFinder.GetPoint("Ground", 2);
                if (hitPoint != null)
                {
                    RaycastHit hit = hitPoint.Value;
                    //                Debug.Log("hit"+hit.transform.name + hit.transform.position);
                    if (hit.transform.CompareTag(tagToSpawnOn)) //Checks its allowed spawn there
                    {
                        newObj = new GameObject("Animalholder");
                        newObj.transform.up = hit.normal;
                        newObj.transform.parent = parentObject.transform;

                        //newObj.transform.position = hit.point; //place object at hit
                        //newObj.transform.up = newObj.transform.position - core; //set rotation so orients properly
                        newObj.transform.position = hit.point + newObj.transform.up * heightFromHitPoint; //repoisition to correct height from hit
                        
                        
                        AnimalInitializer manager = newObj.AddComponent<AnimalInitializer>();
                        manager.animalDNA = animalToSpawn;
                        manager.btTexts = btTexts;
                        
                        // print(newObj.transform.position);
                        manager.InitialiseAnimal();
                        
                    }
                }
                else
                {
                    //Debug.Log("no hit");
                }
            }
            //Resposition();
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
