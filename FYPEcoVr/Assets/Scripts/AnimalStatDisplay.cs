using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AnimalStatDisplay : MonoBehaviour
{

    //public Transform animalToDisplay;
    public GameObject statCanvas;
    public StatCanvasManager canvasManager;
    public AnimalBrain selectAnimalBrain;
    public AnimalBehaviours selectAnimalBehaviours;

    public Color32 positiveColour;
    public Color32 negativeColour;

    public bool isRefreshing = false;



    // Start is called before the first frame update
    void Start()
    {
        statCanvas = GameObject.Find("StatCanvas");
        if(canvasManager==null)
            canvasManager = statCanvas.GetComponentInChildren<StatCanvasManager>();
    }

    public void SetToPos()
    {
        RaycastHit hit;
        //Only add if theres environment below
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            if (hit.transform.GetComponent<AnimalBrain>() != null)
            {
                print(hit.transform.name);
                statCanvas.transform.SetParent(hit.transform);
                statCanvas.transform.rotation = hit.transform.rotation;

                selectAnimalBrain = hit.transform.GetComponent<AnimalBrain>();
                selectAnimalBehaviours = hit.transform.GetComponent<AnimalBehaviours>();
                Transform anTransform = selectAnimalBrain.transform;
                statCanvas.transform.position = anTransform.position +
                                                (anTransform.transform.up * (selectAnimalBrain.animalHeight/2));
                SetDNA();
                SetValues();
                SetColours();
                if (!isRefreshing)
                {
                    StartCoroutine(UpdateStatScreen());
                }
            }
            else
            {
                print("Controller hit non animal: " + hit.transform.name);
            }
        }
    }

    IEnumerator UpdateStatScreen()
    {
        while (true)
        {
            if (selectAnimalBrain != null && selectAnimalBrain.gameObject.activeInHierarchy)
            {
                SetValues();
                SetColours();
            }
            yield return new WaitForSeconds(1);
        }
    }


    public void SetValues()
    {
        canvasManager.health.text = "Health: " +
                                    Mathf.Clamp(Mathf.RoundToInt(selectAnimalBrain.health), 0,
                                        selectAnimalBrain.maxHealth);
        canvasManager.hunger.text = "Nutrition: " +
                                    Mathf.Clamp(Mathf.RoundToInt(selectAnimalBrain.hunger), 0,
                                        selectAnimalBrain.maxStat);
        canvasManager.thirst.text = "Hydration: " +
                                    Mathf.Clamp(Mathf.RoundToInt(selectAnimalBrain.thirst), 0,
                                        selectAnimalBrain.maxStat);
        canvasManager.urge.text = "Mating urge: " + Mathf.Clamp(Mathf.RoundToInt(selectAnimalBrain.reproductiveUrge), 0,
            selectAnimalBrain.maxStat);
        canvasManager.age.text = "Age: " + Mathf.Clamp(Mathf.RoundToInt(selectAnimalBrain.age), 0,
            selectAnimalBrain.maxStat);
        canvasManager.task.text = "Task: " + selectAnimalBehaviours.currentTask;
    }

    public void SetDNA()
    {
        canvasManager.animalname.text = (selectAnimalBrain.name);
        canvasManager.generation.text = ("Generation: " + selectAnimalBrain.generation);
        canvasManager.maxHealth.text = ("Max Health: " + Math.Round(selectAnimalBrain.maxHealth,1) + " (" + Math.Round(selectAnimalBrain.maxHealth - selectAnimalBrain.animalBaseDNA.maxHealth,1)+")");
        canvasManager.hungerSpeed.text = ("Hunger Speed: " + Math.Round(selectAnimalBrain.hungerDecrement,1) + " ("+ Math.Round(selectAnimalBrain.hungerDecrement-selectAnimalBrain.animalBaseDNA.hungerDecrement,1)+")");
        canvasManager.thirstSpeed.text = ("Thirst Speed: " + Math.Round(selectAnimalBrain.thirstDecrement,1) + " (" + Math.Round(selectAnimalBrain.thirstDecrement - selectAnimalBrain.animalBaseDNA.thirstDecrement,1)+")");
        canvasManager.urgeSpeed.text = ("Mating urge Speed: " + Math.Round(selectAnimalBrain.reproductiveIncrement,1) + " (" + Math.Round(selectAnimalBrain.reproductiveIncrement - selectAnimalBrain.animalBaseDNA.reproductiveIncrement,1)+")");
        canvasManager.memoryLossRate.text = ("Memory Loss Rate: " + Math.Round(selectAnimalBrain.memoryLossRate,1) + " (" + Math.Round(selectAnimalBrain.memoryLossRate - selectAnimalBrain.animalBaseDNA.memoryLossRate,1)+")");
        canvasManager.sensoryRange.text = ("Sensory Range: " + Math.Round(selectAnimalBrain.sensoryRange,1) + " (" + Math.Round(selectAnimalBrain.sensoryRange - selectAnimalBrain.animalBaseDNA.sensoryRange,1)+")");
        canvasManager.moveSpeed.text = ("Move Speed: " + Math.Round(selectAnimalBrain.moveSpeed,1) + " (" + Math.Round(selectAnimalBrain.moveSpeed - selectAnimalBrain.animalBaseDNA.moveSpeed,1)+")");
        canvasManager.attackRate.text = ("Attack Rate: " + Math.Round(selectAnimalBrain.attackRate,1) + " (" + Math.Round(selectAnimalBrain.attackRate - selectAnimalBrain.animalBaseDNA.attackRate,1)+")");
        canvasManager.attackDamage.text = ("Attack Damage: " + Math.Round(selectAnimalBrain.attackDamage,1) + " (" + Math.Round(selectAnimalBrain.attackDamage - selectAnimalBrain.animalBaseDNA.attackDamage,1)+")");
        canvasManager.predatorRating.text = ("Predator Rating: " + Math.Round(selectAnimalBrain.predatorRating,1) + " (" + Math.Round(selectAnimalBrain.predatorRating - selectAnimalBrain.animalBaseDNA.predatorRating,1)+")");
        canvasManager.preyRating.text = ("Prey Rating: " + Math.Round(selectAnimalBrain.preyRating,1) + " (" + Math.Round(selectAnimalBrain.preyRating - selectAnimalBrain.animalBaseDNA.preyRating,1)+")");
        canvasManager.litterSize.text = ("Litter Size: " + Math.Round(selectAnimalBrain.litterSize,1) + " (" + Math.Round(selectAnimalBrain.litterSize - selectAnimalBrain.animalBaseDNA.litterSize,1)+")");
        canvasManager.deathAge.text = ("Lifespan: " + Math.Round(selectAnimalBrain.deathAge,1) + " (" + Math.Round(selectAnimalBrain.deathAge - selectAnimalBrain.animalBaseDNA.deathAge,1)+")");
        canvasManager.eatsPlants.text = ("Eats Plants: " + selectAnimalBrain.eatsPlants + " (" + selectAnimalBrain.animalBaseDNA.eatsPlants+")");
        canvasManager.eatsMeat.text = ("Eats Meat: " + selectAnimalBrain.eatsMeat + " (" + selectAnimalBrain.animalBaseDNA.eatsMeat+")");
        canvasManager.maxMutate.text = ("Max amount to mutate: " + Math.Round(selectAnimalBrain.maxMutatePercent,1)+"%");

        if (selectAnimalBrain.genderIsMale)
            canvasManager.gender.text = ("Gender: Male");
        else
            canvasManager.gender.text = ("Gender: Female");
        
    }

public void SetColours()
    {
        if (selectAnimalBrain.health >= 50)
        {
            canvasManager.health.color = positiveColour;
        }
        else
        {
            canvasManager.health.color = negativeColour;
        }
        
        
        if (selectAnimalBrain.hunger >= selectAnimalBrain.hungerThresh)
        {
            canvasManager.hunger.color = positiveColour;
        }
        else
        {
            canvasManager.hunger.color = negativeColour;
        }
        
        if (selectAnimalBrain.thirst >=  selectAnimalBrain.thirstThresh)
        {
            canvasManager.thirst.color = positiveColour;
        }
        else
        {
            canvasManager.thirst.color = negativeColour;
        }
        
        if (selectAnimalBrain.age < selectAnimalBrain.deathAge/2)
        {
            canvasManager.age.color = positiveColour;
        }
        else
        {
            canvasManager.age.color = negativeColour;
        }
        
        if (selectAnimalBrain.reproductiveUrge >=  selectAnimalBrain.mateThresh)
        {
            canvasManager.urge.color = positiveColour;
        }
        else
        {
            canvasManager.urge.color = Color.white;
        }
    }

}
