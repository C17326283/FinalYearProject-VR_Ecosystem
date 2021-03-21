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
    
    [Header("DNA")]
    public float maxHealth = 100f;
    public float healthStarveDecrement = 0.01f;
    public float hungerDecrement = 0.02f;
    public float thirstDecrement = 0.01f;
    public float reproductiveIncrement = 0.01f;
    public float memoryLossRate = 20;
    public float sensoryRange = 15;
    public float moveSpeed = 5;
    public float wanderRadius = 5;
    public float attackRate = 1;
    public float attackDamage = 10;
    
    public float litterSize = 1;
    public bool eatsPlants = false;
    public float predatorRating = 1;
    public float preyRating = 1;
    
    public float hungerThresh = 50;
    public float thirstThresh = 50;
    public float mateThresh = 90;
    
    
    [Header("Static stats")]
    public float foodWorth = 2;
    public float maxMutatePercent = 2;
    public float forwardWanderBias = 2;
    public float ageIncrement = 0.5f;
    public float maxStat = 100;

    
    [Header("Set stats")]
    public float animalHeight;
    public float animalLength;
    
    [Header("Canvas")]
    public GameObject deathCanvas;

    [Header("Parents")]
    public AnimalBrain mother;
    public AnimalBrain father;
    
    [Header("Memory lists")]
    public List<GameObject> objSensedMemory = new List<GameObject>();
    public List<GameObject> forgettingObjs = new List<GameObject>();//List for objects that animal is no longer touching but are still in memory

    
    public float timeTillAdult= 60;




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
    }

    // Update is called once per frame
    void Update()
    {
        hunger = hunger - hungerDecrement * Time.deltaTime;// div 100 to keep it within normal numbers for testing
        thirst = thirst - thirstDecrement * Time.deltaTime;
        reproductiveUrge = reproductiveUrge + reproductiveIncrement * Time.deltaTime;
        age = age + ageIncrement * Time.deltaTime;
        if (hunger < 0 || thirst < 0 || age>100)
            health -= 0.01f;
        
        if (health <= 0 && this.GetComponent<BehaviourTree>().enabled)//if died and hasnt triggered already
        {
            Die();
        }
    }

    public void GiveBirth(AnimalBrain motherBrain, AnimalBrain fatherBrain)
    {
        GameObject baby = new GameObject("Gen2Holder");
        baby.transform.parent = transform.parent.parent;//the overall holder not the initialiser
        baby.transform.position = transform.position+(transform.forward*2);
        AnimalInitializer initializer = baby.AddComponent<AnimalInitializer>();
        initializer.animalDNA = animalBaseDNA;
        initializer.btTexts = transform.parent.GetComponent<AnimalInitializer>().btTexts;//copy current ones
        initializer.InitialiseAnimal();
        //todo optimise
        initializer.brain.mother = motherBrain;
        initializer.brain.father = fatherBrain;

    }

    public void Die()
    {
        Debug.Log("Died");
        if (this.GetComponent<BehaviourTree>().enabled == true)//Only trigger once
        {
            this.GetComponent<BehaviourTree>().enabled = false;//disable ai
            //this.GetComponent<Rigidbody>().freezeRotation = false;
            Instantiate(deathCanvas, this.transform.position, transform.rotation);
            animalHeight = -5;
            gameObject.GetComponent<AnimalGravity>().animalHeight = -5;//Collapse to ground

            StartCoroutine(SetInactive(10));
        }
    }

    IEnumerator SetInactive(float time)
    {
        yield return new WaitForSeconds(time);//wait specific time
        gameObject.GetComponent<Collider>().enabled = false;//fall through floor
        yield return new WaitForSeconds(1);

        gameObject.GetComponentInParent<AnimalInitializer>().gameObject.SetActive(false);
        gameObject.SetActive(false);
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
              //Set self as parent so it can have a mutation on first generation for more interest
              mother = GetComponent<AnimalBrain>();
              father = GetComponent<AnimalBrain>();
              MutateStats();
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
        reproductiveIncrement = Mathf.Clamp(((mother.reproductiveIncrement+father.reproductiveIncrement)/2)+Random.Range(-percentDif, percentDif), 0.1f, hungerDecrement);//Dont let reproduce before they get hungry
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
        thirst = Mathf.Clamp(((mother.thirst+father.thirst)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        percentDif = mateThresh * change;
        mateThresh = Mathf.Clamp(((mother.mateThresh+father.mateThresh)/2)+Random.Range(-percentDif, percentDif), 0.1f, 30000);
        

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
        wanderRadius = animalBaseDNA.wanderRadius;
        forwardWanderBias = animalBaseDNA.forwardWanderBias;
        maxMutatePercent = animalBaseDNA.maxMutatePercent;
        predatorRating = animalBaseDNA.predatorRating;
        preyRating = animalBaseDNA.preyRating;
        eatsPlants = animalBaseDNA.eatsPlants;

        attackDamage = animalBaseDNA.attackDamage;
        attackRate = animalBaseDNA.attackRate;
        litterSize = animalBaseDNA.litterSize;
        foodWorth = animalBaseDNA.foodWorth;
    }
}
