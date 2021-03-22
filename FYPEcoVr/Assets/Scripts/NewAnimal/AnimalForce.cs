using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This script is meant to compress all animal physics updates down to one per update
 excluding impulse forces from attack or actions
 */
public class AnimalForce : MonoBehaviour
{

    
    public Vector3 forceToApply;
    public Rigidbody rb;
    public AnimalBrain brain;
    public bool isPanicked;
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, rb.transform.InverseTransformDirection(forceToApply.normalized));
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyForce();
    }

    public void AddToForce(Vector3 forceToAdd)
    {
        forceToApply = forceToApply + forceToAdd;
    }

    public void ApplyForce()
    {
        
        Vector3 force = forceToApply.normalized * brain.moveSpeed;
        
        if (isPanicked)
        {
            force = force*3;
            rb.AddRelativeForce(force* (Time.deltaTime * 100));
        }
        else
        {
            rb.AddRelativeForce(force * (Time.deltaTime * 100));
        }

        isPanicked = false;
        forceToApply = Vector3.zero;
    }
}
