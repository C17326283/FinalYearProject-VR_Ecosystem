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
    
    public GameObject wanderPosObj;
    private List<GameObject> allPossibleMates;
    public List<String> taskList = new List<String>();

    public GameObject sensorySphere;
    public List<GameObject> objSensedMemory = new List<GameObject>();
    
    private int numOfTasks = 0;
    

    private void OnEnable()
    {
        wanderPosObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        wanderPosObj.transform.name = transform.name+" WanderPoint";
        wanderPosObj.transform.localScale = new Vector3(.2f, .2f, .2f);
        
        //Make a sensory range collider fo adding things to memory
        sensorySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sensorySphere.GetComponent<Renderer>().enabled = false;
        sensorySphere.transform.name = "SenseSphere";
        sensorySphere.GetComponent<SphereCollider>().radius = creatureStats.sensoryRange * 2;
        sensorySphere.GetComponent<Collider>().isTrigger = true;
        sensorySphere.transform.parent = this.gameObject.transform;
        sensorySphere.transform.position = this.transform.position;
        Rigidbody rb = sensorySphere.AddComponent<Rigidbody>();//Needs kinematic to register collisions
        rb.useGravity = false;
        rb.isKinematic = true;
        SensoryScript sense = sensorySphere.AddComponent<SensoryScript>();
        sense.creatureTaskManager = this;
        GetNewTask();
    }

    void Update()
    {
        //Check if task list has increased since last frame as their may be a higher priority than wander.
        if (taskList.Count > numOfTasks && currentTask == "Wander")
        {
//            Debug.Log("numOfTasks"+numOfTasks);
            GetNewTask();
        }
        numOfTasks = taskList.Count;
        
        //Get target if there is none else move towards target and perform task if reached it
        if (target == null || currentTask == null) //if theres no target then get one
        {
            GetNewTask();
        }
        else//move toward location
        {
            //todo add a time out that adds task back to list if cant reach target anymore
            targetPos = target.transform.position;//Need a vector3 form to access the single axis?
            targetPos.y = transform.position.y;//Cancel y so it only moves on x and z and wander target obj stays at same y

            if (Vector3.Distance(this.transform.position, targetPos) > 1f)//If over a certain distance then keep moving towards it
            {
                //Lerp rotation
                Vector3 relativePos = targetPos - transform.position;
                Quaternion toRotation = Quaternion.LookRotation(relativePos);
                transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, creatureStats.rotSpeed * Time.deltaTime );
            
                //Move Towards
                transform.position = Vector3.MoveTowards(transform.position, targetPos, creatureStats.moveSpeed*Time.deltaTime);
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
            if (target.GetComponent<CreatureStats>() != null)
            {
                target.GetComponent<CreatureStats>().Die();
                Debug.Log("chopped and killed");
            }
        }
        else if (currentTask == "Drink")
        {
            creatureStats.thirst = 100;
            currentTask = null;
        }
        else if (currentTask == "Mate")
        {
//            Debug.Log("making mini me: "+this.gameObject.name+creatureStats.reproductiveUrge);
            creatureStats.reproductiveUrge = 0;
            currentTask = null;
            target = null;
            GameObject baby;
            if(this.gameObject.transform.parent != null)
                baby = Instantiate(this.gameObject.transform.parent.gameObject);//check if they have a parent
            else
                baby = Instantiate(this.gameObject);
            baby.transform.name = this.transform.name + " Gen"+(creatureStats.gen+1);//Display generations for testing
            baby.GetComponent<CreatureStats>().gen = creatureStats.gen + 1;
            baby.GetComponent<CreatureStats>().Born();
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
            //if they are currently trying to eat or their next possible task is eat then eat//Allows for overwriting wandering 
            if ((possibleTask == "Eat" || currentTask == "Eat") && getClosestTargetWithTag(creatureStats.foodTag) != null)//todo am calling getClosest function twice which is expensive so need alternative
            {
                target = getClosestTargetWithTag(creatureStats.foodTag);
//                Debug.Log("Eat target: "+target);
                //Remove from list and set current task to it
                currentTask = possibleTask;
                taskList.Remove(possibleTask);
                
                return;
            }
            else if ((possibleTask == "Drink" || currentTask == "Drink") && getClosestTargetWithTag("Water") != null)
            {
                target = getClosestTargetWithTag("Water");
//                Debug.Log("Water target: "+target);
                //Remove from list and set current task to it
                currentTask = possibleTask;
                taskList.Remove(possibleTask);
                return;
            }
            else if ((possibleTask == "Mate" || currentTask == "Mate") && getClosestTargetWithTag(this.transform.tag) != null)
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
        wanderPosObj.transform.position =  thisPos + new Vector3(Random.Range(-creatureStats.wanderRadius, creatureStats.wanderRadius), thisPos.y, Random.Range(-creatureStats.wanderRadius, creatureStats.wanderRadius))+(transform.forward*creatureStats.forwardWanderBias);//add forward bias at end  +(transform.forward*forwardWanderBias)
        target = wanderPosObj;
        
    }

    //Checks all items in memory and returns the closest one//Better than searching by all with tag
    GameObject getClosestTargetWithTag(String tag)
    {
        target = null;
        
        //Make an array of animals can mate with and fidn the closest one
        
        //GameObject[] allPossibleObjs = GameObject.FindGameObjectsWithTag(tag);
        float closestPathDistance = 99999;
        GameObject closestObj = null;
        foreach (GameObject possibleObj in objSensedMemory)
        {
            //check object still exists
            if(possibleObj != null && possibleObj.activeInHierarchy)
            {
                //Check obj has tag, that its not this obj and it is the closest distance //todo have a distance the animal can see
                if (possibleObj.transform.CompareTag(tag) && possibleObj != this.gameObject && closestPathDistance > Vector3.Distance(transform.position, possibleObj.transform.position))
                {
//                Debug.Log("found Obj"+possibleObj.transform.name);
                    closestObj = possibleObj;
                    closestPathDistance = Vector3.Distance(transform.position, possibleObj.transform.position);
                }
            }
            else
            {
                objSensedMemory.Remove(possibleObj);
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
