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
    
    //use these by reference instead
    public Rigidbody rb;
    public float tooCloseDist = 3;
    public float attackRange = 3;
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
    public GameObject combatAnimal;

    public float lastAttackTime = 0;

    public GameObject hitCanvas;
    public GameObject heartCanvas;
    //public GameObject deathCanvas;
    public GameObject foodCanvas;
    public GameObject drinkCanvas;

    public string currentTask;
    public AnimalAudioManager audioManager;
    public AnimalForce animalForce;

    //public Vector3 forceToApply;
    


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
        
        
    }
    

    [Task]
    void EnemyIsTooCloseCondition()
    {
        float closestPredator = Mathf.Infinity;

        bool found = false;
        foreach (var obj in brain.objSensedMemory)
        {
            float distanceToCurrent = Vector3.Distance(rb.transform.position, obj.transform.position);
            //todo run from multiple
            if (distanceToCurrent<tooCloseDist&&obj.transform.name!=this.transform.name&&obj.GetComponent<AnimalBrain>()!=null&&obj.GetComponent<AnimalBrain>().predatorRating>brain.preyRating && distanceToCurrent<tooCloseDist)
            {
                found = true;
                fromTarget = obj.transform;
                isPanicked = true;
                StartCoroutine(panicCoolown());//Become panicked until timer stops
                Task.current.Succeed();
                break;
            }
        }

        if (isPanicked)
        {
            Task.current.Succeed();
        }
        else if (found == false)
        {
            fromTarget = null;
            Task.current.Fail();
        }
    }

    [Task]
    void ApplyForce()
    {
        print("Apply force");
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
        Vector3 forceToApply = new Vector3(0,0,0);
        bool found = false;
        if (!isPanicked)
        {
            foreach (var obj in brain.objSensedMemory)
            {
                float distanceToCurrent = Vector3.Distance(rb.transform.position, obj.transform.position);
                //todo run from multiple
                if (distanceToCurrent<brain.animalLength*15&&(obj.GetComponent<AnimalBrain>()!=null || obj.transform.CompareTag("Player"))&&obj.transform!=toTarget&&obj.transform!=combatAnimal)//if animal is tooclose and not a target
                {
                    Vector3 fleeDir;
                    fleeDir = (rb.transform.position - obj.transform.position).normalized;
                    Vector3 locDir = rb.transform.InverseTransformDirection(fleeDir);
                    locDir.y = 0;
                    Vector3 force = locDir * (tooCloseDist/distanceToCurrent)*(brain.moveSpeed/2);//Add avoid forc ebased on dist
//                print(force);

                    float pushAmount = (brain.animalHeight*4)/distanceToCurrent;
                
                
                    if (obj.GetComponent<AnimalBrain>()!=null && (obj.GetComponent<AnimalBrain>().preyRating > brain.preyRating))
                        pushAmount *= 1.5f;//more push from predators
                
                    pushAmount = Mathf.Clamp(pushAmount,0,brain.moveSpeed/2);

                    animalForce.AddToForce(force * pushAmount);
                    //forceToApply = forceToApply + (force * pushAmount);
                    found = true;
                }
            }
        }
        //rb.AddRelativeForce(forceToApply*Time.deltaTime*100);
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
                Vector3 fleeDir;
                fleeDir = (rb.transform.position - combatAnimal.transform.position).normalized;
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
            Vector3 fleeDir;
            fleeDir = (rb.transform.position - fromTarget.position).normalized;
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
        float closestWeakAnimal = Mathf.Infinity;
        bool found = false;
        foreach (var obj in brain.objSensedMemory)
        {
            float distanceToCurrent =
                Vector3.Distance(rb.transform.position, obj.transform.position);
            if (obj.transform.name!=this.transform.name&&obj.activeInHierarchy&&obj.GetComponent<AnimalBrain>()!=null&&obj.GetComponent<AnimalBrain>().preyRating<brain.predatorRating && distanceToCurrent<closestWeakAnimal)//get easiest and closest target
            {
                closestWeakAnimal = distanceToCurrent;
                toTarget = obj.transform;
                Task.current.Succeed();
                currentTask = "Hunting "+toTarget.name;
                found = true;
                break;
            }
        }
        if(found==false)
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
        //todo only if not panicked
        bool found = false;
        foreach (var obj in brain.objSensedMemory)
        {
            if (obj.transform.name == transform.name&&obj.transform!=transform &&obj.activeInHierarchy&&obj.GetComponent<AnimalBrain>()!=null&& obj.GetComponent<AnimalBrain>().reproductiveUrge>90 && obj.GetComponent<AnimalBehaviours>().combatAnimal == null)
            {
                toTarget = obj.transform;
                Task.current.Succeed();
                found = true;
                break;
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
        //todo only if not panicked
        bool found = false;
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

        if (found == false)
        {
            Task.current.Fail();
        }
    }
    
    [Task]
    void SeekTarget()
    {
        if (toTarget != null && !isStunned)
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
        if (toTarget.GetComponent<AnimalBrain>().preyRating < brain.predatorRating || toTarget.GetComponent<AnimalBrain>().preyRating < brain.preyRating)
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
        if (combatAnimal != null && combatAnimal.activeInHierarchy == true)
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
        if (toTarget != null)
        {
            float distance = Vector3.Distance(toTarget.transform.position, headObject.position);
            if (distance < brain.animalLength*1.5f)
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
        if (toTarget.GetComponent<AnimalBrain>()!=null &&toTarget.transform.name!=transform.name&& toTarget.gameObject.activeInHierarchy&&Vector3.Distance(toTarget.position,headObject.position)<attackRange+brain.animalHeight)//if has health
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
                    StartCoroutine(otherBehaviours.panicCoolown());
                    StartCoroutine(otherBehaviours.stunCoolown());
                    toTarget.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);//Hit slows other down
                    StartCoroutine(panicCoolown());//Is engaged in combat 
                    otherAnimalBrain.health -= 5; //todo brain attack strength 
                    //animalForce.AddToForce(rb.transform.forward*(brain.moveSpeed/2));
                    rb.AddRelativeForce(rb.transform.forward*(brain.moveSpeed/5)*Time.deltaTime*100,ForceMode.Impulse);

                    Instantiate(hitCanvas, (otherBehaviours.headObject.transform.position), transform.rotation);
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
        if (toTarget.CompareTag("Food") || (toTarget.GetComponent<AnimalBrain>()!=null && toTarget.GetComponent<AnimalBrain>().foodWorth>0&& toTarget.GetComponent<AnimalBrain>().health<=0) && toTarget.gameObject.activeInHierarchy)//if natural food or animal
        {
            brain.hunger = 100;
            Task.current.Succeed();
//            print("eat");
            //animalForce.AddToForce(-rb.transform.up*(brain.moveSpeed/5));
            rb.AddRelativeForce(-rb.transform.up*(brain.moveSpeed/5)*Time.deltaTime*100,ForceMode.Impulse);
            Instantiate(foodCanvas, (toTarget.transform.position), transform.rotation);
            combatAnimal = null;//If was targeting to eat then complete that
            if(toTarget.parent.GetComponent<Food>()!=null)
                toTarget.parent.GetComponent<Food>().isEaten();
            if(toTarget.parent.GetComponent<AnimalBrain>()!=null)
                toTarget.parent.GetComponent<AnimalBrain>().foodWorth -= 1;
            

        }
        else if(toTarget.CompareTag("Water"))
        {
            brain.thirst = 100;
            Task.current.Succeed();
            Instantiate(drinkCanvas, (toTarget.transform.position), transform.rotation);
            //animalForce.AddToForce(-rb.transform.up*(brain.moveSpeed/5));
            rb.AddRelativeForce(-rb.transform.up*(brain.moveSpeed/5)*Time.deltaTime*100,ForceMode.Impulse);
//            print("drink");
        }
        else if(toTarget.transform.name==(transform.name))
        {
            print("mate");
            int amountToSpawn = Random.Range(1,Mathf.RoundToInt(brain.litterSize));
            
            brain.reproductiveUrge = 0;
            Instantiate(heartCanvas, (toTarget.transform.position+this.transform.position)/2, transform.rotation);
            toTarget.GetComponent<AnimalBrain>().reproductiveUrge = 0;
            for (int i = 0; i < amountToSpawn; i++)
            {
                brain.GiveBirth(brain,toTarget.GetComponent<AnimalBrain>());
            }
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void HasWanderTargetCondition()
    {
//        print("Has:"+toTarget+":"+Vector3.Distance(wanderObj.transform.position, rb.transform.position));
        if (toTarget != null&&toTarget==wanderObj.transform&&Vector3.Distance(wanderObj.transform.position, rb.transform.position) < brain.wanderRadius*2)
        {
            Task.current.Succeed();
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

    IEnumerator panicCoolown()
    {
        isPanicked = true;
        yield return new WaitForSeconds(3);
        isPanicked = false;
        combatAnimal = null;
    }
    
    IEnumerator stunCoolown()
    {
        isStunned = true;
        yield return new WaitForSeconds(1f);
        isStunned = false;
    }
    

    void hit()
    {
        Instantiate(hitCanvas, (toTarget.transform.position+this.transform.position)/2, transform.rotation);
    }

}
