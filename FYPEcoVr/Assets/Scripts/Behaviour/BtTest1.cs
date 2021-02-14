using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Panda;//Can be used by the panda behaviour

public class BtTest1 : MonoBehaviour
{
    public bool it = true;
    
    

    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [Task]
    void Catch()
    {
            
    }

    [Task]
    bool IsChaser()
    {
        return it;
    }
    
    [Task]//set it as a task that can be seen
    void MoveAnimal()//do what ever is in this function when called from sequence until succeed then it will move on
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position,
            10 * Time.deltaTime);
        if(Vector3.Distance(transform.position,target.position)<1)
            Task.current.Succeed();
    }
    
    [Task]
    void Run()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(0,0,0), 
            10 * Time.deltaTime);
        if(Vector3.Distance(transform.position,new Vector3(0,0,0))<1)
            Task.current.Succeed();
    }
    
}
