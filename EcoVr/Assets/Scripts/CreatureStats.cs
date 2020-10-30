using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CreatureStats : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100f;
    public Slider healthSlider;
    
    public float hunger = 100;
    public Slider hungerSlider;
    public float hungerDecrement = 0.01f;
    
    public float maxStat;
    public CreatureTaskManager creatureTaskManager;
    
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        hunger = hunger - hungerDecrement;
        
        if (healthSlider != null)
        {
            if (health <= 0)
            {
                healthSlider.value = 0;
                Die();
            }

            healthSlider.value = health/maxHealth;
        }
        
        if (hungerSlider != null)
        {
            if (hunger <= 50)
            {
                creatureTaskManager.nextTask = "Eat";
                if (hunger <= 10)
                {
                    health = health - hungerDecrement;
                }
                
            }
            

            hungerSlider.value = hunger/maxStat;
        }
    }

    public void Die()
    {
        Destroy(gameObject,20);//destroy after 20secs
    }
}
