using System;
using System.Collections;
using System.Collections.Generic;
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

    public float distLastFrame;
    public float failCloserChecks;
    
    private int layerMask;
    public float lastWanderSuccess;
    public bool isPanicked = false;
    public GameObject combatAnimal;

    public float lastAttackTime = 0;

    public GameObject hitCanvas;
    public GameObject heartCanvas;
    public GameObject deathCanvas;


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if(toTarget!=null)
            Gizmos.DrawSphere(toTarget.transform.position, 1);
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
    void ObstacleAvoid()
    {
        RaycastHit hit; //shoot ray and if its ground then spawn at that location
        if (Physics.Raycast(transform.position, rb.transform.forward, out hit, brain.animalHeight*4))
        {
            //todo improve this, this is temp
            rb.AddRelativeForce(transform.right*(brain.moveSpeed/2)*Time.deltaTime*100);
            Task.current.Succeed(); //if found no enemies

        }
        else
        {
            Task.current.Fail(); //if found no enemies

        }
        
    }

    [Task]
    void AvoidOthers()
    {
        Task.current.Succeed();
        bool found = false;
        foreach (var obj in brain.objSensedMemory)
        {
            float distanceToCurrent = Vector3.Distance(rb.transform.position, obj.transform.position);
            //todo run from multiple
            if (distanceToCurrent<tooCloseDist&&obj.GetComponent<AnimalBrain>()!=null&&obj.transform!=toTarget&&obj.transform!=combatAnimal)//if animal is tooclose and not a target
            {
                Vector3 fleeDir;
                fleeDir = (rb.transform.position - obj.transform.position).normalized;
                Vector3 locDir = rb.transform.InverseTransformDirection(fleeDir);
                locDir.y = 0;
                Vector3 force = locDir * (tooCloseDist/distanceToCurrent)*(brain.moveSpeed/2);//Add avoid forc ebased on dist
//                print(force);
                if (obj.GetComponent<AnimalBrain>().predatorRating > brain.preyRating)
                    force *= 3;
                rb.AddRelativeForce(force*Time.deltaTime*100);
                found = true;
            }
        }
    }

    [Task]
    void ChasePrey()
    {
        Vector3 seekDir;
        seekDir = (toTarget.position - rb.transform.position);//dont normalize because need the force amounts
        Vector3 locDir = rb.transform.InverseTransformDirection(seekDir);
        locDir.y = 0;

        if (locDir.magnitude > 1 && Vector3.Distance(toTarget.transform.position,rb.transform.position)>attackRange+brain.animalHeight)
        {
            Vector3 force = locDir.normalized * brain.moveSpeed;
            rb.AddRelativeForce(force*2*Time.deltaTime*100);
            Task.current.Succeed();//if found no enemies
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
            Vector3 fleeDir;
            fleeDir = (rb.transform.position - combatAnimal.transform.position).normalized;
            Vector3 locDir = rb.transform.InverseTransformDirection(fleeDir);
            locDir.y = 0;
            Vector3 force = locDir * brain.moveSpeed;
            rb.AddRelativeForce(force * 2 * Time.deltaTime * 100);
            Task.current.Succeed(); //if found no enemies
        }
        else
        {
            Task.current.Fail();
        }
    }
    


    [Task]
    void FleeFromPredator()
    {
        if (fromTarget != null)
        {
            Vector3 fleeDir;
            fleeDir = (rb.transform.position - fromTarget.position).normalized;
            Vector3 locDir = rb.transform.InverseTransformDirection(fleeDir);
            locDir.y = 0;
            Vector3 force = locDir * brain.moveSpeed;
            rb.AddRelativeForce(force*2*Time.deltaTime*100);
            Task.current.Succeed(); //if found no enemies
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void Mate()
    {
        brain.reproductiveUrge = 0;
        Instantiate(heartCanvas, (toTarget.transform.position+this.transform.position)/2, transform.rotation);
        toTarget.GetComponent<AnimalBrain>().reproductiveUrge = 0;
        brain.GiveBirth();
        Task.current.Succeed();

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
                if (obj.transform.name == "Food" && found==false)
                {
                    toTarget = obj.transform;
                    Task.current.Succeed();
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
            if (obj.transform.name == transform.name&&obj.transform!=transform &&obj.activeInHierarchy&&obj.GetComponent<AnimalBrain>()!=null&& obj.GetComponent<AnimalBrain>().reproductiveUrge>90)
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
            if (obj.transform.name == resource && found==false)
            {
                toTarget = obj.transform;
                Task.current.Succeed();
                found = true;
                break;
            }
        }

        if (found == false)
        {
            toTarget = null;
            Task.current.Fail();
        }
    }
    
    [Task]
    void SeekTarget()
    {
        if (toTarget != null)
        {
            Vector3 seekDir;
            seekDir = (toTarget.position - rb.transform.position);//dont normalize because need the force amounts
            Vector3 locDir = rb.transform.InverseTransformDirection(seekDir);
            locDir.y = 0;

            Vector3 force = locDir.normalized * brain.moveSpeed;
            rb.AddRelativeForce(force*Time.deltaTime*100);
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
        if (toTarget.GetComponent<AnimalBrain>().preyRating < brain.predatorRating)
        {
            Task.current.Succeed();

        }
        else
        {
            Task.current.Fail();

        }
    }

    [Task]
    void TargetCombatAnimal()
    {
        toTarget = combatAnimal.transform;
        Task.current.Succeed();

        
    }

    [Task]
    void IsInCombatCondition()
    {
        if (combatAnimal != null && combatAnimal.activeInHierarchy == true)
        {
            Task.current.Succeed();

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
            float distance = Vector3.Distance(toTarget.transform.position, rb.transform.position);
            if (distance < attackRange+brain.animalHeight)
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
        Task.current.Succeed();//not having cooled down shouldnt cause flee
        if (toTarget.GetComponent<AnimalBrain>()!=null&&Time.time>lastAttackTime+brain.attackRate && toTarget.gameObject.activeInHierarchy&&Vector3.Distance(toTarget.position,rb.transform.position)<attackRange+brain.animalHeight)//if has health
        {
            //todo add cooldown
            AnimalBrain otherAnimalBrain = toTarget.GetComponent<AnimalBrain>();
            combatAnimal = toTarget.gameObject;
            toTarget.GetComponent<AnimalBehaviours>().combatAnimal = this.gameObject;
            toTarget.GetComponent<AnimalBehaviours>().isPanicked = true;
            isPanicked = true;
            StartCoroutine(panicCoolown());
            otherAnimalBrain.health -= 5;//todo brain attack strength 

            Instantiate(hitCanvas, (toTarget.transform.position+this.transform.position)/2, transform.rotation);

            lastAttackTime = Time.time;
            print("attack");
        }
    }
    
    [Task]
    void ConsumeTarget()//if health lower than x
    {
        if (toTarget.name == "Food" || toTarget.GetComponent<AnimalBrain>()!=null && toTarget.GetComponent<AnimalBrain>().health<=0 && toTarget.gameObject.activeInHierarchy)//if natural food or animal
        {
            brain.hunger = 100;
            Task.current.Succeed();
            
        }
        else if(toTarget.name == "Water")
        {
            brain.thirst = 100;
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
        Vector3 randomPoint = rb.transform.position +(Random.insideUnitSphere * brain.wanderRadius);
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
            Task.current.Succeed();
            lastWanderSuccess = Time.time;//reset time counter
        }
        else
        {
            Task.current.Succeed();
        }

        if (Time.time > lastWanderSuccess+1)//if its been over a second since been stuck
        {
//            print("stuck too long. Time"+Time.time+" last success"+lastWanderSuccess+" ob"+this.transform.name);
            lastWanderSuccess = Time.time;
            Task.current.Fail();
        }

        distLastFrame = distThisFrame;
    }


    [Task]
    void IsHungryCondition()
    {
        if (brain.hunger < 50)
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
        if (brain.thirst < 50)
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
        if (brain.reproductiveUrge > 90)
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
        yield return new WaitForSeconds(3);
        isPanicked = false;
        combatAnimal = null;
    }
    

    void hit()
    {
        Instantiate(hitCanvas, (toTarget.transform.position+this.transform.position)/2, transform.rotation);
    }

}
