using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)),RequireComponent(typeof(Collider))]
public class GravityAIMovement : MonoBehaviour
{
    public GameObject core;
    public Rigidbody rb;
    public float gravityStrength = 100;
    public Vector3 groundNormal;
    public float moveSpeed = 50;
    public GameObject target;

    public bool hasPlanetGravity = true;
    public bool hasUpForce = true;
    public float upMultiplier = 2f;
    public Vector3 gravityDir;
    
    // Start is called before the first frame update
    void Start()
    {
        hasPlanetGravity = true;
        if(core == null)
            core = GameObject.Find("Core");
        //core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        moveSpeed = 0.02f;//temp, assigned by stats in future
        gravityStrength = 200;
        upMultiplier = 30;


        rb.mass = 10;
        rb.drag = 2;
        rb.angularDrag = 2;
        rb.useGravity = false;
        rb.isKinematic = false;//true for testing but normally false
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*

        //Get ground position
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -transform.up, out hit, 100))
        {
            Debug.DrawRay(transform.position, -transform.up, Color.green);
            groundNormal = hit.normal;
        }

*/
        //Add gravity down
        if (hasPlanetGravity)
        {
            gravityDir = (transform.position - core.transform.position).normalized;

            if (hasUpForce)
            {
                ApplyUpForce();
            }
            rb.AddForce(gravityDir * -gravityStrength);

            if (target != null && Vector3.Distance(this.transform.position, target.transform.position) > 4.0f)
            {
                if (hasPlanetGravity)
                {
                    transform.LookAt(target.transform, gravityDir);
                    MoveForward();
                }
            }
            else
            {
                OrientWithoutTarget();
            }
        }
    }

    public void MoveForward()
    {
        
        transform.Translate(0,0,moveSpeed);
    }

    public void ApplyUpForce()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, -gravityDir, out hit,50f))
        {
            float upForce = 0;
            upForce = Mathf.Abs(1 / (hit.point.y - transform.position.y));
            rb.AddForceAtPosition(gravityDir * upForce*upMultiplier,transform.position,ForceMode.Acceleration);

        }
    }

    
    public void OrientWithoutTarget()
    {
        //Get ground position
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            Debug.DrawRay(transform.position, -transform.up, Color.green);
            groundNormal = hit.normal;
            
            Quaternion toRotation = Quaternion.FromToRotation(transform.up,groundNormal)*transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation,toRotation,1f);
        }
    }
    
}
