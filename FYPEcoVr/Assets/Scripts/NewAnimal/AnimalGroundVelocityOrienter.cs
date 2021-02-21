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

    
    public Vector3 moveVel;//for velocity without the up and down force
    public  GameObject orienter;



    // Start is called before the first frame update
    void Awake()
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
        
        //If its an animal then set from settings else use default
        if (brain != null)
        {
            turnSpeed = brain.moveSpeed / 5;
        }

        print("brain"+brain.moveSpeed);

        transform.up = -gravityDir;
        
        //Todo find more efficient way than adding another obj
        orienter = new GameObject("orienter");
        orienter.transform.parent = this.transform;
        orienter.transform.position = this.transform.position;
        orienter.transform.rotation = this.transform.rotation;
    }

    public void AimToVelOrientedToGround()
    {
        //this took about a week to find a solution to but it allows gravity without messing up the targetting
        //convert velocity to local then remove the y so can have gravity without it focing animal to look up and down
        RaycastHit hit;
        if (Physics.Raycast(transform.position, gravityDir, out hit, 100,layerMask))
        {
            Debug.DrawRay(transform.position, gravityDir, Color.black);
            orienter.transform.up = hit.normal;

            Vector3 locVel = orienter.transform.InverseTransformDirection(rb.velocity);//Find velocity in relation to an object oriented to ground
            locVel.y = 0;//cancel out vertical force
            moveVel = orienter.transform.TransformDirection(locVel);//set the new cancelled related velocity
            
            Quaternion rotation;
            if (locVel.magnitude>.1f)
            {
                rotation = Quaternion.LookRotation(moveVel, orienter.transform.up);//look to velocity, align with ground
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed*Time.deltaTime);//do it over time
            }
            else
            {
                rotation = Quaternion.LookRotation(transform.forward, hit.normal);//look to velocity, align with ground
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, .0001f*Time.deltaTime);
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
