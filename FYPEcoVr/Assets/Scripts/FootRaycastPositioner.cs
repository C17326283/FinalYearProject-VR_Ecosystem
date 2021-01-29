using System;
using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;

public class FootRaycastPositioner : MonoBehaviour
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
    public float footHeightMult = 1;
    public bool footAtPosition = true;
    public FootRaycastPositioner otherFootRaycastPositioner;
    public float extraSpace = 0.1f;
    //public float footOnGroundDist = 0.2f;
    public float footMoveStopDist = 0.1f;

    private int layerMask;//Mask for choosing what layer the raycast hits
    
    public Rigidbody rb;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        //animalObj = this.GetComponentInParent<CreatureStats>().gameObject;//get the first obj going up in hierarchy with animal stats script
        transform.rotation = forwardFacingObj.transform.rotation;

        //Make target if none exists, better to do this way anyways
        if (footIKTargetObj == null)
        {
            footIKTargetObj = new GameObject("FootTargetObj");
            //footIKTargetObj.transform.localScale = footIKTargetObj.transform.localScale * 0.01f;
            footIKTargetObj.transform.parent = transform.root; //Set it to highest level parent as they need to move independently
        }

        endBoneObj.GetComponentInParent<FastIKFabric>().Target = footIKTargetObj.transform;
        

        //just shoot rays to figure out where to put feet at start
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position+new Vector3(0,20,0), -forwardFacingObj.transform.up, out hit, 30,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            Debug.DrawLine(transform.position, -forwardFacingObj.transform.up * hit.distance, Color.red);//For testing
            footIKTargetObj.transform.position = hit.point;
            nextFootPos = hit.point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //todo make the lerpspeed and stepdistance increase along with movespped, if animal is running they stides get bigger and feet are faster
        //footIKPositionObj.transform.rotation = animalObj.transform.rotation;
        footIKTargetObj.transform.forward = forwardFacingObj.transform.forward;
        
        //find differences on specific axis
        Vector3 axisDifferences = this.transform.InverseTransformPoint(footIKTargetObj.transform.position);
        
        //Check if foot has got too far from desired position 
        if (axisDifferences.z > forwardStepDist +extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            GetDesiredFootPosition("Forward");
        }
        else if (axisDifferences.z < -forwardStepDist -extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            GetDesiredFootPosition("Behind");
        }
        else if(axisDifferences.x > sideStepDist+extraSpace)//Double check overall distance instead of just forward and sides
        {
            GetDesiredFootPosition("Right");
        }
        else if(axisDifferences.x < -sideStepDist-extraSpace)//Double check overall distance instead of just forward and sides
        {
            GetDesiredFootPosition("Left");
        }
        
        //if foot too far away from where it should be then move closer
        if (Vector3.Distance(footIKTargetObj.transform.position, nextFootPos) > footMoveStopDist) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            float footLift = footHeightMult * (Vector3.Distance(footIKTargetObj.transform.position, nextFootPos) /3);
//            print(nextFootPos.y+" , "+footLift+""+(footIKPositionObj.transform.up*footLift));

            //only move foot if the other food is grounded or both feet had a problem and arent grounded
            if (otherFootRaycastPositioner.footAtPosition == true || (otherFootRaycastPositioner.footAtPosition == false && this.footAtPosition == false))
            {
                footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position
                footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos+(footIKTargetObj.transform.up*footLift), lerpSpeed * Time.deltaTime);
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

    public void GetDesiredFootPosition(string axis)
    {
        bool gotNewPos = false;
        if (axis == "Forward")//move back
        {
            //position the raycast start in the opposite direction of where foot currently is so get a balanced middle
            Vector3 raycastStart = transform.position + (transform.forward*-(forwardStepDist/4)) + new Vector3(0,5,0);//put x distance behind,
            
            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, -forwardFacingObj.transform.up, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
                gotNewPos = true;
                //todo find a way to allow it to use mid/corner points not just exact positions of forward back left right. ie move forward left
            } 
            
            //if (tempNextPosObj != null)
            //    tempNextPosObj.transform.position = raycastStart;
        }
        else if (axis == "Behind")
        {
            Vector3 raycastStart = transform.position +(forwardFacingObj.transform.forward*(forwardStepDist/4))+ new Vector3(0,5,0);//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)

            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, -forwardFacingObj.transform.up, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
                gotNewPos = true;
            } 
        }
        if (axis == "Left")
        {
            Vector3 raycastStart = transform.position +(forwardFacingObj.transform.right*(sideStepDist/4))+ new Vector3(0,5,0);//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)

            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, -forwardFacingObj.transform.up, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
                gotNewPos = true;
            } 
        }
        else if (axis == "Right")//move left
        {
            Vector3 raycastStart = transform.position +(forwardFacingObj.transform.right*-(sideStepDist/4))+ new Vector3(0,5,0);//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)

            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, -forwardFacingObj.transform.up, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
                gotNewPos = true;
            } 
        }
        
        //Set a default to straigth below if couldnt find foot pos
        if (gotNewPos == false)
            nextFootPos = forwardFacingObj.transform.position-(forwardFacingObj.transform.up * 20);

        nextFootPos = nextFootPos + (forwardFacingObj.transform.up * 0.1f);//put alightly above new point
    }

    
}


/*
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);//For testing
            //If creature foot has moved far enough then make a new position the foot should be//forward dist different then side dist

            //Vector3 localDifferent = this.transform.InverseTransformDirection(footTargetObj.transform.position - hit.point);//Get local differences so z is is how much further forward the ray is than foot
//            Debug.Log("hitPosition: "+hit.point+" footTargetObj:"+footTargetObj.transform.position+"  Dif:"+localDifferent);
            
            //nextFootPos = hit.point;
            //todo foot on ground detection
//            Debug.Log("front dist: "+localDifferent.z+"side dist: "+localDifferent.x+"vert dist: "+localDifferent.y);
//            Debug.Log("pos: "+(forwardStepDist)+"neg: "+ (-forwardStepDist));
            //if forward step too far
            if (localDifferent.z > forwardStepDist || localDifferent.z < -forwardStepDist) //If distance of current footpos and rayhit is over stepdistance then take a step
            {
                //Todo seperate axis for forward and back
                StartCoroutine(SetNextFootPos(hit.point, "Forward"));
            }
            else if(localDifferent.x > sideStepDist || localDifferent.x < -sideStepDist)//Double check overall distance instead of just forward and sides
            {
                StartCoroutine(SetNextFootPos(hit.point, "Side"));
            }
            else if(Vector3.Distance(footTargetObj.transform.position, hit.point) > forwardStepDist)//Double check overall distance instead of just forward and sides
            {
                StartCoroutine(SetNextFootPos(hit.point, "Side"));
            }
        }
        */