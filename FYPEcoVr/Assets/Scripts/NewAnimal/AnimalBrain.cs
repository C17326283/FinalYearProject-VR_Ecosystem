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
    
    public List<GameObject> objSensedMemory = new List<GameObject>();
    public List<GameObject> forgettingObjs = new List<GameObject>();//List for objects that animal is no longer touching but are still in memory

    public List<String> toHunt = new List<String>();
    public List<String> huntedBy = new List<String>();
    
    //public Transform currentTarget;//set from behaviour tree so positioner can use

    public String name = "name";
    public float health = 100;
    public float hunger = 100;
    public float thirst = 100;
    public float reproductiveUrge = 0;
    public float age = 0;
    public float predatorRating = 1;
    public float preyRating = 1;
    public bool eatsPlants = false;
    
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
    public float attackRate = 1;
    public float attackDamage = 5;
    
    public float litterSize = 1;
    public float foodWorth = 2;

    public float animalHeight;
    public float animalLength;
    public GameObject deathCanvas;

    public float timeTillAdult= 60;
    public Vector3 defaultScale;

    public AnimalBrain mother;
    public AnimalBrain father;

    public float hungerThresh = 50;
    public float thirstThresh = 50;
    public float mateThresh = 90;


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
        initializer.clip = transform.parent.GetComponent<AnimalInitializer>().clip;
        initializer.InitialiseAnimal();
        //todo optimise
        initializer.brain.mother = motherBrain;
        initializer.brain.father = fatherBrain;

    }

    public void Die()
    {
        Debug.Log("Died");
        this.GetComponent<BehaviourTree>().enabled = false;//disable ai
        //this.GetComponent<Rigidbody>().freezeRotation = false;
        Instantiate(deathCanvas, this.transform.position, transform.rotation);
        animalHeight = -5;
        //gameObject.GetComponent<AnimalGravity>().animalHeight = -5;//Collapse to ground


        StartCoroutine(SetInactive(5));
        //Destroy(gameObject);//destroy after 20secs
    }

    IEnumerator SetInactive(float time)
    {
        yield return new WaitForSeconds(time);//wait specific time
        gameObject.GetComponent<Collider>().enabled = false;//fall through floor
        yield return new WaitForSeconds(1);

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
          //If has parents then mutate instead of doing default 
          if (mother != null)
          {
              MutateStats();
          }
          else
          {
//              Debug.Log("does not have parents on mate");
          }
    }
    

    public void MutateStats()
    {
        float change = maxMutatePercent / 100;//get float as percent
        float percentDif = 1;

        
        percentDif = mother.healthStarveDecrement * change;
//        print("change"+change+" ,percentDif:"+percentDif+" ,mother.healthStarveDecrement:"+mother.healthStarveDecrement+" ,par/2:"+(mother.healthStarveDecrement+father.healthStarveDecrement)/2+",ran:"+Mathf.Clamp(Random.Range(-percentDif, percentDif), -30000, 30000));
        //Get average from parents and add a mutation
        healthStarveDecrement = Mathf.Clamp(((mother.healthStarveDecrement+father.healthStarveDecrement)/2)+Random.Range(-percentDif, percentDif), -30000, 30000);
        percentDif = hungerDecrement * change;
        hungerDecrement = Mathf.Clamp(((mother.hungerDecrement+father.hungerDecrement)/2)+Random.Range(-percentDif, percentDif), -30000, 30000);
        percentDif = thirstDecrement * change;
        thirstDecrement = Mathf.Clamp(((mother.thirstDecrement+father.thirstDecrement)/2)+Random.Range(-percentDif, percentDif), -30000, 30000);
        percentDif = reproductiveIncrement * change;
        reproductiveIncrement = Mathf.Clamp(((mother.reproductiveIncrement+father.reproductiveIncrement)/2)+Random.Range(-percentDif, percentDif), -30000, hungerDecrement);//Dont let reproduce before they get hungry
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
        predatorRating = animalBaseDNA.predatorRating;
        preyRating = animalBaseDNA.preyRating;
        eatsPlants = animalBaseDNA.eatsPlants;

        attackDamage = animalBaseDNA.attackDamage;
        attackRate = animalBaseDNA.attackRate;
        litterSize = animalBaseDNA.litterSize;
        foodWorth = animalBaseDNA.foodWorth;




//        print("SetStatsFromDNA"+animalBaseDNA.moveSpeed);
//        print("moveSpeed"+moveSpeed);
    }
}
