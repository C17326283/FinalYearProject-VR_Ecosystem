using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CreatureStats : MonoBehaviour
{
    public float health = 100;
    public float hunger = 100;
    public float thirst = 100;
    public float reproductiveUrge = 100;
    public float maxHealth = 100f;
    public float maxStat = 100;
    public float healthStarveDecrement = 0.01f;
    public float hungerDecrement = 0.02f;
    public float thirstDecrement = 0.01f;
    public float reproductiveIncrement = 0.01f;
    public TextMeshProUGUI taskText;
    public Slider healthSlider;
    public Slider hungerSlider;
    public Slider thirstSlider;
    public Slider reproSlider;
    
    
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
        hunger = hunger - hungerDecrement;
        thirst = thirst - thirstDecrement;
        reproductiveUrge = reproductiveUrge + reproductiveIncrement;
        
        if(taskText != null)
            taskText.text = creatureTaskManager.currentTask;//For displaying text
        
        if (health <= 0)
        {
            healthSlider.value = 0;
            Die();
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
                health = health - healthStarveDecrement;
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
                health = health - healthStarveDecrement;
            }
        }
        if (reproductiveUrge > 90)//CHange it to a better priority system than else if
        {
            if (creatureTaskManager.taskList.Contains("Mate") == false && creatureTaskManager.currentTask != "Mate")
            {
                creatureTaskManager.taskList.Add("Mate");
            }
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
        Destroy(gameObject,1);//destroy after 20secs
    }
}
