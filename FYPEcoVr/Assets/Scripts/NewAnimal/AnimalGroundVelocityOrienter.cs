using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;

public class AnimalGroundVelocityOrienter : MonoBehaviour
{
    public bool initialiseOnStart = false;
    public AnimalBrain brain;

    public GameObject core;
    public Vector3 gravityDir;
    public Rigidbody rb;
    public float turnSpeed = 3;
    
    private int layerMask;

    public Vector3 offset;

    
    public Vector3 moveVel;//for velocity without the up and down force
    public  GameObject orienter;
    public Collider col;


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
        if(initialiseOnStart)
            Initialize();
    }

    public void Initialize()
    {
        //Get center of planet for orienting to
        if(core == null)
            core = GameObject.Find("Core");
        
        rb = GetComponent<Rigidbody>();
        layerMask = 1 << 8;//bit shift to get mask for raycasting so only on environment and not other animals
        
//        print("brain"+brain.moveSpeed);
        

        gravityDir = (core.transform.position - transform.position).normalized;
        //transform.up = -gravityDir;
        
        
        //Todo find more efficient way than adding another obj
        orienter = new GameObject("orienter");
        orienter.transform.position = this.transform.position;
        orienter.transform.rotation = this.transform.rotation;
        orienter.transform.parent = this.transform;
        col = GetComponent<CapsuleCollider>();

        offset = transform.position - col.bounds.center; 
    }

    public void AimToVelOrientedToGround()
    {
        turnSpeed = Mathf.Max(5-brain.animalHeight,1f);//the bigger animals turn slower but keep in range
        //this took about a week to find a solution to but it allows gravity without messing up the targetting
        //convert velocity to local then remove the y so can have gravity without it focing animal to look up and down
        RaycastHit hit;
        if (Physics.Raycast(rb.transform.position+(transform.up*3), gravityDir, out hit, 100,layerMask))
        {
            //Debug.DrawLine(transform.position, hit.point, Color.black);
            orienter.transform.up = hit.normal;//todo have a max angle

            Vector3 locVel = orienter.transform.InverseTransformDirection(rb.velocity);//Find velocity in relation to an object oriented to ground
            locVel.y = locVel.y*0.01f;//cancel out vertical force but animates better with a small bit
            moveVel = orienter.transform.TransformDirection(locVel);//set the new cancelled related velocity
            
            Quaternion rotation;
            if (locVel.magnitude>.7f)//Normal speed
            {
                //look to velocity, align with ground
                rotation = Quaternion.LookRotation(moveVel, orienter.transform.up);
                //do it slowly over time
                rb.transform.rotation = Quaternion.Slerp(col.transform.rotation, rotation, (turnSpeed)*Time.deltaTime);
            }
            else if (locVel.magnitude>.01f)//moving very slowly
            {
                rotation = Quaternion.LookRotation(transform.forward, hit.normal);//look to velocity, align with ground
                rb.transform.rotation = Quaternion.Slerp(col.transform.rotation, rotation, (Mathf.Max(1,turnSpeed/2))*Time.deltaTime);
            }
            else//Barely moving at all so stop from spinning from small magnitude
            {
                rotation = Quaternion.LookRotation(transform.forward, hit.normal);//look to velocity, align with ground
                rb.transform.rotation = Quaternion.Slerp(col.transform.rotation, rotation, .0001f*Time.deltaTime);
            }
        }
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (core != null) //todo optimise
        {
            gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir

            AimToVelOrientedToGround();
        }
    }
}
