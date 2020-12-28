using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRaycastPositioner : MonoBehaviour
{
    public GameObject headObj;//for positioning head so body can follow with spine animator, also need to child positioning obj to head if using that
    public GameObject bodyObj;
    public GameObject positionerMeasuringObj;//This is the point for measuring dist from ground//put at front of biggest mass
    public GameObject backRotFixingObj;//This is the point for measuring dist from ground//put at front of biggest mass
    public float heightDistance = 2;
    public float wiggleRoom = .01f;//Small area where the height isnt moved up or down everyfram if havent moved too much
    public float lerpSpeed = 1f;
    public float raydistance = 100;
    public float gravityRayDistance = 1000;//
    public float levelingRotIncrements = 0.1f;
    private Rigidbody rb;

    private int layerMask;//Mask for choosing what layer the raycast hits
    // Start is called before the first frame update
    void Start()
    {
        bodyObj = this.gameObject;
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8

        //making an object to check if the back of the animal is level with the front
        if (backRotFixingObj == null)
        {
            backRotFixingObj = Instantiate(positionerMeasuringObj);
            backRotFixingObj.transform.parent = positionerMeasuringObj.transform.parent;
            backRotFixingObj.transform.position = new Vector3(positionerMeasuringObj.transform.position.x,positionerMeasuringObj.transform.position.y,positionerMeasuringObj.transform.position.z - 2);
        }
        
        
        RaycastHit hit;
        if (Physics.Raycast(positionerMeasuringObj.transform.position, Vector3.down, out hit, 100, layerMask))
        {
            //bodyObj.transform.position = hit.point; //+ new Vector3(0, 2, 0);//place a little above the ground point
            bodyObj.transform.position = new Vector3(bodyObj.transform.position.x, hit.point.y+heightDistance, bodyObj.transform.position.z);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        //Todo have an isGroundeda system so the animal can fall convincingly
        RaycastHit hit;
        if (Physics.Raycast(positionerMeasuringObj.transform.position, -transform.up, out hit, raydistance,layerMask))
        {
            Vector3 transformPosition = bodyObj.transform.position;
            if (hit.distance != heightDistance)//is too low so go higher
            {
                bodyObj.transform.position = Vector3.Lerp(bodyObj.transform.position, new Vector3(transformPosition.x, hit.point.y+heightDistance, transformPosition.z), lerpSpeed*Time.deltaTime);
            }
            
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
        }
    }
    
    
}
