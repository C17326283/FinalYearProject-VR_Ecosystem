using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)),RequireComponent(typeof(Collider))]
public class TestBetterGravity : MonoBehaviour
{
    public GameObject core;
    public Rigidbody rb;
    public float gravityStrength = 10000;
    public Vector3 groundNormal;
    public float moveSpeed = 50;
    public GameObject target;

    public bool hasPlanetGravity = true;
    public bool hasUpForce = true;
    public float upBaseAmount = 100f;
    public float upMultiplier = 16f;
    public Vector3 gravityDir;
    public float stoppingDistance = 8;
    public float animalHeight;
    public float desiredHeight;

    public Transform point1;
    public Transform point2;
    public Transform[] forcePoints;
    
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
        gravityStrength = 100;


        rb.mass = 10;
        rb.drag = 1;
        rb.angularDrag = 1;

        desiredHeight = animalHeight * 0.8f;
        
        rb.useGravity = false;
        rb.isKinematic = false;//true for testing but normally false
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hasPlanetGravity)
        {
            gravityDir = (transform.position - core.transform.position).normalized;//todo flip dir

            
            rb.AddForce(gravityDir * (-gravityStrength));
            
            if (hasUpForce)
            {
                ApplyUpForce();
            }
            
            //rb.AddForce(gravityDir * (-gravityStrength * Time.deltaTime));
            




            if (target != null && Vector3.Distance(this.transform.position, target.transform.position) > stoppingDistance)
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

        foreach (var point in forcePoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(point.position, -gravityDir, out hit,desiredHeight))
            {
                float upForce = 0;
                upForce = Mathf.Abs(1 / ((hit.point.y - point.position.y)));
                rb.AddForceAtPosition(gravityDir * (upForce * upMultiplier * animalHeight), point.position, ForceMode.Acceleration);
            }
        }
        
    }

    
    public void OrientWithoutTarget()
    {
        //Get ground position
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            Debug.DrawRay(transform.position, -transform.up, Color.gray);
            groundNormal = hit.normal;
            
            Quaternion toRotation = Quaternion.FromToRotation(transform.up,groundNormal)*transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation,toRotation,1f);
        }
    }
    
}
