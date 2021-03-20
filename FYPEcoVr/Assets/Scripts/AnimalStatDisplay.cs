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

    
    
    // Start is called before the first frame update
    void Start()
    {
        statCanvas = GameObject.Find("StatCanvas");
        canvasManager = statCanvas.GetComponentInChildren<StatCanvasManager>();
    }

    public void SetToPos()
    {
        RaycastHit hit;
        //Only add if theres environment below
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2000))
        {
            if (hit.transform.GetComponent<AnimalBrain>() != null)
            {
                print(hit.transform.name);
                statCanvas.transform.parent = hit.transform;
                statCanvas.transform.position = hit.transform.position + (hit.transform.transform.up);
                statCanvas.transform.rotation = hit.transform.rotation;
                
                selectAnimalBrain = hit.transform.GetComponent<AnimalBrain>();
                selectAnimalBehaviours = hit.transform.GetComponent<AnimalBehaviours>();
                statCanvas.transform.position = selectAnimalBrain.transform.position + (selectAnimalBrain.transform.transform.up*selectAnimalBrain.animalHeight);
            }
        }
    }

    public void Update()
    {
        if (selectAnimalBrain != null && selectAnimalBrain.gameObject.activeInHierarchy)
        {
            SetValues();
            SetColours();
        }
    }

    public void SetValues()
    {
        canvasManager.health.text = "Health: " + Mathf.Clamp(Mathf.RoundToInt(selectAnimalBrain.health),0,selectAnimalBrain.maxHealth);
        canvasManager.hunger.text = "Hunger: " + Mathf.Clamp(Mathf.RoundToInt(selectAnimalBrain.hunger),0,selectAnimalBrain.maxStat);
        canvasManager.thirst.text = "Thirst: " + Mathf.Clamp(Mathf.RoundToInt(selectAnimalBrain.thirst),0,selectAnimalBrain.maxStat);
        canvasManager.urge.text = "Mating urge: " + Mathf.Clamp(Mathf.RoundToInt(selectAnimalBrain.reproductiveUrge),0,selectAnimalBrain.maxStat);
        canvasManager.task.text = "Task: " + selectAnimalBehaviours.currentTask;
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
