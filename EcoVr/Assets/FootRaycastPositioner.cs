using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;

public class FootRaycastPositioner : MonoBehaviour
{
    //todo instead of a raycast obj use a stride length and side stride length and get raycast start position based on that
    public GameObject footTargetObj;
    public GameObject relatedBoneObj;
    public float forwardStepDist = 2;
    public float sideStepDist = .2f;
    public Vector3 nextFootPos;
    public float lerpSpeed = 15;
    public bool hasOffset = false;
    public float timeOffset = 0.0f;

    private int layerMask;//Mask for choosing what layer the raycast hits
    // Start is called before the first frame update
    void Start()
    {
        
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8

        //Make target if none exists, better to do this way anyways
        if (footTargetObj == null)
        {
            footTargetObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            footTargetObj.transform.name = "FootTargetObj";
            footTargetObj.transform.localScale = footTargetObj.transform.localScale * 0.01f;
            footTargetObj.transform.parent = transform.root; //Set it to highest level parent as they need to move independently
        }

        relatedBoneObj.GetComponent<FastIKFabric>().Target = footTargetObj.transform;
        

        //just shoot rays to figure out where to put feet at start
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            footTargetObj.transform.position = hit.point;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);//For testing
            //If creature foot has moved far enough then make a new position the foot should be//forward dist different then side dist

            Vector3 localDifferent = this.transform.InverseTransformDirection(footTargetObj.transform.position - hit.point);//Get local differences so z is is how much further forward the ray is than foot
//            Debug.Log("hitPosition: "+hit.point+" footTargetObj:"+footTargetObj.transform.position+"  Dif:"+localDifferent);
            
            //nextFootPos = hit.point;
            //todo foot on ground detection
//            Debug.Log("front dist: "+localDifferent.z+"side dist: "+localDifferent.x+"vert dist: "+localDifferent.y);
//            Debug.Log("pos: "+(forwardStepDist)+"neg: "+ (-forwardStepDist));
            //if forward step too far
            if (localDifferent.z > forwardStepDist || localDifferent.z < -forwardStepDist || localDifferent.x > sideStepDist || localDifferent.x < -sideStepDist) //If distance of current footpos and rayhit is over stepdistance then take a step
            {
                StartCoroutine(SetNextFootPos(hit.point));
            }
            else if(Vector3.Distance(footTargetObj.transform.position, hit.point) > forwardStepDist)//Double check overall distance instead of just forward and sides
            {
                StartCoroutine(SetNextFootPos(hit.point));
            }
            
            //if (footTargetObj.transform.forward.x - hit.point.x > forwardStepDist) //If distance of current footpos and rayhit is over stepdistance then take a step
            //{
//                Debug.Log("nextFootPos"+nextFootPos);
              //  nextFootPos = hit.point;
            //}
    //        if (Vector3.Distance(footTargetObj.transform.position, hit.point) > forwardStepDist) //If distance of current footpos and rayhit is over stepdistance then take a step
     //       {
//   //             Debug.Log("nextFootPos"+nextFootPos);
     //                 nextFootPos = hit.point;
     //        }

        }
        
        //If the position the foot should be is not where the foot should be then lepr between
        if(Vector3.Distance(footTargetObj.transform.position, nextFootPos) > 0.01f)//If distance of current footpos and rayhit is over stepdistance then take a step
            footTargetObj.transform.position = Vector3.Lerp(footTargetObj.transform.position, nextFootPos, lerpSpeed* Time.deltaTime);
    }

    //Function for setting next foto position considering the offset
    IEnumerator SetNextFootPos(Vector3 pos)
    {
        if (hasOffset)
        {
            yield return new WaitForSeconds(timeOffset);     
            nextFootPos = pos;
        }
        else
        {
            nextFootPos = pos;
        }
    }
    
}
