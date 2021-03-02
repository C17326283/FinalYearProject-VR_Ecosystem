using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;

public class SpineNew : MonoBehaviour {
    public List<Vector3> offsets = new List<Vector3>();
    public List<Transform> spineContainers = new List<Transform>();
    
    public GameObject start;
    private GameObject armatureBase;//one above head

    public float damping = 20.0f;

    private GameObject spinesHolder;
    public bool isLimbSetup = false;


    //make all the holders and position spines
    public void InitializeSpine()
    {
        armatureBase = start.transform.parent.gameObject;//The object above head has the full armature
        spinesHolder = this.gameObject;
        //spinesHolder.transform.name = "SpinesHolder";

        //get spinal sections which stem from the head
        GetSpineObjRecursively(start);
        SaveSpineOffsets();

        //MatchLimbsToSpine();
    }

    public void GetSpineObjRecursively(GameObject head)
    {
        //get all direct children 
        Transform[] spineObjs =  head.GetComponentsInChildren<Transform>();
//        print(spineObjs.Length);
        
        //Make a holder for each section of the spine and add this to it
        foreach (Transform spineObj in spineObjs)//expect only one or 0 children but foreach works better
        {
            PutSpineObjIntoContainer(spineObj.gameObject);
        }
    }


    //Make a blank object where this one is so the conatiner can follow prev object but maintain rotatation of bone
    public void PutSpineObjIntoContainer(GameObject spineObj)
    {
        GameObject spineContainer;
        spineContainer = new GameObject("SpineSection"+spineContainers.Count);

        if (spineContainers.Count == 0)
        {
            if (isLimbSetup)
            {
                spineContainer = spineObj.transform.parent.gameObject;
            }
            //spineContainer.transform.name = "SpineSection" + spineContainers.Count + "-PhysicsObject";
        }

        spineContainer.transform.position = spineObj.transform.position;
        spineContainer.transform.parent = spinesHolder.transform;
        spineContainer.transform.tag = "SpineContainer";//so feet positioners can get parented to it too
        
        spineObj.transform.parent = spineContainer.transform;
        
        spineContainers.Add(spineContainer.transform);
    }

    //The legs need to stick to the closest spine in the armature model or else theyll stretch 

    public void MatchLimbToSpine(Transform objToCompare)
    {
        float closestDist = Mathf.Infinity;
        Transform closestContainer = spineContainers[0];//set default
        foreach (var spineContainer in spineContainers)
        {
            float currentContDist = Vector3.Distance(objToCompare.transform.position, spineContainer.transform.position);
            if (currentContDist<closestDist)
            {
                closestDist = currentContDist;//set it as new closest
                closestContainer = spineContainer.transform;//set it to that container for now unless overwritten
            }
        }
        
        objToCompare.transform.parent = closestContainer;
    }
    
    //overload function to check if head is closer//for ears
    public void MatchLimbToSpine(Transform objToCompare,Transform head)
    {
        float closestDist = Mathf.Infinity;
        Transform closestContainer = spineContainers[0];//set default
        foreach (var spineContainer in spineContainers)
        {
            float currentContDist = Vector3.Distance(objToCompare.transform.position, spineContainer.transform.position);
            if (currentContDist<closestDist)
            {
                closestDist = currentContDist;//set it as new closest
                closestContainer = spineContainer.transform;//set it to that container for now unless overwritten
            }
        }
        
        //Also check head which isnt part of main spine
        float headDist = Vector3.Distance(objToCompare.transform.position, head.position);
        if (headDist<closestDist)
        {
            closestContainer = head;//set it to that container for now unless overwritten
        }
        
        objToCompare.transform.parent = closestContainer;
    }
    
    

    public Transform GetClosestSpineContainer(Transform objToCompare,Transform head)
    {
        float closestDist = 999999f;
        Transform closestContainer = spineContainers[0];//set default
        foreach (var spineContainer in spineContainers)
        {
            float currentContDist = Vector3.Distance(objToCompare.transform.position, spineContainer.transform.position);
            if (currentContDist<closestDist)
            {
                closestDist = currentContDist;//set it as new closest
                closestContainer = spineContainer.transform;//set it to that container for now unless overwritten
            }
        }
        //Also check head which isnt part of main spine
        float headDist = Vector3.Distance(objToCompare.transform.position, head.position);
        if (headDist<closestDist)
        {
            closestDist = headDist;//set it as new closest
            closestContainer = head;//set it to that container for now unless overwritten
        }
        
        return closestContainer;
    }

    public void SaveSpineOffsets()
    {
        //save the local position of each spine
        for (int i = 0; i < spineContainers.Count; i++)
        {
            Transform current = spineContainers[i].transform;
            if (i > 0)
            {
                Transform prev = spineContainers[i-1].transform;
                // Offset from previous to current
                Vector3 offset = current.transform.position - prev.transform.position; 
                
                // Rotating from world back to local
                offset = Quaternion.Inverse(prev.transform.rotation) * offset;//rotate it so it stays at the original rotation
                offsets.Add(offset);                
            }            
        }
    }
    
    
    
    
    
    
    //Procedurally animate spine each frame
    void FixedUpdate ()
    {
        for (int i = 1; i < spineContainers.Count; i++)
        {
            Transform prev = spineContainers[i - 1];
            Transform current = spineContainers[i];
            Vector3 wantedPosition = prev.position + ((prev.rotation * offsets[i-1]));

            Vector3 lerpedPosition = Vector3.Lerp(current.position, wantedPosition, Time.deltaTime * damping);
            
            // Dont move the segments too far apart
            Vector3 clampedOffset = lerpedPosition - prev.position;
            clampedOffset = Vector3.ClampMagnitude(clampedOffset, offsets[i-1].magnitude);
            current.position = prev.position + clampedOffset;

            //uses containers to preserve the natural bone rotations so containers match the head
            current.rotation = Quaternion.Slerp(current.rotation, prev.rotation, Time.deltaTime * (damping));
        }
        
        
    }
    
}
