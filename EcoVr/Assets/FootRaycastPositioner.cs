using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootRaycastPositioner : MonoBehaviour
{
    public GameObject footPosObj;
    public float stepDist = 5;
    public Vector3 nextFootPos;
    public float lerpSpeed = 15;

    private int layerMask;//Mask for choosing what layer the raycast hits
    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        
        //just shoot rays to figure out where to put feet at start
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            footPosObj.transform.position = hit.point;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);//For testing
            //If creature has moved far enough then make a new position the foot should be
            if (Vector3.Distance(footPosObj.transform.position, hit.point) > stepDist
            ) //If distance of current footpos and rayhit is over stepdistance then take a step
            {
//                Debug.Log("nextFootPos"+nextFootPos);
                nextFootPos = hit.point;
            }

        }
        
        //If the position the foot should be is not where the foot should be then lepr between
        if(Vector3.Distance(footPosObj.transform.position, nextFootPos) > 0.1f)//If distance of current footpos and rayhit is over stepdistance then take a step
            footPosObj.transform.position = Vector3.Lerp(footPosObj.transform.position, nextFootPos, lerpSpeed* Time.deltaTime);
    }
    
    
}
