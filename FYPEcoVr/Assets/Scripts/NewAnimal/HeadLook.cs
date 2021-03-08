using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLook : MonoBehaviour
{
    public Rigidbody rb;

    public float speed=3;

    public float maxTurnAngle = 65;

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
    {
        if (behaviourTargeting.toTarget != null)
        {
            objToLookAt = behaviourTargeting.toTarget;
            RestrictedLookAt();

        }
    }

    public void RestrictedLookAt()
    {
        toDir = objToLookAt.transform.position-transform.position;
        
        bodyDir = rb.transform.forward;
        float angleToTarget = Vector3.Angle(bodyDir, toDir);
        
        if (angleToTarget < maxTurnAngle)
        {
            Quaternion toRotation = Quaternion.LookRotation(toDir,rb.transform.up);//Look at direction relative to body up
            transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, 1 * Time.deltaTime );
        }
        else
        {
            Quaternion toRotation = Quaternion.LookRotation(rb.transform.forward,rb.transform.up);
            transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, 1 * Time.deltaTime );
        }
    }
}
