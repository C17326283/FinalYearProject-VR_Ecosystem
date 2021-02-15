using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBodyPositioner : MonoBehaviour
{

    public AnimalBrain brain;
    public float maxSpeed = 5;
    public float maxForce = 10;

    public float mass = 1;

    public GameObject core;
    public float gravityStrength = 1000;
    public bool hasUpForce = true;
    public float upMultiplier = 5f;
    public Vector3 gravityDir;
    public float animalHeight;
    public float animalLength;
    public Rigidbody rb;
    public float distToGround;
    public float desiredHeight;
    public GameObject headHeightPosObj;
    public float lerpSpeed = 3;
    
    public Vector3 vertForce;
    private int layerMask;

    public List<GameObject> forcePoints;

    public bool isGrounded = false;

    public GameObject lookPoint;
    public Vector3 moveVel;//for velocity without the up and down force


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveVel);


    }

    // Start is called before the first frame update
    void Start()
    {
        if(core == null)
            core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();
        desiredHeight = animalHeight * .8f;
        layerMask = 1 << 8;//bit shift to get mask

        rb.mass = 100;
        rb.drag = 2;
        rb.angularDrag = 2;

        lookPoint = new GameObject("fox follow");
        
        forcePoints = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            GameObject p = new GameObject();
            forcePoints.Add(p);
            p.transform.parent = headHeightPosObj.transform.parent;
        }
        //temp assume 2 points, front and back
        forcePoints[0].transform.position = headHeightPosObj.transform.position;
        forcePoints[1].transform.position = headHeightPosObj.transform.position+(-transform.forward * (animalLength));
        

    }

    //For setting the target to the a pos above terrain instead of aiming through ground
    public void TerrainPosCorrecting()
    {
        Vector3 dir =  brain.currentTarget.transform.position -transform.position;
        RaycastHit hit;
        //if objstacle in the way look above 
        if (Physics.Raycast(this.transform.position, dir, out hit, layerMask))
        {
            print("poitn above ground");
            lookPoint.transform.position = Vector3.Lerp(lookPoint.transform.position, hit.point + (-gravityDir * desiredHeight), lerpSpeed*Time.deltaTime);
            //return hit.point + (-gravityDir * desiredHeight);
        }
        else//if direct line of sight aim at position above ground
        {
            if (Physics.Raycast(this.transform.position, gravityDir, out hit, layerMask))//stop body from trying to go into the air
            {
                lookPoint.transform.position = Vector3.Lerp(lookPoint.transform.position, hit.point + (-gravityDir * desiredHeight), lerpSpeed*Time.deltaTime);
            }
        }
    }
    

    public void GravityHeightPositioning()
    {
        foreach (var point in forcePoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(point.transform.position, gravityDir, out hit, animalHeight, layerMask))
            {
                isGrounded = true;
                Debug.DrawRay(point.transform.position, gravityDir, Color.green);
                print("hit");
                float upForce = 0;
                upForce = Mathf.Abs(1 / ((hit.point.y - point.transform.position.y)));
                rb.AddForceAtPosition(-gravityDir * (upForce * upMultiplier * animalHeight),
                    point.transform.position, ForceMode.Acceleration);
            }
            else
            {
                isGrounded = false;
            }
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        gravityDir = (core.transform.position-transform.position).normalized;//todo flip dir
        
        rb.AddForce(gravityDir * (gravityStrength));
        GravityHeightPositioning();



        //Vector3 moveVel = rb.velocity;
        //moveVel += gravityDir * 0;
        
            
        //this took about a week to find a solution to but it allows gravity without messing up the targetting
        //convert velocity to local then remove the y so can have gravity without it focing animal to look up and down
        Vector3 locVel = transform.InverseTransformDirection(rb.velocity);
        locVel.y = 0;
        moveVel = transform.TransformDirection(locVel);

        //print(locVel);
        Quaternion rotation;
        if (brain.currentTarget != null)
        {
            //TerrainPosCorrecting();//moves the lookpoint to either the target or the terrain in front of it
            
            
            Vector3 dir =  lookPoint.transform.position -transform.position;
            print("look t");
            rotation = Quaternion.LookRotation(moveVel, -gravityDir);
            //rotation = Quaternion.LookRotation(locVel, -gravityDir);
            transform.rotation = rotation;
        }
        else
        {
            print("no t");
            rotation = Quaternion.LookRotation(brain.transform.position, -gravityDir);
            transform.rotation = rotation;
        }
        
    }
}
