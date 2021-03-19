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
    public Color32 negaticeColour;

    
    
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
        if (Physics.Raycast(transform.position, transform.forward, out hit, 300))
        {
            //print(hit.transform.name);
            statCanvas.transform.parent = hit.transform;
            statCanvas.transform.position = hit.transform.position + (hit.transform.transform.up);
            statCanvas.transform.rotation = hit.transform.rotation;
            
            if (hit.transform.GetComponent<AnimalBrain>() != null)
            {
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
        canvasManager.health.text = "Health: " + Math.Round(selectAnimalBrain.health,2);
        canvasManager.hunger.text = "Hunger: " + Math.Round(selectAnimalBrain.hunger,2);
        canvasManager.thirst.text = "Thirst: " + Math.Round(selectAnimalBrain.thirst,2);
        canvasManager.urge.text = "Mating urge: " + Math.Round(selectAnimalBrain.reproductiveUrge,2);
        canvasManager.task.text = "Task: " + selectAnimalBehaviours.currentTask;
    }

    public void SetColours()
    {
        if (selectAnimalBrain.health > 50)
        {
            canvasManager.health.color = positiveColour;
        }
        else
        {
            canvasManager.health.color = negaticeColour;
        }
        
        if (selectAnimalBrain.hunger > selectAnimalBrain.hungerThresh)
        {
            canvasManager.hunger.color = positiveColour;
        }
        else
        {
            canvasManager.hunger.color = negaticeColour;
        }
        
        if (selectAnimalBrain.thirst >  selectAnimalBrain.thirstThresh)
        {
            canvasManager.thirst.color = positiveColour;
        }
        else
        {
            canvasManager.thirst.color = negaticeColour;
        }
        
        if (selectAnimalBrain.reproductiveUrge >  selectAnimalBrain.mateThresh)
        {
            canvasManager.urge.color = positiveColour;
        }
        else
        {
            canvasManager.urge.color = Color.white;
        }
    }

    /*
    public void SetToPos()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            Debug.Log("hit:"+hit.transform.name);
        }
        
        GameObject animal = GetComponent<XRRayInteractor>().raycastTriggerInteraction;

        statCanvas.transform.parent = animal.transform;
        statCanvas.transform.position = animal.transform.position + (animal.transform.transform.up);
    }
*/
}
