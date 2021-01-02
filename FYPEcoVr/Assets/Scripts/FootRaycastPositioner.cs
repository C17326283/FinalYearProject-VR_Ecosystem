using System;
using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;

public class FootRaycastPositioner : MonoBehaviour
{
    //todo instead of a raycast obj use a stride length and side stride length and get raycast start position based on that
    
    //todo add check to make sure the other leg is grounded so only one leg is lifted at a time
    
    public GameObject footIKPositionObj;//the objects that the foot will stretch to, these are in the animal container not parented to animal itself
    public GameObject tempNextPosObj;//point for moving the target foot positions to, allows lerping feat
    public GameObject endBoneObj;//bone at foot position or object that is child at foot position. Not always bone with Ik script so check parents
    public float forwardStepDist = 2;
    public float sideStepDist = .2f;
    public Vector3 nextFootPos;
    public float lerpSpeed = 15;
    public bool hasOffset = false;
    public float timeOffset = 0.0f;
    public GameObject animalObj;//for using for forward direction of whole animal
    public float footHeightMult = 1;
    public bool footAtPosition = true;
    public FootRaycastPositioner otherFootRaycastPositioner;
    public float extraSpace = 0.1f;
    public float footOnGroundDist = 0.2f;
    public float footMoveStopDist = 0.1f;

    private int layerMask;//Mask for choosing what layer the raycast hits
    
    public Rigidbody rb;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        animalObj = this.GetComponentInParent<CreatureStats>().gameObject;//get the first obj going up in hierarchy with animal stats script
        transform.rotation = animalObj.transform.rotation;

        //Make target if none exists, better to do this way anyways
        if (footIKPositionObj == null)
        {
            footIKPositionObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            footIKPositionObj.transform.name = "FootTargetObj";
            footIKPositionObj.transform.localScale = footIKPositionObj.transform.localScale * 0.01f;
            footIKPositionObj.transform.parent = transform.root; //Set it to highest level parent as they need to move independently
        }

        endBoneObj.GetComponentInParent<FastIKFabric>().Target = footIKPositionObj.transform;
        

        //just shoot rays to figure out where to put feet at start
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position+new Vector3(0,20,0), Vector3.down, out hit, 30,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            Debug.DrawLine(transform.position, Vector3.down * hit.distance, Color.red);//For testing
            footIKPositionObj.transform.position = hit.point;
            nextFootPos = hit.point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //todo make the lerpspeed and stepdistance increase along with movespped, if animal is running they stides get bigger and feet are faster
        footIKPositionObj.transform.rotation = animalObj.transform.rotation;
        
        //find differences on specidic axis
        Vector3 axisDifferences = this.transform.InverseTransformPoint(endBoneObj.transform.position);
        
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
        if (Vector3.Distance(footIKPositionObj.transform.position, nextFootPos) > footMoveStopDist) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            float footLift = footHeightMult * (Vector3.Distance(footIKPositionObj.transform.position, nextFootPos) /3);
//            print(nextFootPos.y+" , "+footLift+""+(footIKPositionObj.transform.up*footLift));

            //only move foot if the other food is grounded or both feet had a problem and arent grounded
            if (otherFootRaycastPositioner.footAtPosition == true || (otherFootRaycastPositioner.footAtPosition == false && this.footAtPosition == false))
            {
                footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position
                footIKPositionObj.transform.position = Vector3.MoveTowards( footIKPositionObj.transform.position, nextFootPos+(footIKPositionObj.transform.up*footLift), lerpSpeed * Time.deltaTime);
            }
        }
        else
        {
            footAtPosition = true;
        }
        
        //if theres a porblem causing leg to be very far then fix
        if (Vector3.Distance(footIKPositionObj.transform.position, nextFootPos) > 5) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            footIKPositionObj.transform.position = Vector3.MoveTowards( footIKPositionObj.transform.position, nextFootPos, 20);
        }
    }

    public void GetDesiredFootPosition(string axis)
    {
        if (axis == "Forward")//move back
        {
            //position the raycast start in the opposite direction of where foot currently is so get a balanced middle
            Vector3 raycastStart = transform.position + (transform.forward*-(forwardStepDist/4)) + new Vector3(0,5,0);//put x distance behind,
            
            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
                //todo find a way to allow it to use mid/corner points not just exact positions of forward back left right. ie move forward left
            } 
            if (tempNextPosObj != null)
                tempNextPosObj.transform.position = raycastStart;
        }
        else if (axis == "Behind")
        {
            Vector3 raycastStart = transform.position +(transform.forward*(forwardStepDist/4))+ new Vector3(0,5,0);//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)

            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
            } 
        }
        if (axis == "Left")
        {
            Vector3 raycastStart = transform.position +(transform.right*(sideStepDist/4))+ new Vector3(0,5,0);//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)

            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
            } 
        }
        else if (axis == "Right")//move left
        {
            Vector3 raycastStart = transform.position +(transform.right*-(sideStepDist/4))+ new Vector3(0,5,0);//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)

            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
            } 
        }
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