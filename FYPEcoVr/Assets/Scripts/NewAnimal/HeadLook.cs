using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLook : MonoBehaviour
{
    public Rigidbody rb;

    public float speed=3;

    public float maxTurnAngle = 50;

    private Vector3 bodyDir;
    private Vector3 toDir;
    public Transform objToLookAt;
    public AnimalBehaviours behaviourTargeting;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponentInParent<Rigidbody>();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, bodyDir);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, toDir);
    }

    // Update is called once per frame
    void Update()
    {/*
        if (rb.velocity.magnitude>.1f)
        {
            Quaternion rotation = Quaternion.LookRotation(rb.velocity, rb.transform.up);//look to velocity, align with ground
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed*Time.deltaTime);
            lockRotation(maxTurnAngle);
        }
        */
        //lockRotation(maxTurnAngle);
        //print(rb.transform.forward);
        
        
        if (behaviourTargeting.toTarget != null)
        {
            objToLookAt = behaviourTargeting.toTarget;
            restrictLookForward();

        }
    }

    public void lockRotation(float maxAngle)
    {
        Vector3 curRot = transform.localEulerAngles;
        Vector3 newRot;
        
        newRot.x = Mathf.Clamp(curRot.x, -maxAngle, maxAngle);
        newRot.y = Mathf.Clamp(curRot.y, -maxAngle, maxAngle);
        newRot.z = Mathf.Clamp(curRot.z, -maxAngle, maxAngle);
        transform.localRotation = Quaternion.Euler (newRot.x, newRot.y, newRot.z);
    }

    public void restrictLookForward()
    {
        toDir = objToLookAt.transform.position-transform.position;
        bodyDir = rb.transform.forward;
        //print("bodyDir"+bodyDir);
        //print("toDir"+toDir);
        float angleToTarget = Vector3.Angle(bodyDir, toDir);
//        print(angleToTarget);

        Vector3 direction;
        transform.up = rb.transform.up;
        
        
        if (angleToTarget < maxTurnAngle)
        {
            //transform.LookAt(objToLookAt.transform);
            //direction = toDir;
            //Vector3 dir = objToLookAt.transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(toDir);
            transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, 1 * Time.deltaTime );
        }
        else
        {
            //transform.forward = rb.transform.forward;
            //direction = rb.transform.forward;
            Quaternion toRotation = Quaternion.LookRotation(rb.transform.forward);
            transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, 1 * Time.deltaTime );
        }
        
        //Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
        //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.time);
        
        //float angleToTarget = Vector3.Angle(toSun, toPlayer);
    }
}
