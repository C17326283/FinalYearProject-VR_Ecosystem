using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Panda;
public class AnimalBehaviours : MonoBehaviour
{
    public AnimalBrain brain;
    
    //use these by reference instead
    public Rigidbody rb;
    public float maxSpeed = 10;
    public float tooCloseDist = 5;
    public float attackRange = 1;
    public Transform toTarget;
    public Transform fromTarget;
    
    
    
    
    [Task]
    void CheckIfEnemyClose()
    {
        bool found = false;
        foreach (var obj in brain.objSensedMemory)
        {
            if (found == false)
            {
                print("obj");
                foreach (var tag in brain.huntedBy)
                {
                    print(tag);
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
            Vector3 force = locDir * maxSpeed;
            rb.AddRelativeForce(force);
            Task.current.Succeed();//if found no enemies
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
        if (toTarget.GetComponent<Rigidbody>() && Vector3.Distance(toTarget.position,rb.transform.position)<attackRange)//if has health
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
        if (toTarget.GetComponent<Rigidbody>())
        {
            
            Task.current.Succeed();
            print("eat");
        }
        else
        {
            Task.current.Fail();
        }
    }
}