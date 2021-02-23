using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    public GameObject target;

    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = target.transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
