using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CreatureStats : MonoBehaviour
{
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
    public String foodTag = "Food";
    public float maxMutatePercent = 2;
    
    public TextMeshProUGUI taskText;
    public Slider healthSlider;
    public Slider hungerSlider;
    public Slider thirstSlider;
    public Slider reproSlider;
    public int gen = 1;
    
    
    
    public CreatureTaskManager creatureTaskManager;
    
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
        hunger = hunger - hungerDecrement/100;// div 100 to keep it within normal numbers for testing
        thirst = thirst - thirstDecrement/100;
        reproductiveUrge = reproductiveUrge + reproductiveIncrement/100;
        age = age + ageIncrement/100;
        
        if(taskText != null)
            taskText.text = creatureTaskManager.currentTask;//For displaying text
        
        if (health <= 0)
        {
            Die();
            if(healthSlider != null)
                healthSlider.value = 0;
        }

        //Add tasks to task list if stats are too low
        if (hunger <= 50)
        {
            if (creatureTaskManager.taskList.Contains("Eat") == false && creatureTaskManager.currentTask != "Eat")
            {
                creatureTaskManager.taskList.Add("Eat");
            }
            if (hunger <= 10)
            {
                health = health - healthStarveDecrement/100;
            }
        }
        if (thirst <= 30)//CHange it to a better priority system than else if
        {
            if (creatureTaskManager.taskList.Contains("Drink") == false && creatureTaskManager.currentTask != "Drink")
            {
                creatureTaskManager.taskList.Add("Drink");
            }
            if (thirst <= 10)
            {
                health = health - healthStarveDecrement/100;
            }
        }
        if (reproductiveUrge > 90)//CHange it to a better priority system than else if
        {
            if (creatureTaskManager.taskList.Contains("Mate") == false && creatureTaskManager.currentTask != "Mate")
            {
                creatureTaskManager.taskList.Add("Mate");
            }
        }
        if (age >= 100)
        {
            Debug.Log("Died of old age");
            Die();
        }
            
        //Update info sliders
        if (healthSlider != null)
        {
            healthSlider.value = health/maxHealth;
        }
        if (hungerSlider != null)
        {
            hungerSlider.value = hunger/maxStat;
        }
        if (thirstSlider != null)
        {
            thirstSlider.value = thirst/maxStat;
        }
        if (reproSlider != null)
        {
            reproSlider.value = reproductiveUrge/maxStat;
        }
    }

    public void Die()
    {
        Debug.Log("Died");
        Destroy(gameObject);//destroy after 20secs
        Destroy(creatureTaskManager.wanderPosObj);//destroy after 20secs
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
          creatureTaskManager.GetNewTask();
    }

    public void MutateStats()
    {
        float change = maxMutatePercent / 100;//get float as percent
        float percentDif = 0;

        percentDif = healthStarveDecrement * change;
        healthStarveDecrement = Random.Range(healthStarveDecrement-percentDif, healthStarveDecrement+percentDif);
        percentDif = hungerDecrement * change;
        hungerDecrement = Random.Range(hungerDecrement-percentDif, hungerDecrement+percentDif);
        percentDif = thirstDecrement * change;
        thirstDecrement = Random.Range(thirstDecrement-percentDif, thirstDecrement+percentDif);
        percentDif = reproductiveIncrement * change;
        reproductiveIncrement = Random.Range(reproductiveIncrement-percentDif, reproductiveIncrement+percentDif);
        percentDif = ageIncrement * change;
        ageIncrement = Random.Range(ageIncrement-percentDif, ageIncrement+percentDif);
        percentDif = memoryLossRate * change;
        memoryLossRate = Random.Range(memoryLossRate-percentDif, memoryLossRate+percentDif);
        percentDif = sensoryRange * change;
        sensoryRange = Random.Range(sensoryRange-percentDif, sensoryRange+percentDif);
        percentDif = healthStarveDecrement * change;
        healthStarveDecrement = Random.Range(maxHealth-percentDif, maxHealth+percentDif);
        percentDif = moveSpeed * change;
        moveSpeed = Random.Range(moveSpeed-percentDif, moveSpeed+percentDif);
        percentDif = rotSpeed * change;
        rotSpeed = Random.Range(rotSpeed-percentDif, rotSpeed+percentDif);
        percentDif = wanderRadius * change;
        wanderRadius = Random.Range(wanderRadius-percentDif, wanderRadius+percentDif);
        percentDif = forwardWanderBias * change;
        forwardWanderBias = Random.Range(forwardWanderBias-percentDif, forwardWanderBias+percentDif);
    }

    
    //todo
    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
        Debug.Log("Invis");
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.GetComponent<Renderer>().enabled = false;
        }
    }
    
    private void OnBecameVisible()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.GetComponent<Renderer>().enabled = true;
        }
    }
}
