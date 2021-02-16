using System;
using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;

public class AnimalFeetPositioner : MonoBehaviour
{
    //todo instead of a raycast obj use a stride length and side stride length and get raycast start position based on that
    
    //todo add check to make sure the other leg is grounded so only one leg is lifted at a time
    
    public GameObject footIKTargetObj;//the objects that the foot will stretch to, these are in the animal container not parented to animal itself
    //public GameObject tempNextPosObj;//point for moving the target foot positions to, allows lerping feat
    public GameObject endBoneObj;//bone at foot position or object that is child at foot position. Not always bone with Ik script so check parents
    public float forwardStepDist = 1.4f;//todo set dynamically
    public float sideStepDist = .6f;
    public Vector3 nextFootPos;
    public float lerpSpeed = 15;
    public bool hasOffset = false;
    //public float timeOffset = 0.0f;
    public GameObject forwardFacingObj;//for using for forward direction of whole animal
    public float footHeightMult = 2;
    public bool footAtPosition = true;
    public AnimalFeetPositioner otherFootRaycastPositioner;
    public float extraSpace = 0.1f;
    //public float footOnGroundDist = 0.2f;
    public float footMoveStopDist = 0.01f;

    private int layerMask;//Mask for choosing what layer the raycast hits
    
    public Rigidbody rb;
    public bool debug = false;
    public float animalHeight = 1;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        //animalObj = this.GetComponentInParent<CreatureStats>().gameObject;//get the first obj going up in hierarchy with animal stats script
        transform.rotation = forwardFacingObj.transform.rotation;
        animalHeight = GetComponentInParent<AnimalBrain>().animalHeight;
        forwardStepDist = animalHeight / 2;

        endBoneObj.GetComponentInParent<FastIKFabric>().Target = footIKTargetObj.transform;

        //just shoot rays to figure out where to put feet at start
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position+ (forwardFacingObj.transform.up*20), -forwardFacingObj.transform.up, out hit, 30,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            Debug.DrawLine(transform.position, -forwardFacingObj.transform.up * hit.distance, Color.red);//For testing
            footIKTargetObj.transform.position = hit.point;
            nextFootPos = hit.point;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //todo make the lerpspeed and stepdistance increase along with movespped, if animal is running they stides get bigger and feet are faster
 
        Vector3 axisDifferences = this.transform.InverseTransformPoint(footIKTargetObj.transform.position);
        
        Vector3 raycastStart = transform.position;
        //Check if foot has got too far from desired position 
        if (axisDifferences.z > forwardStepDist +extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            raycastStart = raycastStart + (forwardFacingObj.transform.forward*-(forwardStepDist/4));
            GetDesiredFootPosition(raycastStart);
        }
        else if (axisDifferences.z < -forwardStepDist -extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            raycastStart = raycastStart + (forwardFacingObj.transform.forward*(forwardStepDist/4));//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)
            GetDesiredFootPosition(raycastStart);
        }
        if(axisDifferences.x > sideStepDist+extraSpace)//Double check overall distance instead of just forward and sides
        {
            raycastStart = raycastStart + (forwardFacingObj.transform.right*-(sideStepDist/4));//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)
            GetDesiredFootPosition(raycastStart);
        }
        else if(axisDifferences.x < -sideStepDist-extraSpace)//Double check overall distance instead of just forward and sides
        {
            raycastStart = raycastStart + (forwardFacingObj.transform.right*(sideStepDist/4));//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)
            GetDesiredFootPosition(raycastStart);
        }

        
        
        float distToNext = Vector3.Distance(footIKTargetObj.transform.position, nextFootPos);
        //if foot too far away from where it should be then move closer
        if (distToNext > footMoveStopDist) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            float footLift = footHeightMult * (distToNext / 3);
            
            footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position

            //only move foot if the other food is grounded or both feet had a problem and arent grounded
            if (otherFootRaycastPositioner.footAtPosition == true || (otherFootRaycastPositioner.footAtPosition == false && this.footAtPosition == false))
            {
                float footMoveSpeed = Mathf.Max(lerpSpeed,distToNext*lerpSpeed);
                footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos+(forwardFacingObj.transform.up*footLift), footMoveSpeed * Time.deltaTime);
            }
            
        }
        else
        {
            footAtPosition = true;
        }
        
        //if theres a porblem causing leg to be very far then fix
        if (Vector3.Distance(footIKTargetObj.transform.position, nextFootPos) > 5) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos, 20);
        }
    }

    public void GetDesiredFootPosition(Vector3 raycastStart)
    {
        bool gotNewPos = false;
        
        
        RaycastHit hit;//hit informationf
        if (Physics.Raycast(raycastStart+ (forwardFacingObj.transform.up*5), -forwardFacingObj.transform.up, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            if (debug)
            {
                Debug.DrawRay(raycastStart, -forwardFacingObj.transform.up, Color.green);
                print(hit.point);
            }
            nextFootPos = hit.point;
            gotNewPos = true;
        } 
        
        //Set a default to straigth below if couldnt find foot pos
        if (gotNewPos == false)
            nextFootPos = forwardFacingObj.transform.position-(forwardFacingObj.transform.up * 20);

        nextFootPos = nextFootPos + (forwardFacingObj.transform.up * 0.1f);//put alightly above new point
    }

    
}
