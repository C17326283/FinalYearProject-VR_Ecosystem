using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AnimalBrain : MonoBehaviour
{
    public AnimalProfile animalBaseDNA;
    
    public List<GameObject> objSensedMemory = new List<GameObject>();
    public List<GameObject> forgettingObjs = new List<GameObject>();//List for objects that animal is no longer touching but are still in memory

    public List<String> toHunt = new List<String>();
    public List<String> huntedBy = new List<String>();
    
    public Transform currentTarget;//set from behaviour tree so positioner can use

    public String name = "name";
    public float health = 100;
    public float hunger = 100;
    public float thirst = 100;
    public float reproductiveUrge = 0;
    public float age = 0;
    
    public float maxHealth = 100f;
    public float maxStat = 100;
    public float healthStarveDecrement = 0.01f;
    public float hungerDecrement = 0.02f;
    public float thirstDecrement = 0.01f;
    public float reproductiveIncrement = 0.01f;
    public float ageIncrement = 0.5f;
    
    public float memoryLossRate = 20;
    public float sensoryRange = 15;
    public float moveSpeed = 5;
    public float rotSpeed = 50;
    public float wanderRadius = 5;
    public float forwardWanderBias = 2;
    public float maxMutatePercent = 2;

    public float animalHeight;


    void Awake()
    {
        objSensedMemory = new List<GameObject>();
        forgettingObjs = new List<GameObject>();
        huntedBy = new List<String>();
        toHunt = new List<String>();
        //temp
        huntedBy.Add("Cheetah");
        toHunt.Add("Food");
        toHunt.Add("Chicken");
    }

    // Start is called before the first frame update
    void Start()
    {
        SetStatsFromDNA();
        Born();
    }

    // Update is called once per frame
    void Update()
    {
        hunger = hunger - hungerDecrement * Time.deltaTime;// div 100 to keep it within normal numbers for testing
        thirst = thirst - thirstDecrement * Time.deltaTime;
        reproductiveUrge = reproductiveUrge + reproductiveIncrement * Time.deltaTime;
        age = age + ageIncrement * Time.deltaTime;
        
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Died");
        Destroy(gameObject);//destroy after 20secs
    }

    public void Born()
    {
        //Need a better mating system with father and mother and get a base stat based on the 2 of them
          health = maxHealth;
          hunger = maxStat;
          thirst = maxStat;
          reproductiveUrge = 0;
          age = 0;
          MutateStats();
    }
    

    public void MutateStats()
    {
        float change = maxMutatePercent / 100;//get float as percent
        float percentDif = 1;

        percentDif = healthStarveDecrement * change;
        healthStarveDecrement = Mathf.Clamp(Random.Range(healthStarveDecrement-percentDif, healthStarveDecrement+percentDif), 0, 30000);
        percentDif = hungerDecrement * change;
        hungerDecrement = Mathf.Clamp(Random.Range(hungerDecrement-percentDif, hungerDecrement+percentDif), 0, 30000);
        percentDif = thirstDecrement * change;
        thirstDecrement = Mathf.Clamp(Random.Range(thirstDecrement-percentDif, thirstDecrement+percentDif), 0, 30000);
        percentDif = reproductiveIncrement * change;
        reproductiveIncrement = Mathf.Clamp(Random.Range(reproductiveIncrement-percentDif, reproductiveIncrement+percentDif), 0, 300000);
        percentDif = memoryLossRate * change;
        memoryLossRate = Mathf.Clamp(Random.Range(memoryLossRate-percentDif, memoryLossRate+percentDif), 0, 100000);
        percentDif = sensoryRange * change;
        sensoryRange = Mathf.Clamp(Random.Range(sensoryRange-percentDif, sensoryRange+percentDif), 0, 100000);
        percentDif = healthStarveDecrement * change;
        healthStarveDecrement = Mathf.Clamp(Random.Range(maxHealth-percentDif, maxHealth+percentDif), 0, 30000);
        percentDif = moveSpeed * change;
        moveSpeed = Mathf.Clamp(Random.Range(moveSpeed-percentDif, moveSpeed+percentDif), 0, 300000);
        percentDif = rotSpeed * change;
        rotSpeed = Mathf.Clamp(Random.Range(rotSpeed-percentDif, rotSpeed+percentDif), 0, 1000000);
        percentDif = wanderRadius * change;
        wanderRadius = Mathf.Clamp(Random.Range(wanderRadius-percentDif, wanderRadius+percentDif), 0, 300000);
        percentDif = forwardWanderBias * change;
        forwardWanderBias = Mathf.Clamp(Random.Range(forwardWanderBias-percentDif, forwardWanderBias+percentDif), 0, 300000);
    }

    public void SetStatsFromDNA()
    {
        name = animalBaseDNA.name;
        maxHealth = animalBaseDNA.maxHealth;
        maxStat = animalBaseDNA.maxStat;
        healthStarveDecrement = animalBaseDNA.healthStarveDecrement;
        hungerDecrement = animalBaseDNA.hungerDecrement;
        thirstDecrement = animalBaseDNA.thirstDecrement;
        reproductiveIncrement = animalBaseDNA.reproductiveIncrement;
        ageIncrement = animalBaseDNA.ageIncrement;
        
        memoryLossRate = animalBaseDNA.memoryLossRate;
        sensoryRange = animalBaseDNA.sensoryRange;
        moveSpeed = animalBaseDNA.moveSpeed;
        rotSpeed = animalBaseDNA.rotSpeed;
        wanderRadius = animalBaseDNA.wanderRadius;
        forwardWanderBias = animalBaseDNA.forwardWanderBias;
        maxMutatePercent = animalBaseDNA.maxMutatePercent;
//        print("SetStatsFromDNA"+animalBaseDNA.moveSpeed);
//        print("moveSpeed"+moveSpeed);
    }
    
}
