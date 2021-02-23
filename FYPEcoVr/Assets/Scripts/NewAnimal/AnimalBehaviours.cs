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
    public float tooCloseDist = 20;
    public float attackRange = 2;
    public Transform toTarget;
    public Transform fromTarget;
    public GameObject wanderObj;

    public float distLastFrame;
    public float failCloserChecks;
    
    private int layerMask;
    public float lastWanderSuccess;


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(toTarget!=null)
            Gizmos.DrawSphere(toTarget.transform.position, 1);
    }

    private void Awake()
    {
        wanderObj=new GameObject();
        layerMask = 1 << 8;//bit shift to get mask for raycasting so only on environment and not other animals
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
                    if (found==false && obj.transform.name==tag && Vector3.Distance(obj.transform.position,rb.transform.position)<tooCloseDist)
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
            Vector3 force = locDir * brain.moveSpeed;
            rb.AddRelativeForce(force*Time.deltaTime*100);
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
        Vector3 seekDir;
        seekDir = (toTarget.position - rb.transform.position);//dont normalize because need the force amounts
        Vector3 locDir = rb.transform.InverseTransformDirection(seekDir);
        locDir.y = 0;

        if (locDir.magnitude > attackRange)
        {
//            print("locDir.magnitude"+locDir.magnitude);
            if (locDir.z < -1) //if theres a force pushing back then  cant seek in that dir
            {
                Task.current.Fail();
            }
            else
            {
                Vector3 force = locDir.normalized * brain.moveSpeed;
                rb.AddRelativeForce(force*Time.deltaTime*100);
                Task.current.Succeed();//if found no enemies
            }
        }
        else
        {
//            print("locDir.magnitude"+locDir.magnitude+"  "+attackRange);
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
            rb.AddForce(seekDir * (brain.moveSpeed/4*Time.deltaTime)*Time.deltaTime*100);//slow down approach
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
                
            rb.AddForce(attackDir * 2*Time.deltaTime*100);
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
        Vector3 randomPoint = rb.transform.position +(Random.insideUnitSphere * brain.wanderRadius);
        Vector3 tarPos = randomPoint +(rb.transform.up*brain.wanderRadius*2)+(rb.transform.forward*brain.forwardWanderBias);//+(rb.transform.forward*brain.forwardWanderBias)
        
        //Vector3 locTarPos = rb.transform.InverseTransformDirection(tarPos);//cancel out y
        //locTarPos.y = 0;//cancel out vertical force
        //tarPos = rb.transform.TransformDirection(locTarPos);//set the new cancelled related velocity
            
            
            
        RaycastHit hit; //shoot ray and if its ground then spawn at that location
        if (Physics.Raycast(tarPos, -rb.transform.up, out hit, 1000,layerMask))
        {
            Debug.DrawLine(tarPos,hit.point,Color.white);
            toTarget.transform.position = hit.point+transform.up*brain.animalHeight;
            Task.current.Succeed();
        }
        else
        {
            //toTarget.transform.position = rb.transform.position + (rb.transform.forward * 10);
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
            lastWanderSuccess = Time.time;//reset time counter
        }
        else
        {
            Task.current.Succeed();
        }

        if (Time.time > lastWanderSuccess+1)//if its been over a second since been stuck
        {
            print("stuck too long. Time"+Time.time+" last success"+lastWanderSuccess+" ob"+this.transform.name);
            lastWanderSuccess = Time.time;
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
