using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;

using Panda;
using Random = UnityEngine.Random;

public class AnimalBehaviours : MonoBehaviour
{
    public AnimalBrain brain;
    public AnimalGravity gravityScript;
    
    //use these by reference instead
    public Rigidbody rb;
    public Transform toTarget;
    public Transform fromTarget;
    public GameObject wanderObj;
    public Transform headObject;

    public float distLastFrame;
    public float failCloserChecks;
    
    public int layerMask;
    public float lastWanderSuccess;
    public bool isPanicked = false;
    public bool isStunned = false;
    public bool isSleeping = false;
    public GameObject combatAnimal;

    //For performed with delay
    public float lastAttackTime = 0;
    
    //Object pools instead of instanciating
    public ObjectPool foodCanvasPool;
    public ObjectPool heartCanvasPool;
    public ObjectPool drinkCanvasPool;
    public ObjectPool attackCanvasPool;
    public ObjectPool questionCanvasPool;
    public ObjectPool sleepCanvasPool;

    public string currentTask;
    public AnimalAudioManager audioManager;
    public AnimalForce animalForce;

    public GameObject playerCam;
    //public Vector3 forceToApply;

    public SunAngle angleScript;
    
    public int secondsToSleep = 10;

    public GameObject leader;
    


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (toTarget != null)
        {
            Gizmos.DrawSphere(toTarget.transform.position, .2f);
            Gizmos.DrawLine(transform.position, toTarget.transform.position);

        }
        Gizmos.color = Color.blue;
        if (combatAnimal != null)
        {
            Gizmos.DrawSphere(toTarget.transform.position, .2f);
            Gizmos.DrawLine(transform.position, combatAnimal.transform.position);

        }
    }

    private void Awake()
    {
        wanderObj=new GameObject("WanderObj");
        wanderObj.transform.parent = this.transform.parent;
        layerMask = 1 << 8;//bit shift to get mask for raycasting so only on environment and not other animals

        foodCanvasPool = GameObject.Find("FoodCanvasPool").GetComponent<ObjectPool>();
        heartCanvasPool = GameObject.Find("HeartCanvasPool").GetComponent<ObjectPool>();
        drinkCanvasPool = GameObject.Find("DrinkCanvasPool").GetComponent<ObjectPool>();
        attackCanvasPool = GameObject.Find("AttackCanvasPool").GetComponent<ObjectPool>();
        questionCanvasPool = GameObject.Find("QuestionCanvasPool").GetComponent<ObjectPool>();
        sleepCanvasPool = GameObject.Find("SleepCanvasPool").GetComponent<ObjectPool>();
        playerCam = GameObject.FindGameObjectWithTag("MainCamera");

        gravityScript = GetComponent<AnimalGravity>();
        angleScript = GameObject.Find("Core").GetComponent<SunAngle>();

    }
    


    [Task]
    void ObstacleAvoid()
    {
        Task.current.Succeed();
        RaycastHit hit; //shoot ray and if its ground then spawn at that location
        if (Physics.Raycast(headObject.position, rb.transform.forward, out hit, brain.animalHeight*4))
        {
            //todo improve this, this is temp
            animalForce.AddToForce(transform.right*(brain.moveSpeed/2));
            //rb.AddRelativeForce(transform.right*(brain.moveSpeed/2)*Time.deltaTime*100);
            Task.current.Succeed(); //if found no enemies
        }
        
    }

    [Task]
    void AvoidOthers()
    {
        Task.current.Succeed();
        if (!isPanicked)
        {
            foreach (var obj in brain.objSensedMemory)
            {
                float distanceToCurrent = Vector3.Distance(rb.transform.position, obj.transform.position);
                //todo run from multiple
                if (distanceToCurrent<brain.animalLength*15&&(obj.GetComponent<AnimalBrain>()!=null || obj.transform.CompareTag("MainCamera"))&&obj.transform!=toTarget)//if animal is tooclose and not a target
                {
                    Vector3 fleeDir;
                    fleeDir = (rb.transform.position - obj.transform.position).normalized;
                    Vector3 locDir = rb.transform.InverseTransformDirection(fleeDir);
                    locDir.y = 0;
                    Vector3 force = locDir * brain.moveSpeed;//Add avoid forc ebased on dist
//                print(force);

                    float pushAmount = (brain.animalHeight)/distanceToCurrent;
                
                
                    if (obj.GetComponent<AnimalBrain>()!=null && (obj.GetComponent<AnimalBrain>().preyRating > brain.preyRating))
                        pushAmount *= 1.5f;//more push from predators
                
                    pushAmount = Mathf.Clamp(pushAmount,0,brain.moveSpeed/2);

                    animalForce.AddToForce(force * pushAmount);
                    //forceToApply = forceToApply + (force * pushAmount);
                }
            }
        }
    }

    [Task]
    void ChasePrey()
    {
        if (toTarget != null)
        {
            currentTask = "Hunting prey";
            Task.current.Succeed();//if found no enemies
            if (Vector3.Distance(toTarget.transform.position, headObject.position) > brain.animalLength)
            {
                Vector3 seekDir;
                seekDir = (toTarget.position - rb.transform.position);//dont normalize because need the force amounts
                Vector3 locDir = rb.transform.InverseTransformDirection(seekDir);
                locDir.y = 0;

                Vector3 force = locDir.normalized * brain.moveSpeed;
                animalForce.AddToForce(force*2);
                animalForce.isPanicked = true;//double the force applied
                //rb.AddRelativeForce(force*2*Time.deltaTime*100);
            }
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void FleeFromTarget()
    {
        if (combatAnimal != null)
        {
            Task.current.Succeed(); //if found no enemies
            currentTask = "Fleeing from predator";
            if (!isStunned)
            {
                Vector3 fleeDir = (rb.transform.position - combatAnimal.transform.position).normalized;
                Vector3 locDir = rb.transform.InverseTransformDirection(fleeDir);
                locDir.y = 0;
                Vector3 force = locDir * brain.moveSpeed;
                animalForce.AddToForce(force * 3);
                animalForce.isPanicked = true;//double the force applied
                //rb.AddRelativeForce(force * 3 * Time.deltaTime * 100);
            }
            
        }
        else
        {
            Task.current.Fail();
        }
    }
    


    [Task]
    void FleeFromPredator()
    {
        if (fromTarget != null && !isStunned)
        {
            currentTask = "Fleeing from predator";
            Vector3 fleeDir = (rb.transform.position - fromTarget.position).normalized;
            Vector3 locDir = rb.transform.InverseTransformDirection(fleeDir);
            locDir.y = 0;
            Vector3 force = locDir * brain.moveSpeed;
            animalForce.AddToForce(force*2);
            animalForce.isPanicked = true;//double the force applied
            //rb.AddRelativeForce(force*2*Time.deltaTime*100);
            Task.current.Succeed(); //if found no enemies
        }
        else
        {
            Task.current.Fail();
        }
    }


    [Task]
    void GetAnimalTarget()
    {
        bool found = false;
        //If eats meat and is not already targeting an animal
        if (brain.eatsMeat)
        {
            //If already has target else get
            if (toTarget&&toTarget.GetComponent<AnimalBrain>()!=null &&
                toTarget.GetComponent<AnimalBrain>().preyRating < brain.predatorRating &&toTarget.GetComponent<AnimalBrain>().foodWorth>0)
            {
                print("get an target, alreayd has");
                found = true;
            }
            else
            {
                float closestWeakAnimal = Mathf.Infinity;
                foreach (var obj in brain.objSensedMemory)
                {
                    if (obj.GetComponent<AnimalBrain>())
                    {
                        AnimalBrain otherAnBrain = obj.GetComponent<AnimalBrain>();
                        
                        //If dead target then just use that instead of attacking alive one
                        if (otherAnBrain.health<=0 && otherAnBrain.foodWorth>0)
                        {
                            print("get an target, theres a dead");
                            found = true;
                            break;
                        }
                    
                        float distanceToCurrent = Vector3.Distance(rb.transform.position, obj.transform.position);
                    
                        if (obj.transform.name != this.transform.name && obj.activeInHierarchy && otherAnBrain.preyRating < brain.predatorRating && distanceToCurrent < closestWeakAnimal) //get easiest and closest target
                        {
                            print("get an target, an to hunt");
                            closestWeakAnimal = distanceToCurrent;
                            toTarget = obj.transform;
                            currentTask = "Hunting " + toTarget.name;
                            found = true;
                        }
                    }
                }
            }
        }

        if (found)
            Task.current.Succeed();
        else
            Task.current.Fail();
    }
    
    [Task]
    void TargetPlants()
    {
        
        //todo only if not panicked
        bool found = false;
        if (brain.eatsPlants)
        {
            foreach (var obj in brain.objSensedMemory)
            {
                if (obj.transform.CompareTag("Food") && obj.gameObject.activeInHierarchy)
                {
                    toTarget = obj.transform;
                    Task.current.Succeed();
                    currentTask = "Eating plants";
                    found = true;
                    break;
                }
            } 
        }

        if (found == false)
        {
            Task.current.Fail();
        }
    }
    
    [Task]
    void TargetMate()
    {
        bool found = false;
        if (brain.hunger > brain.hungerThresh && !isPanicked) //Make sure animal has eaten first to avoid overpopulation
        {
            foreach (var obj in brain.objSensedMemory)
            {
                if (obj.transform.name == transform.name && obj.GetComponent<AnimalBrain>()) //if animal of same type
                {
                    AnimalBrain otherBrain;
                    otherBrain = obj.GetComponent<AnimalBrain>();

                    //dif obj, opposite gender, active,
                    if (obj.transform != transform && !otherBrain.genderIsMale && obj.activeInHierarchy &&
                        otherBrain.reproductiveUrge > 90 && otherBrain.health >= 0 && !obj.GetComponent<AnimalBehaviours>().isPanicked)
                    {
                        toTarget = obj.transform;
                        Task.current.Succeed();
                        found = true;
                        break;
                    }
                }


            }
        }

        if (found == false)
        {
            Task.current.Fail();
        }
    }
    
    [Task]
    void TargetResource(String resource)
    {

        bool found = false;
        if (toTarget)
            found = toTarget.transform.CompareTag(resource) && toTarget.gameObject.activeInHierarchy;
        //if current target isnt the resource or active then this is false and it continues to find a new one

        if (!isPanicked && !found)
        {
            foreach (var obj in brain.objSensedMemory)
            {
                if (obj.transform.CompareTag(resource) && found==false && obj.activeInHierarchy)
                {
                    currentTask = "Getting "+resource;
                    toTarget = obj.transform;
                    Task.current.Succeed();
                    found = true;
                    break;
                }
            }
        }
        else//already has a target
        {
            found = true;
            Task.current.Succeed();
        }
       

        if (found == false)
        {
            Task.current.Fail();
        }
    }
    
    [Task]
    void SeekTarget()
    {
        if (toTarget && !isStunned)
        {
            //currentTask = "Getting "+toTarget.transform.name;
            Vector3 seekDir;
            seekDir = (toTarget.position - rb.transform.position);//dont normalize because need the force amounts
            Vector3 locDir = rb.transform.InverseTransformDirection(seekDir);
            locDir.y = 0;

            Vector3 force = locDir.normalized * brain.moveSpeed;
            animalForce.AddToForce(force*2);
            //rb.AddRelativeForce(force*Time.deltaTime*100);
            Task.current.Succeed();//if found no enemies
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void WantsToAttackCondition()
    {
        //IF other animal is weaker and not scarier and health is high enough
        if (brain.health>brain.maxHealth/10&&toTarget.GetComponent<AnimalBrain>().preyRating < brain.predatorRating || toTarget.GetComponent<AnimalBrain>().preyRating < brain.preyRating)
        {
            Task.current.Succeed();

        }
        else
        {
            Task.current.Fail();

        }
    }

    [Task]
    void IsSleepingCondition()
    {
        if (isSleeping)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();

        }
    }


    [Task]
    void HasCombatTarget()
    {
        if (combatAnimal && combatAnimal.activeInHierarchy == true)
        {
            Task.current.Succeed();
            currentTask = "Fighting";
            toTarget = combatAnimal.transform;
            animalForce.isPanicked = true;

        }
        else
        {
            combatAnimal = null;
            Task.current.Fail();

        }
    }

    [Task]
    void ArrivedAtTargetCondition()
    {
        if (toTarget)
        {
            float distance = Vector3.Distance(toTarget.transform.position, headObject.position);
            if (distance < brain.animalHeight*3f)
            {
//                print(transform.name+distance+" and "+(attackRange+brain.animalHeight));
                Task.current.Succeed();
            }
            else
            {
                Task.current.Fail();
            }
        }
        else
        {
            Task.current.Fail();

        }

    }
    
    [Task]
    void CheckWorking()
    {
        print("working on"+gameObject.transform.name);
    }
    
    [Task]
    void AttackTarget()
    {
        if (toTarget.GetComponent<AnimalBrain>() &&toTarget.transform.name!=transform.name&& toTarget.gameObject.activeInHierarchy)//if has health
        {
            AnimalBrain otherAnimalBrain = toTarget.GetComponent<AnimalBrain>();
            if (otherAnimalBrain.health > 0)
            {
                Task.current.Succeed();//dont fail during cooldown
                if (Time.time > lastAttackTime + brain.attackRate)
                {
                    combatAnimal = toTarget.gameObject;
                    AnimalBehaviours otherBehaviours = toTarget.GetComponent<AnimalBehaviours>();
                    otherBehaviours.combatAnimal = this.gameObject;
                    StartCoroutine(otherBehaviours.PanicCoolown());
                    StartCoroutine(otherBehaviours.StunCoolown());
                    toTarget.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);//Hit slows other down
                    StartCoroutine(PanicCoolown());//Is engaged in combat 
                    otherAnimalBrain.health -= 5; //todo brain attack strength 
                    //animalForce.AddToForce(rb.transform.forward*(brain.moveSpeed/2));
                    rb.AddRelativeForce(rb.transform.forward*(brain.moveSpeed/5)*Time.deltaTime*100,ForceMode.Impulse);

                    GameObject hitCanvas = attackCanvasPool.GetObj();
                    hitCanvas.transform.position = headObject.transform.position+transform.up;
                    
                    audioManager.playAttack();

                    lastAttackTime = Time.time;
                    //print("attack");
                }
            }
            else
            {
                Task.current.Fail();
            }
        }
        else
        {
            Task.current.Fail();
        }
    }
    
    
    [Task]
    void CompleteTarget()//if health lower than x
    {
        if (((toTarget.CompareTag("Food") && brain.hunger<brain.hungerThresh) || (toTarget.GetComponent<AnimalBrain>() && toTarget.GetComponent<AnimalBrain>().foodWorth>0&& toTarget.GetComponent<AnimalBrain>().health<=0) && toTarget.gameObject.activeInHierarchy&&brain.hunger<brain.hungerThresh))//if natural food or animal
        {
            brain.hunger = 100;
            Task.current.Succeed();
//            print("eat");
            //animalForce.AddToForce(-rb.transform.up*(brain.moveSpeed/5));
            rb.AddRelativeForce(-rb.transform.up*(brain.moveSpeed/5)*Time.deltaTime*100,ForceMode.Impulse);
            GameObject foodCanvasObj = foodCanvasPool.GetObj();
            foodCanvasObj.transform.position = toTarget.transform.position;
            //Instantiate(foodCanvas, (toTarget.transform.position), transform.rotation);
            combatAnimal = null;//If was targeting to eat then complete that
            if(toTarget.parent.GetComponent<Food>())
                toTarget.parent.GetComponent<Food>().isEaten();
            if(toTarget.parent.GetComponent<AnimalBrain>())
                toTarget.parent.GetComponent<AnimalBrain>().foodWorth -= 1;
            toTarget = null;
            

        }
        else if(toTarget.CompareTag("Water")&& brain.thirst<brain.thirstThresh)
        {
            brain.thirst = 100;
            Task.current.Succeed();
            GameObject drinkCanvas = drinkCanvasPool.GetObj();
            drinkCanvas.transform.position = toTarget.transform.position;
            
            //animalForce.AddToForce(-rb.transform.up*(brain.moveSpeed/5));
            rb.AddRelativeForce(-rb.transform.up*(brain.moveSpeed/5)*Time.deltaTime*100,ForceMode.Impulse);
//            print("drink");
            toTarget = null;
        }
        else if (toTarget.transform.name == (transform.name) && brain.reproductiveUrge > 90 &&
                 toTarget.GetComponent<AnimalBrain>().health >= 0) 
        {
            print("mate");
            int amountToSpawn = Random.Range(1,Mathf.RoundToInt(brain.litterSize));
            
            brain.reproductiveUrge = 0;
            GameObject heartCanvas = heartCanvasPool.GetObj();
            heartCanvas.transform.position = (toTarget.transform.position+this.transform.position)/2;
            
            toTarget.GetComponent<AnimalBrain>().reproductiveUrge = 0;
            for (int i = 0; i < amountToSpawn; i++)
            {
                brain.GiveBirth(brain,toTarget.GetComponent<AnimalBrain>());
            }
            Task.current.Succeed();
            toTarget = null;
        }
        else if(toTarget == playerCam.transform)//investigation
        {
            GameObject questionCanvas = questionCanvasPool.GetObj();
            questionCanvas.transform.position = headObject.transform.position+(rb.transform.up*(brain.animalHeight/2));
            toTarget = null;
            StartCoroutine(StunCoolown());
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void RemoveWanderTarget()
    {
        toTarget = null;
        Task.current.Succeed();
    }

    [Task]
    void HasWanderTargetCondition()
    {
//        print("Has:"+toTarget+":"+Vector3.Distance(wanderObj.transform.position, rb.transform.position));
        if (toTarget&&(toTarget==wanderObj.transform || toTarget==playerCam.transform)&&Vector3.Distance(wanderObj.transform.position, rb.transform.position) < brain.wanderRadius*2)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }
    [Task]
    void Sleep(float chanceToTrigger)//chance to trigger between 0 and 1
    {
        if (Random.value < chanceToTrigger && angleScript.GetTargetAngleToSun(this.gameObject,false)<60 && !isSleeping&& !isPanicked)//nighttime
        {
            StartCoroutine("Sleeping");
            print("sleep condition");
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void InvestigatePlayer(float chanceToTrigger)
    {
        if (Random.value < chanceToTrigger && angleScript.GetTargetAngleToSun(this.gameObject,false)<60 && !isSleeping&& !isPanicked)//nighttime
        {
            StartCoroutine("Sleeping");
            print("sleep condition");
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void FollowPack()
    {
//        print("leader: " + leader);
        AnimalBehaviours leaderBehaviour = leader.GetComponent<AnimalBehaviours>();
        //print("leader: "+leader+", lt: "+leaderBehaviour.toTarget.name+", ");

        //Only follow behaviour for wandering toegther or fighting, not mating
        if (leaderBehaviour.toTarget && (leaderBehaviour.toTarget.name == ("WanderObj") || (leaderBehaviour.toTarget.GetComponent<AnimalBrain>()!=null &&leaderBehaviour.toTarget.name != brain.name)))
        {
            currentTask = "Following Pack";
            toTarget = leaderBehaviour.toTarget;
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }


    [Task]
    void HasLeaderCondition()
    {
        if (leader != null && leader.activeInHierarchy&& Vector3.Distance(leader.transform.position,transform.position)<brain.sensoryRange/2)
        {
            Task.current.Succeed();
        }
        else
        {
            leader = null;
            Task.current.Fail();
        }
    }

    [Task]
    void GetLeader()
    {
        bool found = false;
        float strongestFound = brain.predatorRating;//start at own rating so the other has to be higher
        
        foreach (var obj in brain.objSensedMemory)
        {
            //if active animal and close
            if (obj.gameObject.activeInHierarchy && obj.GetComponent<AnimalBrain>() && Vector3.Distance(obj.transform.position,transform.position)<brain.sensoryRange/2)
            {
                AnimalBrain objBrain = obj.GetComponent<AnimalBrain>();

                //if stronger of same type
                if (obj.transform.name == (transform.name) && objBrain.predatorRating > brain.predatorRating && objBrain.predatorRating > strongestFound)
                {
                    strongestFound = objBrain.predatorRating;

                    leader = obj;
                    found = true;
                }
            }
        }

        if (found)
        {
            Task.current.Succeed();
            //toTarget = leader.GetComponent<AnimalBehaviours>().toTarget;
        }
        else
        {
            Task.current.Fail();

        }
    }
    

    [Task]
    void GetWanderTarget()
    {
        Vector3 randomPoint = headObject.position +(Random.insideUnitSphere * brain.wanderRadius);
        Vector3 tarPos = randomPoint +(rb.transform.up*brain.wanderRadius*2)+(rb.transform.forward*brain.forwardWanderBias);//+(rb.transform.forward*brain.forwardWanderBias)
        
        //Vector3 locTarPos = rb.transform.InverseTransformDirection(tarPos);//cancel out y
        //locTarPos.y = 0;//cancel out vertical force
        //tarPos = rb.transform.TransformDirection(locTarPos);//set the new cancelled related velocity

        
        
        RaycastHit hit; //shoot ray and if its ground then spawn at that location
        if (Physics.Raycast(tarPos, -rb.transform.up, out hit, 1000,layerMask))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                Debug.DrawLine(tarPos,hit.point,Color.white);
                wanderObj.transform.position = hit.point;
                toTarget = wanderObj.transform;
                currentTask = "Wandering";
            
                Task.current.Succeed();
            }
            else
            {
                Task.current.Fail();
            }
        }
        else
        {
            //toTarget.transform.position = rb.transform.position + (rb.transform.forward * 10);
            Task.current.Fail();
        }
        
            
        
    }
    

    [Task]
    void GettingCloserCondition()
    {
        float distThisFrame = Vector3.Distance(rb.transform.position, toTarget.transform.position);
//        print("distThisFrame"+distThisFrame+"  distLastFrame"+distLastFrame+  "  failCloserChecks"+failCloserChecks);
        if (distThisFrame+0.01f < distLastFrame)//todo switch to time not frame
        {
            lastWanderSuccess = Time.time;//reset time counter
        }

        if (Time.time > lastWanderSuccess+1)//if its been over a second since been stuck
        {
//            print("stuck too long. Time"+Time.time+" last success"+lastWanderSuccess+" ob"+this.transform.name);
            lastWanderSuccess = Time.time;
            Task.current.Fail();
//            print(" stuck, got new wander");
        }
        else
        {
            Task.current.Succeed();
        }

        distLastFrame = distThisFrame;
    }


    [Task]
    void IsHungryCondition()
    {
        if (brain.hunger < brain.hungerThresh)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }
    
    [Task]
    void IsThirstyCondition()
    {
        if (brain.thirst < brain.thirstThresh)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void InvestigatePlayer()
    {
        if (playerCam)
        {
            print("Investigating player");
            toTarget = playerCam.transform;
            wanderObj.transform.position = playerCam.transform.position;
            Task.current.Succeed();
            currentTask = "Investigating player";
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void WantsToMateCondition()//Todo add behaviour
    {
        //Only mate if not starving and wants to mate
        if (brain.reproductiveUrge > brain.mateThresh && !(brain.hunger<0)&&!(brain.thirst<0))
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void DebugFail()
    {
        Task.current.Fail();
    }
    
    IEnumerator Sleeping()
    {
        
        int secondsPassed = 0;
        isSleeping = true;

        float originalHeight = gravityScript.animalHeight;
        gravityScript.animalHeight = originalHeight / 2;

        while (secondsPassed < secondsToSleep*2 && !isPanicked)//*2 because it run every half of second
        {
            yield return new WaitForSeconds(.5f);
            GameObject sleepCanvasObj = sleepCanvasPool.GetObj();
            sleepCanvasObj.transform.position = headObject.position+transform.up;
            secondsPassed++;
        }
        isSleeping = false;
        gravityScript.animalHeight = originalHeight;
        
        
    }

    IEnumerator PanicCoolown()
    {
        isPanicked = true;
        yield return new WaitForSeconds(5);
        isPanicked = false;
        combatAnimal = null;
    }
    
    IEnumerator StunCoolown()
    {
        isStunned = true;
        yield return new WaitForSeconds(.3f);
        isStunned = false;
    }
    
}
