using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRaycastPositioner : MonoBehaviour
{
    public GameObject bodyObj;
    public GameObject positionerMeasuringObj;//This is the point for measuring dist from ground//put at front of biggest mass
    public GameObject backRotFixingObj;//This is the point for measuring dist from ground//put at front of biggest mass
    public float heightDistance = 2;
    public float lerpSpeed = 1f;
    public float raydistance = 100;
    public float heightStepIncrements = 2;
    public float levelingRotIncrements = 0.1f;
    private Rigidbody rb;
    public GameObject core;
    public float force;

    private int layerMask;//Mask for choosing what layer the raycast hits
    // Start is called before the first frame update
    void Start()
    {
        heightDistance = gameObject.GetComponentInChildren<MeshRenderer>().bounds.size.y/2;
        bodyObj = this.gameObject;
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8

        if (core == null)
        {
            if (GameObject.Find("Core"))
            {
                core = GameObject.Find("Core");
            }
        }
        
        //making an object to check if the back of the animal is level with the front
        if (backRotFixingObj == null)
        {
            backRotFixingObj = Instantiate(positionerMeasuringObj);
            backRotFixingObj.transform.parent = positionerMeasuringObj.transform.parent;
            backRotFixingObj.transform.position = new Vector3(positionerMeasuringObj.transform.position.x,positionerMeasuringObj.transform.position.y,positionerMeasuringObj.transform.position.z - 2);
        }
        
        RaycastHit hit;
        if (Physics.Raycast(positionerMeasuringObj.transform.position, Vector3.down, out hit, raydistance, layerMask))
        {
            //bodyObj.transform.position = hit.point; //+ new Vector3(0, 2, 0);//place a little above the ground point
            bodyObj.transform.position = new Vector3(bodyObj.transform.position.x, hit.point.y+heightDistance, bodyObj.transform.position.z);
        }

        rb = transform.GetComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (core == null)
            flatPositioning();
        else
            CorePositioning();
    }

    void CorePositioning()
    {
        //Todo have an isGrounded system so the animal can fall convincingly
        RaycastHit hit;
        if (Physics.Raycast(positionerMeasuringObj.transform.position, core.transform.position, out hit, raydistance,layerMask))
        {
            Debug.Log("corePositioning");
            Vector3 transformPosition = bodyObj.transform.position;
            if (hit.distance != heightDistance)//is too low so go higher
            {
                bodyObj.transform.position = Vector3.Lerp(bodyObj.transform.position, new Vector3(transformPosition.x, hit.point.y+heightDistance, transformPosition.z), lerpSpeed*Time.deltaTime);
                
            }
        }
    }

    void flatPositioning()
    {/*
        RaycastHit hit;
        if (Physics.Raycast(positionerMeasuringObj.transform.position, -transform.up, out hit, heightDistance*2,
            layerMask))
        {
            //rb.useGravity = false;
            rb.isKinematic = false;
            if (hit.distance < heightDistance*2)
            {
                Debug.Log("adjust height");
                rb.AddForce(transform.up * force * Time.deltaTime);
            }

        }
        else
        {
            Debug.Log("gravity");
            rb.useGravity = true;
        }
*/
        
        //Todo have an isGroundeda system so the animal can fall convincingly
        RaycastHit hit;
        if (Physics.Raycast(positionerMeasuringObj.transform.position, -transform.up, out hit, heightDistance*2,layerMask))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            Vector3 transformPosition = bodyObj.transform.position;
            if (hit.distance != heightDistance)//is too low so go higher
            {
                bodyObj.transform.position = Vector3.Lerp(bodyObj.transform.position, new Vector3(transformPosition.x, hit.point.y+heightDistance, transformPosition.z), lerpSpeed*Time.deltaTime);
                
            }
            
            /*
            //use second raycast at back to tilt body 
            RaycastHit backHit;
            if (Physics.Raycast(backRotFixingObj.transform.position, -transform.up, out backHit, raydistance, layerMask))
            {
                if (backHit.distance > hit.distance + 0.1f) //back is too high, need to decrease x rotation//todo add dead zone
                {
                    bodyObj.transform.Rotate (-levelingRotIncrements,0,0);
                }
                else if (backHit.distance < hit.distance - 0.1f) //back is too high, need to decrease x rotation
                {
                    bodyObj.transform.Rotate (levelingRotIncrements,0,0);
                }
            }
            */
        }
        else
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        
    }
    
    
}
