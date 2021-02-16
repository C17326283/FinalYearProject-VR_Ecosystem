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
    public float maxSpeed = 1000;
    public float tooCloseDist = 20;
    public float attackRange = 2;
    public Transform toTarget;
    public Transform fromTarget;
    public GameObject wanderObj;

    public float distLastFrame;
    public float failCloserChecks;


    private void Awake()
    {
        wanderObj=new GameObject();
    }

    [Task]
    void CheckIfEnemyClose()
    {
        bool found = false;
        foreach (var obj in brain.objSensedMemory)
        {
            if (found == false)
            {
//                print("obj");
                foreach (var tag in brain.huntedBy)
                {
//                    print(tag);
                    //todo run from multiple
                    if (found==false && obj.transform.CompareTag(tag) && Vector3.Distance(obj.transform.position,rb.transform.position)<tooCloseDist)
                    {
                        print("found");
                        found = true;
                        fromTarget = obj.transform;
                        Task.current.Succeed();
                        break;
                    }
                }
            }
        }

        if (found == false)
        {
            fromTarget = null;
            Task.current.Fail();
        }
        
    }
    
    [Task]
    void FleeFromEnemy()
    {
        if (fromTarget != null)
        {
            Vector3 fleeDir;
            fleeDir = (rb.transform.position - fromTarget.position).normalized;
            Vector3 locDir = rb.transform.InverseTransformDirection(fleeDir);
            locDir.y = 0;
            Vector3 force = locDir * maxSpeed;
            rb.AddRelativeForce(force);
            Task.current.Succeed(); //if found no enemies
        }
    }
    
    [Task]
    void CheckIfFoodClose()
    {
        //todo only if not panicked
        bool found = false;
        foreach (var obj in brain.objSensedMemory)
        {
            if (obj.transform.CompareTag("Food") && found==false)
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

        if (Vector3.Distance(toTarget.transform.position, rb.transform.position) > attackRange)
        {
            Vector3 seekDir;
            seekDir = (toTarget.position - rb.transform.position).normalized;
            Vector3 locDir = rb.transform.InverseTransformDirection(seekDir);
            locDir.y = 0;
            if (locDir.z < -1) //if theres a force pushing back then  cant seek in that dir
            {
                Task.current.Fail();
            }
            else
            {
                Vector3 force = locDir * maxSpeed;
                rb.AddRelativeForce(force);
                Task.current.Succeed();//if found no enemies
            }
        }
        else
        {
            Task.current.Fail();
        }
            
                
        
    }

    [Task]
    void ArriveTarget()
    {
        float distance = Vector3.Distance(toTarget.transform.position, rb.transform.position);
        if (distance < attackRange/4)
        {
            Task.current.Succeed();
        }
        else if (distance < attackRange)
        {
            Vector3 seekDir;
            seekDir = (toTarget.position - rb.transform.position).normalized;
            rb.AddForce(seekDir * (maxSpeed/4));//slow down approach
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }

    }
    
    [Task]
    void AttackTarget()
    {
        if (toTarget.GetComponent<AnimalBrain>() && Vector3.Distance(toTarget.position,rb.transform.position)<attackRange)//if has health
        {
            Rigidbody otherRb = toTarget.GetComponent<Rigidbody>();
            Vector3 attackDir;
            attackDir = (toTarget.position - rb.transform.position).normalized;
                
            rb.AddForce(attackDir * 2);
            Task.current.Succeed();
            print("attack");
        }
        else
        {
            Task.current.Fail();
        }
    }
    
    [Task]
    void EatTarget()//if health lower than x
    {
        Task.current.Succeed();
        brain.hunger = 100;
        print("eat");
    }

    [Task]
    void CheckWanderTarget()
    {
        toTarget = wanderObj.transform;
        if (Vector3.Distance(wanderObj.transform.position, rb.transform.position) < brain.wanderRadius)
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
        Vector3 randomePos = rb.position + Random.insideUnitSphere * brain.wanderRadius;
        randomePos.y = rb.position.y+20;//set at random height
        
        RaycastHit hit; //shoot ray and if its ground then spawn at that location
        if (Physics.Raycast(randomePos, -rb.transform.up, out hit, 100))
        {
            wanderObj.transform.position = hit.point;
            toTarget = wanderObj.transform;
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void CheckGettingCloser()
    {
        float distThisFrame = Vector3.Distance(rb.transform.position, toTarget.transform.position);
//        print("distThisFrame"+distThisFrame+"  distLastFrame"+distLastFrame+  "  failCloserChecks"+failCloserChecks);
        if (distThisFrame+0.01f < distLastFrame)//todo switch to time not frame
        {
            Task.current.Succeed();
            failCloserChecks = 0;
        }
        else
        {
            Task.current.Succeed();
            failCloserChecks += 1;
        }

        if (failCloserChecks > 50)
        {
            Task.current.Fail();
        }

        distLastFrame = distThisFrame;
    }


    [Task]
    void IsHungry()
    {
        if (brain.hunger < 20)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }
    
    [Task]
    void IsThirsty()
    {
        if (brain.thirst < 20)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }
    
    

}
