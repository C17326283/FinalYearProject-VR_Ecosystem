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
    public float lerpSpeed = 10;//todo set based on speed or size
    public bool hasOffset = false;
    //public float timeOffset = 0.0f;
    public GameObject forwardFacingObj;//for using for forward direction of whole animal
    public float footHeightMult = 0.5f;
    public bool footAtPosition = true;
    public AnimalFeetPositioner otherFootRaycastPositioner;
    public float extraSpace = 0.2f;
    //public float footOnGroundDist = 0.2f;
    public float footMoveStopDist = 0.2f;

    private int layerMask;//Mask for choosing what layer the raycast hits
    
    public Rigidbody rb;
    public bool debug = true;
    public float animalHeight = 1;
    public float animalLength = 1;

    public GameObject nextPosRaycaster;
    public bool needToMove = false;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        nextPosRaycaster = new GameObject("nextPosRaycaster");
        nextPosRaycaster.transform.parent = this.transform;
        nextPosRaycaster.transform.position = this.transform.position + (forwardFacingObj.transform.up * 2);
        
        
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        //animalObj = this.GetComponentInParent<CreatureStats>().gameObject;//get the first obj going up in hierarchy with animal stats script
        transform.rotation = forwardFacingObj.transform.rotation;
        forwardStepDist = animalLength / 5f;
        sideStepDist = animalLength / 10f;
        //float limbMoveSpeed = (1/animalLength);
        //print("limbMoveSpeed"+limbMoveSpeed);
        //lerpSpeed = Mathf.Max(lerpSpeed, 10, limbMoveSpeed);//have a minimum speed
        

        endBoneObj.GetComponentInParent<FastIKFabric>().Target = footIKTargetObj.transform;

        //just shoot rays to figure out where to put feet at start
        RaycastHit hit;//hit information
        if (Physics.Raycast(transform.position+ (forwardFacingObj.transform.up*20), -forwardFacingObj.transform.up, out hit, 30,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
        {
//            Debug.DrawLine(transform.position, -forwardFacingObj.transform.up * hit.distance, Color.red);//For testing
            footIKTargetObj.transform.position = hit.point;
            nextFootPos = hit.point;
        }
        print("foot setup");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //todo make the lerpspeed and stepdistance increase along with movespped, if animal is running they stides get bigger and feet are faster
 
        Vector3 axisDifferences = this.transform.InverseTransformPoint(footIKTargetObj.transform.position);
        //todo use end foot object without moveable ankle and point directly at hit point?
        footIKTargetObj.transform.forward = forwardFacingObj.transform.forward;//this prevents the feet from beign twisted

        float velForwardStep = Mathf.Max(forwardStepDist,(forwardStepDist * rb.velocity.magnitude)/4f);
        velForwardStep = Mathf.Clamp(velForwardStep, forwardStepDist, forwardStepDist * 2);
        
//        print("forwardStepDist"+forwardStepDist+"  velForwardStep"+velForwardStep);
        
        //Check if foot has got too far from desired position 
        if (axisDifferences.z > velForwardStep +extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            //reset x when moveing forward so not too sideways but 
            nextPosRaycaster.transform.position = transform.position-(forwardFacingObj.transform.forward*velForwardStep);
            needToMove = true;
//            print("too forward");
        }
        else if (axisDifferences.z < -velForwardStep -extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            nextPosRaycaster.transform.position = transform.position+(forwardFacingObj.transform.forward*velForwardStep);
            needToMove = true;
//            print("too behind");

        }
        
        if(axisDifferences.x > sideStepDist+extraSpace)//Double check overall distance instead of just forward and sides
        {
            //todo dont overwrite forward
            nextPosRaycaster.transform.position = transform.position-(forwardFacingObj.transform.right*sideStepDist);
            //nextPosRaycaster.transform.position = new Vector3((transform.position-forwardFacingObj.transform.right*sideStepDist).x,nextPosRaycaster.transform.position.y,nextPosRaycaster.transform.position.z);
            needToMove = true;
//            print("too right");

        }
        else if(axisDifferences.x < -sideStepDist-extraSpace)//Double check overall distance instead of just forward and sides
        {
            nextPosRaycaster.transform.position = transform.position+(forwardFacingObj.transform.right*sideStepDist);
            //nextPosRaycaster.transform.position = new Vector3((transform.position+forwardFacingObj.transform.right*sideStepDist).x,nextPosRaycaster.transform.position.y,nextPosRaycaster.transform.position.z);
            needToMove = true;
//            print("too left");

        }
        else
        {
            footAtPosition = true;
        }

        if (needToMove)
        {
            GetDesiredFootPosition(nextPosRaycaster.transform.position);
            
            //IF not at next position
            float distToNext = Vector3.Distance(footIKTargetObj.transform.position, nextFootPos);
//            print(distToNext);
            //foot isnt at target position yet
            if (distToNext>footMoveStopDist)
            {
//                print("move ik to next pos");
                //float footLift = footHeightMult * (Vector3.Distance(footIKTargetObj.transform.position, nextFootPos) /5);         
                
                float footLift= (Vector3.Distance(footIKTargetObj.transform.position, nextFootPos)*footHeightMult)-footMoveStopDist;
                footLift= Mathf.Clamp(footLift,0f,(animalHeight/rb.velocity.magnitude)*3);
//                print(footLift);

                //only move foot if the other food is grounded or both feet had a problem and arent grounded
                if (otherFootRaycastPositioner.footAtPosition == true  || distToNext>(velForwardStep+extraSpace)*2 || (otherFootRaycastPositioner.footAtPosition == false && this.footAtPosition == false))
                {
                    footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position
                    //float footMoveSpeed = Mathf.Max(lerpSpeed,(distToNext/3)*lerpSpeed);
                    float footMoveSpeed = Mathf.Max(lerpSpeed,(lerpSpeed * rb.velocity.magnitude)/3);
                    footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos+(forwardFacingObj.transform.up*footLift), footMoveSpeed * Time.deltaTime);//+(forwardFacingObj.transform.up*footLift)
                }
                else if(distToNext>(velForwardStep+extraSpace)*4)
                {
 //                   print("far foot");
                    footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position
                    footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos, lerpSpeed*10 * Time.deltaTime);
                }
            }
            else
            {
                needToMove = false;
//                print("needToMove false");

            }
            
        }


        /*
        float distToNext = Vector3.Distance(footIKTargetObj.transform.position, nextFootPos);
        //if foot too far away from where it should be then move closer
        if (distToNext > footMoveStopDist) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            float footLift = footHeightMult * (Vector3.Distance(footIKTargetObj.transform.position, nextFootPos) /5);            

            //only move foot if the other food is grounded or both feet had a problem and arent grounded
            if (otherFootRaycastPositioner.footAtPosition == true  || distToNext>(forwardStepDist+extraSpace)*2 || (otherFootRaycastPositioner.footAtPosition == false && this.footAtPosition == false))
            {
                footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position
                float footMoveSpeed = Mathf.Max(lerpSpeed,(distToNext/3)*lerpSpeed);
                footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos+(forwardFacingObj.transform.up*footLift), footMoveSpeed * Time.deltaTime);
            }
            else if(distToNext>(forwardStepDist+extraSpace)*4)
            {
                print("far foot");
                footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position

                footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos, lerpSpeed*10 * Time.deltaTime);

            }
            else
            {
                needToMove = false;
            }
            
        }
        else
        {
            footAtPosition = true;
            needToMove = false;
        }
        */
        
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
        //nextFootPos = nextFootPos + (forwardFacingObj.transform.up * 0.1f);//put alightly above new point
    }

    
}

/*
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
    public float lerpSpeed = 15;//todo set based on speed or size
    public bool hasOffset = false;
    //public float timeOffset = 0.0f;
    public GameObject forwardFacingObj;//for using for forward direction of whole animal
    public float footHeightMult = 2;
    public bool footAtPosition = true;
    public AnimalFeetPositioner otherFootRaycastPositioner;
    public float extraSpace = 0.2f;
    //public float footOnGroundDist = 0.2f;
    public float footMoveStopDist = 0.01f;

    private int layerMask;//Mask for choosing what layer the raycast hits
    
    public Rigidbody rb;
    public bool debug = false;
    public float animalHeight = 1;
    public float animalLength = 1;

    public GameObject nextPosRaycaster;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        nextPosRaycaster = new GameObject("nextPosRaycaster");
        nextPosRaycaster.transform.parent = this.transform;
        
        
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        //animalObj = this.GetComponentInParent<CreatureStats>().gameObject;//get the first obj going up in hierarchy with animal stats script
        transform.rotation = forwardFacingObj.transform.rotation;
        forwardStepDist = animalLength / 4f;
        sideStepDist = animalLength / 6f;
        

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
        //todo use end foot object without moveable ankle and point directly at hit point?
        footIKTargetObj.transform.forward = forwardFacingObj.transform.forward;//this prevents the feet from beign twisted

        
        Vector3 raycastStart = transform.position;
        //Check if foot has got too far from desired position 
        if (axisDifferences.z > forwardStepDist +extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            raycastStart = raycastStart + (forwardFacingObj.transform.forward*-(forwardStepDist));
            GetDesiredFootPosition(raycastStart);
        }
        else if (axisDifferences.z < -forwardStepDist -extraSpace) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            raycastStart = raycastStart + (forwardFacingObj.transform.forward*(forwardStepDist));//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)
            GetDesiredFootPosition(raycastStart);
        }
        if(axisDifferences.x > sideStepDist+extraSpace)//Double check overall distance instead of just forward and sides
        {
            raycastStart = raycastStart + (forwardFacingObj.transform.right*-(sideStepDist));//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)
            GetDesiredFootPosition(raycastStart);
        }
        else if(axisDifferences.x < -sideStepDist-extraSpace)//Double check overall distance instead of just forward and sides
        {
            raycastStart = raycastStart + (forwardFacingObj.transform.right*(sideStepDist));//put raycast start x distance forward and in air to raycast down//(transform.forward * forwardStepDist)
            GetDesiredFootPosition(raycastStart);
        }

        
        
        float distToNext = Vector3.Distance(footIKTargetObj.transform.position, nextFootPos);
        //if foot too far away from where it should be then move closer
        if (distToNext > footMoveStopDist) //If distance of current footpos and rayhit is over stepdistance then take a step
        {
            float footLift = footHeightMult * (Vector3.Distance(footIKTargetObj.transform.position, nextFootPos) /5);            

            //only move foot if the other food is grounded or both feet had a problem and arent grounded
            if (otherFootRaycastPositioner.footAtPosition == true  || distToNext>(forwardStepDist+extraSpace)*2 || (otherFootRaycastPositioner.footAtPosition == false && this.footAtPosition == false))
            {
                footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position
                float footMoveSpeed = Mathf.Max(lerpSpeed,(distToNext/3)*lerpSpeed);
                footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos+(forwardFacingObj.transform.up*footLift), footMoveSpeed * Time.deltaTime);
            }
            else if(distToNext>(forwardStepDist+extraSpace)*4)
            {
                print("far foot");
                footAtPosition = false;//has started moving to next position so set to false and only becomes true if gets close enough to next position

                footIKTargetObj.transform.position = Vector3.MoveTowards( footIKTargetObj.transform.position, nextFootPos, lerpSpeed*10 * Time.deltaTime);

            }
            else
            {
//                print("other foot:" + otherFootRaycastPositioner.footAtPosition);
            }
            
        }
        else
        {
            footAtPosition = true;
        }
        
    }

    public void GetDesiredFootPosition(Vector3 raycastStart)
    {
        bool gotNewPos = false;
        
        
        RaycastHit hit;//hit informationf
        if (Physics.Raycast(raycastStart+ (forwardFacingObj.transform.up*10), -forwardFacingObj.transform.up, out hit, 20,layerMask))//cast ray and return if hit//use layer mask to avoid default layer and only hit environment layer
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

        //nextFootPos = nextFootPos + (forwardFacingObj.transform.up * 0.1f);//put alightly above new point
    }

    
}
*/
