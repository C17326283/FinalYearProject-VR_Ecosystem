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

    
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        hunger = maxStat;
        thirst = maxStat;
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
          health = 100;
          hunger = 100;
          thirst = 100;
          reproductiveUrge = 0;
          age = 0;
          MutateStats();
    }

    public void MutateStats()
    {
        float change = maxMutatePercent / 100;//get float as percent
        float percentDif = 0;

        percentDif = healthStarveDecrement * change;
        healthStarveDecrement = Mathf.Clamp(Random.Range(healthStarveDecrement-percentDif, healthStarveDecrement+percentDif), 0, 30);
        percentDif = hungerDecrement * change;
        hungerDecrement = Mathf.Clamp(Random.Range(hungerDecrement-percentDif, hungerDecrement+percentDif), 0, 30);
        percentDif = thirstDecrement * change;
        thirstDecrement = Mathf.Clamp(Random.Range(thirstDecrement-percentDif, thirstDecrement+percentDif), 0, 30);
        percentDif = reproductiveIncrement * change;
        reproductiveIncrement = Mathf.Clamp(Random.Range(reproductiveIncrement-percentDif, reproductiveIncrement+percentDif), 0, 30);
        percentDif = memoryLossRate * change;
        memoryLossRate = Mathf.Clamp(Random.Range(memoryLossRate-percentDif, memoryLossRate+percentDif), 0, 100);
        percentDif = sensoryRange * change;
        sensoryRange = Mathf.Clamp(Random.Range(sensoryRange-percentDif, sensoryRange+percentDif), 0, 100);
        percentDif = healthStarveDecrement * change;
        healthStarveDecrement = Mathf.Clamp(Random.Range(maxHealth-percentDif, maxHealth+percentDif), 0, 30);
        percentDif = moveSpeed * change;
        moveSpeed = Mathf.Clamp(Random.Range(moveSpeed-percentDif, moveSpeed+percentDif), 0, 30);
        percentDif = rotSpeed * change;
        rotSpeed = Mathf.Clamp(Random.Range(rotSpeed-percentDif, rotSpeed+percentDif), 0, 100);
        percentDif = wanderRadius * change;
        wanderRadius = Mathf.Clamp(Random.Range(wanderRadius-percentDif, wanderRadius+percentDif), 0, 30);
        percentDif = forwardWanderBias * change;
        forwardWanderBias = Mathf.Clamp(Random.Range(forwardWanderBias-percentDif, forwardWanderBias+percentDif), 0, 30);
    }
}
