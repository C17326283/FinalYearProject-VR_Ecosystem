using System;
using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;
using Random = UnityEngine.Random;

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
    //public float timeOffset = 0.0f;
    public GameObject forwardFacingObj;//for using for forward direction of whole animal
    public float footHeightMult = 0.6f;
    public bool footAtPosition = true;
    public AnimalFeetPositioner otherFootRaycastPositioner;
    public float extraSpace = 0.1f;
    //public float footOnGroundDist = 0.2f;
    public float footMoveStopDist = 0.2f;

    private int layerMask;//Mask for choosing what layer the raycast hits
    
    public Rigidbody rb;
    public bool debug = true;
    public AnimalBrain brain;
    public float animalHeight = 1;
    public float animalLength = 1;
    public float footSpeed = 2;

    public GameObject nextPosRaycaster;
    public bool needToMove = false;

    public float distToNext;

    public Vector3 axisDifferences;
    public AnimalAudioManager audioManager;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        nextPosRaycaster = new GameObject("nextPosRaycaster");
        nextPosRaycaster.transform.parent = transform;
        nextPosRaycaster.transform.position = transform.position + (forwardFacingObj.transform.up * 2);
        
        
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        //animalObj = this.GetComponentInParent<CreatureStats>().gameObject;//get the first obj going up in hierarchy with animal stats script
        transform.rotation = forwardFacingObj.transform.rotation;
        if (brain != null)
        {
            animalHeight = brain.animalHeight;
            animalLength = brain.animalLength;
        }
        forwardStepDist = animalLength / 7f;
        sideStepDist = forwardStepDist / 4f;
        

        endBoneObj.GetComponentInParent<FastIKFabric>().Target = footIKTargetObj.transform;

        //just shoot rays to figure out where to put feet at start
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position+ (forwardFacingObj.transform.up*20), -forwardFacingObj.transform.up, out hit, 30,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
//            Debug.DrawLine(transform.position, -forwardFacingObj.transform.up * hit.distance, Color.red);//For testing
            footIKTargetObj.transform.position = hit.point;
            nextFootPos = hit.point;
        }
//        print("foot setup");
    }

    private void OnEnable()
    {
        if(footIKTargetObj!=null)
            footIKTargetObj.transform.position = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        //Really fast animals feet cant quite reach the target, i assume floating point errors, so this fixes but changing how close it needs to get
        footMoveStopDist = extraSpace+((rb.velocity.magnitude/50)*(1+animalLength));

        axisDifferences = this.transform.InverseTransformPoint(footIKTargetObj.transform.position);
        footIKTargetObj.transform.rotation = forwardFacingObj.transform.rotation;//this prevents the feet from beign twisted

        //Get a longer forward step distance if doing faster
        float velForwardStep = Mathf.Max(forwardStepDist,(forwardStepDist * rb.velocity.magnitude*0.8f)/3f);
        velForwardStep = Mathf.Clamp(velForwardStep, forwardStepDist, animalLength*0.8f);
        
        
        //Check if foot has got too far from desired position 
        if (axisDifferences.z > velForwardStep +extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            //reset x when moveing forward so not too sideways but 
            nextPosRaycaster.transform.position = transform.position-(forwardFacingObj.transform.forward*velForwardStep);
            needToMove = true;
        }
        else if (axisDifferences.z < -velForwardStep -extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            nextPosRaycaster.transform.position = transform.position+(forwardFacingObj.transform.forward*velForwardStep);
            needToMove = true;
        }
        
        
        if(axisDifferences.x > sideStepDist+extraSpace)
        {
            nextPosRaycaster.transform.position = transform.position-(forwardFacingObj.transform.right*sideStepDist);
            needToMove = true;
        }
        else if(axisDifferences.x < -sideStepDist-extraSpace)
        {
            nextPosRaycaster.transform.position = transform.position+(forwardFacingObj.transform.right*sideStepDist);
            needToMove = true;
        }
        

        if (needToMove)
        {
            GetDesiredFootPosition(nextPosRaycaster.transform.position);
            
            //IF not at next positione
            distToNext = Vector3.Distance(footIKTargetObj.transform.position, nextFootPos);

            //foot isnt at target position yet
            if (distToNext>footMoveStopDist)
            {
                //How fast to move foot
                float footMoveSpeed = Mathf.Max(rb.velocity.magnitude,(distToNext*animalLength+(axisDifferences.x*5))/2)*footSpeed;//Sidestepping needs to be faster
                //footMoveSpeed = Mathf.Clamp(footMoveSpeed, 1f, 15f);//Prevent from being too fast or slow
                
//                print("footmovespeed"+footMoveSpeed);
                
                
                float footLift= (Vector3.Distance(footIKTargetObj.transform.position, nextFootPos)*footHeightMult)-footMoveStopDist;
                footLift= Mathf.Clamp(footLift,0f,(animalHeight/rb.velocity.magnitude)*3);

                //only move foot if the other food is grounded or both feet had a problem and arent grounded
                if (otherFootRaycastPositioner.footAtPosition == true  || distToNext>(velForwardStep+extraSpace)*2 || (otherFootRaycastPositioner.footAtPosition == false && this.footAtPosition == false))
                {
                    footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position
                    //float footMoveSpeed = Mathf.Max(lerpSpeed,(distToNext/3)*lerpSpeed);
                    footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos+(forwardFacingObj.transform.up*footLift), footMoveSpeed * Time.deltaTime);//+(forwardFacingObj.transform.up*footLift)
                }
                else if(Mathf.Abs(axisDifferences.z)>forwardStepDist*6||Mathf.Abs(axisDifferences.x)>sideStepDist*6)
                {
                    //                   print("far foot");
                    footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position
                    footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos, footMoveSpeed*4 * Time.deltaTime);
                    
                }
                else if(Mathf.Abs(axisDifferences.z)>forwardStepDist*13||Mathf.Abs(axisDifferences.x)>sideStepDist*13)//is extremely far
                {
                    //                   print("far foot");
                    footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position
                    footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos, (footMoveSpeed+10)*200 * Time.deltaTime);
                    
                }
                
            }
            else
            {
                audioManager.playFootStep();
                needToMove = false;
                footAtPosition = true;
            }
            
        }
    }

    public void GetDesiredFootPosition(Vector3 raycastStart)
    {
        
        RaycastHit hit;//hit informationf
        if (Physics.Raycast(raycastStart+ (forwardFacingObj.transform.up*10), -forwardFacingObj.transform.up, out hit, 50,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
            if (debug)
            {
                Debug.DrawLine(raycastStart+ (forwardFacingObj.transform.up*10), hit.point, Color.yellow);
                //print(hit.point);
            }
            nextFootPos = hit.point;
        }
    }

    
}
