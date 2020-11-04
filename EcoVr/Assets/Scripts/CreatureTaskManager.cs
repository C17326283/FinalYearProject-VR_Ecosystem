using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatureTaskManager : MonoBehaviour
{
    public CreatureStats creatureStats;
    
    public String currentTask = null;//For tracking in game what a creature is doing
    //public String nextTask = null;//For tracking what creature will do after current task

    public GameObject target = null;
    public Vector3 targetPos;//pos of current task target
    
    public float moveSpeed = 5;
    public float rotSpeed = 50;
    public float wanderRadius = 5;
    public float forwardWanderBias;

    private GameObject wanderPosObj;
    private List<GameObject> allPossibleMates;
    public List<String> taskList = new List<String>();
    

    private void OnEnable()
    {
        if (wanderPosObj == null)
        {
            wanderPosObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            wanderPosObj.transform.localScale = new Vector3(.2f, .2f, .2f);
        }
        GetNewTask();
    }

    void Update()
    {
        //Get target if there is none else move towards target and perform task if reached it
        if(target == null || currentTask == null || (currentTask == "Wander" && taskList.Count > 1))
            GetNewTask();
        else
        {
            //todo add a time out that adds task back to list if cant reach target anymore
            targetPos = target.transform.position;//Need a vector3 form to access the single axis?
            targetPos.y = transform.position.y;//Cancel y so it only moves on x and z and wander target obj stays at same y

            if (Vector3.Distance(this.transform.position, targetPos) > 1f)//If over a certain distance then keep moving towards it
            {
                //Lerp rotation
                Vector3 relativePos = targetPos - transform.position;
                Quaternion toRotation = Quaternion.LookRotation(relativePos);
                transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, rotSpeed * Time.deltaTime );
            
                //Move Towards
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed*Time.deltaTime);
            }
            else//Have reached target
            {
//                Debug.Log("reached target, perform action");
                TryPerformCurrentTask();
            }
        }
    }

    void TryPerformCurrentTask()
    {
        if (currentTask == "Eat")
        {
            creatureStats.hunger = 100;
            currentTask = null;
        }
        else if (currentTask == "Drink")
        {
            creatureStats.thirst = 100;
            currentTask = null;
        }
        else if (currentTask == "Mate")
        {
            Debug.Log("making mini mes"+this.gameObject.name+creatureStats.reproductiveUrge);
            creatureStats.reproductiveUrge = 0;
            currentTask = null;
            target = null;
            if(this.gameObject.transform.parent != null)
                Instantiate(this.gameObject.transform.parent);//check if they have a parent
            else
                Instantiate(this.gameObject.transform);
        }
        else//
        {
            GetNewTask();
        }
    }

    void GetNewTask()
    {
        //Check that their is a possible target for the next task and set currenttask to that else try the next task in list
        foreach (String possibleTask in taskList)
        {
            //have a list of tasks and it checks each else wander
            if (possibleTask == "Eat" && currentTask!= "Eat" && getClosestTargetWithTag("Food") != null)//todo am calling getClosest function twice which is expensive so need alternative
            {
                target = getClosestTargetWithTag("Food");
//                Debug.Log("Eat target: "+target);
                //Remove from list and set current task to it
                currentTask = possibleTask;
                taskList.Remove(possibleTask);
                return;
            }
            else if (possibleTask == "Drink" && currentTask!= "Drink" && getClosestTargetWithTag("Water") != null)
            {
                target = getClosestTargetWithTag("Water");
//                Debug.Log("Water target: "+target);
                //Remove from list and set current task to it
                currentTask = possibleTask;
                taskList.Remove(possibleTask);
                return;
            }
            else if (possibleTask == "Mate" && currentTask!= "Mate" && getClosestTargetWithTag(this.tag) != null)
            {
                target = getClosestTargetWithTag(this.tag);
//                Debug.Log("Mate target: "+target);
                //Remove from list and set current task to it
                currentTask = possibleTask;
                taskList.Remove(possibleTask);
                return;
            }
        }
        //This only runs if a target couldnt be found for any of the other tasks
        currentTask = "Wander";
        Vector3 thisPos = transform.position;
        //This is just a temp solution but it uses a test sphere to make a random target position to travle to
        wanderPosObj.transform.position =  new Vector3(Random.Range(thisPos.x-wanderRadius, thisPos.x+wanderRadius), thisPos.y, Random.Range(thisPos.z-wanderRadius, thisPos.x+wanderRadius))+(transform.forward*forwardWanderBias);//add forward bias at end
        target = wanderPosObj;
        
    }

    GameObject getClosestTargetWithTag(String tag)
    {
        target = null;
        
        //Make an array of animals can mate with and fidn the closest one
        GameObject[] allPossibleObjs = GameObject.FindGameObjectsWithTag(tag);
        float closestPathDistance = 99999;
        GameObject closestObj = null;
        foreach (GameObject possibleObj in allPossibleObjs)
        {
            //Check that there is an obj, that its not this obj and it is the closest distance //todo have a distance the animal can see
            if (possibleObj != this.gameObject && closestPathDistance > Vector3.Distance(transform.position, possibleObj.transform.position))
            {
//                Debug.Log("found Obj"+possibleObj.transform.name);
                closestObj = possibleObj;
                closestPathDistance = Vector3.Distance(transform.position, possibleObj.transform.position);
            }
        }

        if (closestPathDistance == 99999) //If closest didnt change then cancel and get new task
        {
            return null;
        }
        else
        {
//            Debug.Log("return closestObj");
            return closestObj;
        }


    }
}
