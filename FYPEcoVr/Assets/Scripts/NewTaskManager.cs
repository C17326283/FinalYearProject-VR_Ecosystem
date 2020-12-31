using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewTaskManager : MonoBehaviour
{
    public AnimalProfile animalProfile;
    
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
    private int layerMask;
    public float targetDistToTarget = 2;
    
    public List<FootRaycastPositioner> footPositioners = new List<FootRaycastPositioner>();
    


    private void Start()
    {
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
    }

    private void OnEnable()
    {
        wanderPosObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        wanderPosObj.transform.name = transform.name+" WanderPoint";
        wanderPosObj.transform.position = transform.position + new Vector3(.2f, .2f, .2f);
        wanderPosObj.transform.localScale = new Vector3(.2f, .2f, .2f);
        
        //Make a sensory range collider fo adding things to memory
        if (sensorySphere == null)
        {
            sensorySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sensorySphere.GetComponent<Renderer>().enabled = false;
            sensorySphere.transform.name = "SenseSphere";
            sensorySphere.GetComponent<SphereCollider>().radius = animalProfile.sensoryRange * 2;
            sensorySphere.GetComponent<Collider>().isTrigger = true;
            sensorySphere.transform.parent = this.gameObject.transform;
            sensorySphere.transform.position = this.transform.position;
            sensorySphere.AddComponent<SensoryScript>();
            Rigidbody rb = sensorySphere.AddComponent<Rigidbody>();//Needs kinematic to register collisions
            rb.useGravity = false;
            rb.isKinematic = true;
            GetNewTask(); 
        }
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
        else//move toward location//todo move this to new script
        {
            //todo add a time out that adds task back to list if cant reach target anymore
            targetPos = target.transform.position;//Need a vector3 form to access the single axis?
            targetPos.y = transform.position.y;//Cancel y so it only moves on x and z and wander target obj stays at same y

            float legsOnGround = 0;
            foreach (var foot in footPositioners)
            {
                if(foot.footAtPosition)
                    legsOnGround += 1;
            }

            if (Vector3.Distance(this.transform.position, targetPos) > targetDistToTarget)//If over a certain distance then keep moving towards it
            {
                if (legsOnGround >= 2 || footPositioners.Count < 1)
                {
                    //todo move only the head and let the body follow
                    //Lerp rotation
                    Vector3 relativePos = targetPos - transform.position;
                    Quaternion toRotation = Quaternion.LookRotation(relativePos);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation,
                        animalProfile.rotSpeed * Time.deltaTime);

                    //Move Towards
                    transform.position = Vector3.MoveTowards(transform.position, targetPos,
                        animalProfile.moveSpeed * Time.deltaTime);
                }
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
            animalProfile.hunger = 100;
            currentTask = null;
            if (target.GetComponent<CreatureStats>() != null)
            {
                target.GetComponent<CreatureStats>().Die();
                Debug.Log("chopped and killed");
            }
            else if(target.gameObject.CompareTag("Food"))
            {
                Destroy(target);
            }
        }
        else if (currentTask == "Drink")
        {
            animalProfile.thirst = 100;
            currentTask = null;
        }
        else if (currentTask == "Mate")
        {
//            Debug.Log("making mini me: "+this.gameObject.name+creatureStats.reproductiveUrge);
            animalProfile.reproductiveUrge = 0;
            currentTask = null;
            target = null;
            if (GameObject.FindGameObjectsWithTag(this.transform.tag).Length < 200) //Make a hard cap for animals so it doesnt crash
            {
                GameObject baby;
                if(this.gameObject.transform.parent != null)
                    baby = Instantiate(this.gameObject.transform.parent.gameObject);//check if they have a parent
                else
                    baby = Instantiate(this.gameObject);
                baby.transform.name = this.transform.name + " Gen"+(animalProfile.gen+1);//Display generations for testing
                baby.GetComponent<CreatureStats>().gen = animalProfile.gen + 1;
                baby.GetComponent<CreatureStats>().Born();
            }
        }
        else//
        {
            GetNewTask();
        }
    }

    public void GetNewTask()
    {
        //Check that their is a possible target for the next task and set currenttask to that else try the next task in list
        foreach (String possibleTask in taskList)
        {
            //have a list of tasks and it checks each else wander
            //if they are currently trying to eat or their next possible task is eat then eat//Allows for overwriting wandering 
            if ((possibleTask == "Eat" || currentTask == "Eat") && getClosestTargetWithTag(animalProfile.foodTag) != null)//todo am calling getClosest function twice which is expensive so need alternative
            {
                target = getClosestTargetWithTag(animalProfile.foodTag);
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
                //This is low priority so focus on survival tasks first
                if (!taskList.Contains("Eat") && !taskList.Contains("Drink"))
                {
                    target = getClosestTargetWithTag(this.tag);
                    currentTask = possibleTask;
                    taskList.Remove(possibleTask);
                    return;
                }
            }
        }
        //This only runs if a target couldnt be found for any of the other tasks
        currentTask = "Wander";
        GetValidWanderPosition();

    }

    //Checks all items in memory and returns the closest one//Better than searching by all with tag
    GameObject getClosestTargetWithTag(String tag)
    {
        target = null;
        
        //Make an array of animals can mate with and fidn the closest one
        
        //GameObject[] allPossibleObjs = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> inactiveMemoryObjs = new List<GameObject>();//Cant remove an object from a list its looping through so add to garbage list and delete after
        float closestPathDistance = 99999;
        GameObject closestObj = null;
        foreach (GameObject possibleObj in objSensedMemory)//todo fix prob of things getting destroyed but not removed from list
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
                inactiveMemoryObjs.Add(possibleObj);
            }
        }

        //Clear all bad objs from memory
        foreach (GameObject badObj in inactiveMemoryObjs)
        {
            objSensedMemory.Remove(badObj);
//            Debug.Log("badObj removed from memory");
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

    void GetValidWanderPosition()
    {
        
        bool hasFound = false;
        int i = 0;//While safety limit
        Vector3 possiblePos;//Bit over complicated but i was having problems where animals where clumping together for some reason
        Vector3 currentPos = this.transform.position;
        while (hasFound == false && i<100)
        {
           
            possiblePos =  new Vector3(Random.Range(currentPos.x-animalProfile.wanderRadius, currentPos.x+animalProfile.wanderRadius), currentPos.y+40, Random.Range(currentPos.z-animalProfile.wanderRadius+(transform.forward.z*animalProfile.forwardWanderBias), currentPos.z+animalProfile.wanderRadius-(transform.forward.z*animalProfile.forwardWanderBias)));//add forward bias at end  +(transform.forward*forwardWanderBias)
            //Use a ray to test if its ground, if not then get a new point
            RaycastHit hit;
            if (Physics.Raycast(possiblePos, Vector3.down, out hit, 100,layerMask) &&
                hit.transform.CompareTag("Ground"))
            {
//                Debug.Log("GetValidWanderPosition() worked");
                Debug.DrawRay(possiblePos,Vector3.down,Color.blue);
                hasFound = true;
                wanderPosObj.transform.position = hit.point;
                target = wanderPosObj;
            }
            else
            {
//                Debug.Log("GetValidWanderPosition() timed out"+this.gameObject.name);
            }

            i++;
        }
        target = wanderPosObj;//THis happens when cant find obj so just set it anywhere to avoid infinite loop
        
    }
}
