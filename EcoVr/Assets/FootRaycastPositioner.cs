using System;
using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;

public class FootRaycastPositioner : MonoBehaviour
{
    //todo instead of a raycast obj use a stride length and side stride length and get raycast start position based on that
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
    void LateUpdate()
    {
        //lerpSpeed = animalObj.GetComponent<CreatureStats>().moveSpeed * 2;
        //forwardStepDist = animalObj.GetComponent<CreatureStats>().moveSpeed/2;
        footIKPositionObj.transform.rotation = animalObj.transform.rotation;
        
        //find differences on specidic axis
        //Vector3 localDifferent = animalObj.transform.InverseTransformDirection(this.transform.localPosition - endBoneObj.transform.loc);
        Vector3 axisDifferences = this.transform.InverseTransformPoint(endBoneObj.transform.position);
        //tempNextPosObj.transform.position = animalObj.transform.;
        
        
            //this.transform.InverseTransformDirection(this.transform.position - endBoneObj.transform.position);//Get local differences so z is is how much further forward the ray is than foot
//        Debug.Log(axisDifferences);
        //Check if foot has got too far from desired position 
        if (axisDifferences.z > forwardStepDist) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
//            Debug.Log("too far forward");
            GetDesiredFootPosition("Forward");
        }
        else if (axisDifferences.z < -forwardStepDist) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
//            Debug.Log("too far behind");
            GetDesiredFootPosition("Behind");
        }
        else if(axisDifferences.x > sideStepDist)//Double check overall distance instead of just forward and sides
        {
//            Debug.Log("too far right");
            GetDesiredFootPosition("Right");
        }
        else if(axisDifferences.x < -sideStepDist)//Double check overall distance instead of just forward and sides
        {
//            Debug.Log("too far left");
            GetDesiredFootPosition("Left");
        }
        
        
        //If the position the foot should be is not where the foot should be then lepr between
        if (Vector3.Distance(footIKPositionObj.transform.position, nextFootPos) > 0.01f
        ) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            footIKPositionObj.transform.position = Vector3.MoveTowards( footIKPositionObj.transform.position, nextFootPos, lerpSpeed * Time.deltaTime);
            //footIKPositionObj.transform.position = Vector3.Lerp(footIKPositionObj.transform.position, nextFootPos,lerpSpeed * Time.deltaTime);
            
            float footLift = Vector3.Distance(footIKPositionObj.transform.position, nextFootPos) /3;
            footIKPositionObj.transform.position = new Vector3(footIKPositionObj.transform.position.x, nextFootPos.y+footLift,footIKPositionObj.transform.position.z );
//            Debug.Log("dist"+Vector3.Distance(footIKPositionObj.transform.position, nextFootPos)+"  po"+d);
        }
    }

    public void GetDesiredFootPosition(string axis)
    {
//        Debug.Log("amountToMove"+ 1);
        
        if (axis == "Forward")//move back
        {
            //position the raycast start in the opposite direction of where foot currently is so get a balanced middle
            Vector3 raycastStart = transform.position + (transform.forward*-(forwardStepDist/4)) + new Vector3(0,5,0);//put x distance behind,
            
            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
                //nextFootPos.x = endBoneObj.transform.position.x;
                //todo find a way to allow it to use mid/corner points not just exact positions of forward back left right. ie move forward left
                //nextFootPos.z = hit.point.z;
                //nextFootPos.y = hit.point.y;
            } 
            if (tempNextPosObj != null)
                tempNextPosObj.transform.position = raycastStart;
        }
        else if (axis == "Behind")
        {
            //position the raycast start in the opposite direction of where foot currently is so get a balanced middle
            Vector3 raycastStart = transform.position +(transform.forward*(forwardStepDist/4))+ new Vector3(0,5,0);//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)

            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
                //nextFootPos.x = endBoneObj.transform.position.x;
            } 
        }
        if (axis == "Left")
        {
            //position the raycast start in the opposite direction of where foot currently is so get a balanced middle
            Vector3 raycastStart = transform.position +(transform.right*(sideStepDist/4))+ new Vector3(0,5,0);//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)

            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
                //nextFootPos.z = endBoneObj.transform.position.z;
            } 
        }
        else if (axis == "Right")//move left
        {
            //position the raycast start in the opposite direction of where foot currently is so get a balanced middle
            Vector3 raycastStart = transform.position +(transform.right*-(sideStepDist/4))+ new Vector3(0,5,0);//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)

            RaycastHit hit;//hit information
            if (Physics.Raycast(raycastStart, Vector3.down, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
            {
                nextFootPos = hit.point;
                //nextFootPos.z = endBoneObj.transform.position.z;
            } 
        }
//        Debug.Log("footIKPositionObj"+footIKPositionObj.transform.position);
 //       Debug.Log("nextFootPos"+nextFootPos);
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