using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Panda;//Can be used by the panda behaviour

public class BtTest1 : MonoBehaviour
{
    public bool it = true;

    public Transform target;
   
    [Task]
    bool IsChaser()
    {
        return it;
    }
    
    [Task]
    void CatchOther()
    {
        it = false;
        StartCoroutine("SetOtherIt");
    }

    IEnumerator SetOtherIt()
    {
        yield return new WaitForSeconds(1);
        target.GetComponent<BtTest1>().it = true;
    }
    
    
    
    
    [Task]//set it as a task that can be seen
    void Chase1()//do what ever is in this function when called from sequence until succeed then it will move on
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir * 2 * Time.deltaTime);
        if(Vector3.Distance(transform.position,target.position)<1)
            Task.current.Succeed();
    }
    
    [Task]
    void Run1()
    {
        Vector3 dir = transform.position - target.position;
        transform.Translate(dir * 1 * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.position) < 1)
        {
            Task.current.Succeed();
        }
    }
    
    [Task]//set it as a task that can be seen
    void Wander1()//do what ever is in this function when called from sequence until succeed then it will move on
    {
        Task.current.Succeed();
    }
    
}
