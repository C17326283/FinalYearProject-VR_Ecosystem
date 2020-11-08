using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRaycastPositioner : MonoBehaviour
{
    public GameObject bodyObj;
    public GameObject positionerMeasuringObj;//This is the point for measuring dist from ground//put at front of biggest mass
    public float heightDistance = 2;
    public float wiggleRoom = .01f;//Small area where the height isnt moved up or down everyfram if havent moved too much
    public float lerpSpeed = 1f;
    public float raydistance = 100;
    public float gravityRayDistance = 1000;//
    private Rigidbody rb;

    private int layerMask;//Mask for choosing what layer the raycast hits
    // Start is called before the first frame update
    void Start()
    {
        bodyObj = this.gameObject;
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        
        
        RaycastHit hit;
        if (Physics.Raycast(positionerMeasuringObj.transform.position, Vector3.down, out hit, 100, layerMask))
        {
            bodyObj.transform.position = hit.point; //+ new Vector3(0, 2, 0);//place a little above the ground point
        }

        //rb = transform.parent.gameObject.AddComponent<Rigidbody>();
    }
    

    // Update is called once per frame
    void Update()
    {
        //Todo have an isGrounded system so the animal can fall convincingly
        RaycastHit hit;
        if (Physics.Raycast(positionerMeasuringObj.transform.position, Vector3.down, out hit, raydistance,layerMask))
        {
            Vector3 transformPosition = bodyObj.transform.position;
            if (hit.distance != heightDistance)//is too low so go higher//+wiggleRoom || hit.distance > heightDistance-wiggleRoom
            {
//                Debug.Log("go higher");
                bodyObj.transform.position = Vector3.Lerp(bodyObj.transform.position, new Vector3(transformPosition.x, hit.point.y+heightDistance, transformPosition.z), lerpSpeed*Time.deltaTime);
            }
//            if(rb.useGravity == true)
               // rb.useGravity = false;
        }
        else if (Physics.Raycast(positionerMeasuringObj.transform.position, Vector3.down, out hit, gravityRayDistance,layerMask))//
        {
       //     rb.useGravity = true;
        }
//        Debug.Log("body pos ray dist: "+hit.distance);
    }
    
    
}
