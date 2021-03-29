using System;
using System.Collections;
using System.Collections.Generic;
using Panda;
using UnityEngine;
using Random = UnityEngine.Random;

//For managing the actual functions of animals life such as birth and death
public class AnimalLife : MonoBehaviour
{
    public BiomeManager biomeManager;
    public String debuff;

    public AnimalBrain brain;
    public GameObject core;

    public float timeToCheckBiome = 2;

    [Header("Canvas")]
    public ObjectPool deathCanvasPool;
    
    private bool hasDied = false;
    public AnimalDistanceDisabler distDisabler;
    public AnimalBehaviours behaviours;

    
    // Start is called before the first frame update
    void Start()
    {
        biomeManager = core.GetComponent<BiomeManager>();
        deathCanvasPool = GameObject.Find("DeathCanvasPool").GetComponent<ObjectPool>();
    }
    
    void Update()
    {
        if (transform.parent.gameObject.activeInHierarchy) //only update and check if animal manager active
        {
            if(debuff=="Cold")//Get hungry faster in winter biome
                brain.hunger = brain.hunger - (brain.hungerDecrement*1.5f) * Time.deltaTime; // div 100 to keep it within normal numbers for testing
            else
                brain.hunger = brain.hunger - brain.hungerDecrement * Time.deltaTime; // div 100 to keep it within normal numbers for testing
            
            if(debuff=="Hot")//Get hungry faster in desert biome
                brain.thirst = brain.thirst - (brain.thirstDecrement*1.5f) * Time.deltaTime;
            else
                brain.thirst = brain.thirst - brain.thirstDecrement * Time.deltaTime;

            brain.age +=brain.ageIncrement * Time.deltaTime;
            if (brain.hunger < 0 || brain.thirst < 0)
                brain.health -= 0.01f;


            //Dont immediately reproduce
            if (brain.age > brain.deathAge / 10 && behaviours.hasEaten)
                brain.reproductiveUrge = brain.reproductiveUrge + brain.reproductiveIncrement * Time.deltaTime;

            if ((brain.health <= 0 || brain.age > brain.deathAge) && !hasDied) //if died and hasnt triggered already
            {
                brain.health = -1;
                Die();
            }
        }
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
    
    public void GiveBirth(AnimalBrain motherBrain, AnimalBrain fatherBrain)
    {
        GameObject babyHolder = new GameObject(motherBrain.name+",Gen:"+motherBrain.generation+1);
        Transform transf = transform;
        babyHolder.transform.parent = transf.parent.parent;//the overall holder not the initialiser
        babyHolder.transform.localRotation = Quaternion.Euler(0, Random.Range(-180,180), 0);
        babyHolder.transform.position = transf.position+(-transf.forward*brain.animalLength);
        AnimalInitializer initializer = babyHolder.AddComponent<AnimalInitializer>();
        initializer.animalDNA = brain.animalBaseDNA;
        initializer.btTexts = transform.parent.GetComponent<AnimalInitializer>().btTexts;//copy current ones
        initializer.InitialiseAnimal();
        //todo optimise
        initializer.brain.mother = motherBrain;
        initializer.brain.father = fatherBrain;
        

    }

    public void Die()
    {
        hasDied = true;
        this.GetComponent<BehaviourTree>().enabled = false;//disable ai
        //this.GetComponent<Rigidbody>().freezeRotation = false;
        GameObject deathCanvas = deathCanvasPool.GetObj();
        deathCanvas.transform.position = this.transform.position;
        brain.animalHeight = -5;
        transform.right = transform.up;//Lay on side
        gameObject.GetComponent<AnimalGravity>().animalHeight = -5;//Collapse to ground
        gameObject.GetComponentInChildren<HeadLook>().enabled = false;
        gameObject.GetComponentInChildren<AnimalAudioManager>().enabled = false;
        

        StartCoroutine(SetInactive(30));//Maybe destroy or pool on death instead
    }

    IEnumerator SetInactive(float time)
    {
        yield return new WaitForSeconds(time);//wait specific time
        gameObject.GetComponent<Collider>().enabled = false;//fall through floor
        brain.foodWorth = -1;//so it wont be targeted anymore
        yield return new WaitForSeconds(1);

        gameObject.GetComponentInParent<AnimalInitializer>().gameObject.SetActive(false);
        distDisabler.CancelCheck();
        distDisabler.enabled = false;
        gameObject.SetActive(false);
    }
    

}
