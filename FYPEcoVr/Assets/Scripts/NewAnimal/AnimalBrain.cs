using System;
using System.Collections;
using System.Collections.Generic;
using Panda;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//Control the individual stats of this animal that come from their dna
public class AnimalBrain : MonoBehaviour
{
    public AnimalProfile animalBaseDNA;
    
    //public Transform currentTarget;//set from behaviour tree so positioner can use

    [Header("Descriptors")]
    public String name = "name";
    public int generation = 0;
    
    [Header("Current Levels")]
    public float health = 100;
    public float hunger = 100;
    public float thirst = 100;
    public float reproductiveUrge = 0;
    public float age = 0;
    public float packBravery = 0;
    public float tiredness = 0;//
    
    [Header("DNA")]
    public float maxHealth = 100f;
    public float healthStarveDecrement = 0.01f;
    public float hungerDecrement = 2f;
    public float thirstDecrement = 1f;
    public float tiredIncrement = 3f;
    public float reproductiveIncrement = 0.01f;
    public float memoryLossRate = 20;
    public float sensoryRange = 15;
    public float moveSpeed = 5;
    public float wanderRadius = 5;
    public float attackRate = 1;
    public float attackDamage = 10;
    public float litterSize = 1;
    public bool eatsPlants = false;
    public bool eatsMeat = true;
    public float predatorRating = 1;
    public float preyRating = 1;
    public float hungerThresh = 50;
    public float thirstThresh = 50;
    public float mateThresh = 90;
    public bool genderIsMale = true;
    public float deathAge = 100f;
    
    [Header("Static stats")]
    public float foodWorth = 2;
    public float maxMutatePercent = 2;
    public float forwardWanderBias = 2;
    public float ageIncrement = 0.02f;
    public float maxStat = 100;

    [Header("Set stats")]
    public float animalHeight;
    public float animalLength;

    [Header("Parents")]
    public AnimalBrain mother;
    public AnimalBrain father;
    
    [Header("Memory lists")]
    public List<GameObject> objSensedMemory = new List<GameObject>();
    public List<GameObject> forgettingObjs = new List<GameObject>();//List for objects that animal is no longer touching but are still in memory

    
    public float bounceMult = 1;
    

    void Awake()
    {
        objSensedMemory = new List<GameObject>();
        forgettingObjs = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetStatsFromDNA();
        Born();
        packBravery = predatorRating;//set default
    }
    
    public void Born()
    {
        //Need a better mating system with father and mother and get a base stat based on the 2 of them
        health = maxHealth;
        hunger = maxStat;
        thirst = maxStat;
        reproductiveUrge = 0;
        age = 0;
        generation = 1;
        genderIsMale = (Random.value > 0.5f);//random.value returns 0 to 1 so returns true or false with 50/50 odds
          
        //If has parents then mutate instead of doing default 
        if (mother != null)
        {
            if (mother.generation >= father.generation)
                generation = mother.generation + 1;
            else
                generation = father.generation + 1;
            MutateStats();
        }
        else
        {
            //Set default stuff 
            generation = Random.Range(1,4);//small variation in start ones
            //Set self as parent so it can have a mutation on first generation for more interest
            SetStatsFromDNA();
            mother = GetComponent<AnimalBrain>();
            father = GetComponent<AnimalBrain>();
            
            //Mutate based on start gen
            for (int i = 0; i < generation; i++)
            {
                MutateStats();
            }
            
            //start animals should start as if they had been running
            hunger = Random.Range(maxStat/2,maxStat);
            thirst = Random.Range(maxStat/2,maxStat);
            reproductiveUrge = Random.Range(maxStat/2,maxStat);
            
        }
    }



    public void MutateStats()
    {
        float change = maxMutatePercent / 100;//get float as percent
        float percentDif = 1;
        
        //Get average from parents and add a mutation
        percentDif = mother.maxHealth * change;
        maxHealth = Mathf.Clamp(((mother.maxHealth+father.maxHealth)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = healthStarveDecrement * change;
        healthStarveDecrement = Mathf.Clamp(((mother.healthStarveDecrement+father.healthStarveDecrement)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = hungerDecrement * change;
        hungerDecrement = Mathf.Clamp(((mother.hungerDecrement+father.hungerDecrement)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = thirstDecrement * change;
        thirstDecrement = Mathf.Clamp(((mother.thirstDecrement+father.thirstDecrement)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = reproductiveIncrement * change;
        reproductiveIncrement = Mathf.Clamp(((mother.reproductiveIncrement+father.reproductiveIncrement)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);//Dont let reproduce before they get hungry
        percentDif = memoryLossRate * change;
        memoryLossRate = Mathf.Clamp(((mother.memoryLossRate+father.memoryLossRate)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = sensoryRange * change;
        sensoryRange = Mathf.Clamp(((mother.sensoryRange+father.sensoryRange)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = moveSpeed * change;
        moveSpeed = Mathf.Clamp(((mother.moveSpeed+father.moveSpeed)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = wanderRadius * change;
        wanderRadius = Mathf.Clamp(((mother.wanderRadius+father.wanderRadius)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = attackRate * change;
        attackRate = Mathf.Clamp(((mother.attackRate+father.attackRate)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = attackDamage * change;
        attackDamage = Mathf.Clamp(((mother.attackDamage+father.attackDamage)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = litterSize * change;
        litterSize = Mathf.Clamp(((mother.litterSize+father.litterSize)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = predatorRating * change;
        predatorRating = Mathf.Clamp(((mother.predatorRating+father.predatorRating)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = preyRating * change;
        preyRating = Mathf.Clamp(((mother.preyRating+father.preyRating)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = hungerThresh * change;
        hungerThresh = Mathf.Clamp(((mother.hungerThresh+father.hungerThresh)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = thirst * change;
        thirstThresh = Mathf.Clamp(((mother.thirstThresh+father.thirstThresh)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = mateThresh * change;
        mateThresh = Mathf.Clamp(((mother.mateThresh+father.mateThresh)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = deathAge * change;
        deathAge = Mathf.Clamp(((mother.deathAge+father.deathAge)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = mateThresh * change;
        mateThresh = Mathf.Clamp(((mother.mateThresh+father.mateThresh)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = mateThresh * change;
        mateThresh = Mathf.Clamp(((mother.mateThresh+father.mateThresh)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);

        
        
        
        //Evolve to eat new things
        if (!eatsPlants && Random.Range(0, 100) < maxMutatePercent/2) //X% chance of being true
        {
            eatsPlants = true;
        }
        if (!eatsMeat && Random.Range(0, 100) < maxMutatePercent/2) //X% chance of being true
        {
            eatsMeat = true;
        }
        
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
        deathAge = animalBaseDNA.deathAge;
        
        memoryLossRate = animalBaseDNA.memoryLossRate;
        sensoryRange = animalBaseDNA.sensoryRange;
        moveSpeed = animalBaseDNA.moveSpeed;
        wanderRadius = animalBaseDNA.wanderRadius;
        forwardWanderBias = animalBaseDNA.forwardWanderBias;
        maxMutatePercent = animalBaseDNA.maxMutatePercent;
        predatorRating = animalBaseDNA.predatorRating;
        preyRating = animalBaseDNA.preyRating;
        eatsPlants = animalBaseDNA.eatsPlants;
        eatsMeat = animalBaseDNA.eatsMeat;
        hungerThresh = animalBaseDNA.hungerThresh;
        thirstThresh = animalBaseDNA.thirstThresh;
        mateThresh = animalBaseDNA.mateThresh;

        attackDamage = animalBaseDNA.attackDamage;
        attackRate = animalBaseDNA.attackRate;
        litterSize = animalBaseDNA.litterSize;
        foodWorth = animalBaseDNA.foodWorth;
        
        bounceMult = animalBaseDNA.bounceMult;
    }
}
