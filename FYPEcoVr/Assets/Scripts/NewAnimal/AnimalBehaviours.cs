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
    public Transform target;
    
    
    [Task]
    void CheckIfEnemyClose()
    {
        bool found = false;
        foreach (var obj in brain.objSensedMemory)
        {
            if (obj.transform.CompareTag("Fox") && Vector3.Distance(obj.transform.position,rb.transform.position)<tooCloseDist)
            {
                Task.current.Succeed();
                found = true;
                break;
            }
            
        }
        if(found==false)
            Task.current.Fail();
    }
    
    [Task]
    void FleeFromEnemy()
    {
        foreach (var obj in brain.objSensedMemory)
        {
            if (obj.transform.CompareTag("Fox"))
            {
                print("found fox");
                Vector3 awayDir;
                awayDir = (transform.position - rb.transform.position).normalized;
                
                rb.AddForce(awayDir * maxSpeed);
                break;
                
            }
            
        }
        Task.current.Succeed();//if found no enemies
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
                Task.current.Succeed();
                target = obj.transform;
                found = true;
            }
        }

        if (found == false)
        {
            target = null;
            Task.current.Fail();
        }
    }
    
    [Task]
    void SeekTarget()
    {
        Vector3 seekDir;
        seekDir = (target.position - rb.transform.position).normalized;
                
        rb.AddForce(seekDir * maxSpeed);
        Task.current.Succeed();//if found no enemies
    }
    
    [Task]
    void AttackTarget()
    {
        if (target.GetComponent<Rigidbody>() && Vector3.Distance(target.position,rb.transform.position)<attackRange)//if has health
        {
            Rigidbody otherRb = target.GetComponent<Rigidbody>();
            Vector3 attackDir;
            attackDir = (target.position - rb.transform.position).normalized;
                
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
        if (target.GetComponent<Rigidbody>())
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
