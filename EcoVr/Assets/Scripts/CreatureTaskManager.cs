using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreatureTaskManager : MonoBehaviour
{
    public CreatureStats creatureStats;
    
    public String currentTask = null;//For tracking in game what a creature is doing
    public String nextTask = null;//For tracking what creature will do after current task

    public Vector3 targetPos;//pos of current task target
    
    public GameObject food;
    public GameObject nest;
    public float moveSpeed = 5;
    public float rotSpeed = 50;
    public float wanderRadius = 5;

    public GameObject targetTestObj;

    void Update()
    {
        
        if (Vector3.Distance(this.transform.position, targetPos) > .5f)//If over a certain distance then keep moving towards it
        {
            Vector3 relativePos = targetPos - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, rotSpeed * Time.deltaTime );
            
            
            transform.Translate(Vector3.forward * Time.deltaTime);

            if (targetTestObj != null)
                targetTestObj.transform.position = targetPos;
        }
        else
        {
            currentTask = null;
            GetNewTask();
        }
    }

    void GetNewTask()
    {
        Debug.Log("Get new task");
        //have a list of tasks and it checks the next one
        if (nextTask == "Eat")
        {
            Debug.Log("Eat");
            nextTask = null;//Clear next task so it can be writen to by something else
            currentTask = "Eat";
            targetPos = food.transform.position;

            creatureStats.hunger = 100;
            //todo FindFood()
        }
        else//else just get a random position to wander to until it gets a target
        {
            currentTask = "Wander";
            targetPos = transform.position + new Vector3(Random.Range(-wanderRadius, wanderRadius), 0, Random.Range(-wanderRadius, wanderRadius));
        }
    }
}
