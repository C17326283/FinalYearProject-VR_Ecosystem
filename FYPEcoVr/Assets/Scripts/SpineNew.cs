using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineNew : MonoBehaviour {
    public List<Vector3> offsets = new List<Vector3>();
    public List<Transform> spineContainers = new List<Transform>();
    //public List<Transform> children = new List<Transform>();
    
    public GameObject head;
    private GameObject armatureBase;//one above head
    
    
    
    public float damping = 10.0f;

    private GameObject spinesHolder;

    // Use this for initialization
    void Start()
    {
        armatureBase = head.transform.parent.gameObject;
        if (spinesHolder == null)
        {
            spinesHolder = new GameObject("SpinesHolder");
            spinesHolder.transform.position = transform.position;
            spinesHolder.transform.rotation = transform.rotation;
            spinesHolder.transform.parent = transform;
            
        }

        GetSpineObjRecursively(head);
        
        
        
        
        // This iterates through all the children transforms
        for (int i = 0; i < spineContainers.Count; i++)
        {
            Transform current = spineContainers[i].transform;
            if (i > 0)
            {
                Transform prev = spineContainers[i-1].transform;
                // Offset from previous to current
                Vector3 offset = current.transform.position - prev.transform.position; 
                
                // Rotating from world back to local
                offset = Quaternion.Inverse(prev.transform.rotation) * offset;
                offsets.Add(offset);                
            }            
        }
        
        MatchLegsToSpine();
        /*
        for (int i = 0; i < spinesHolder.transform.childCount; i++)
        {
            Transform current = spinesHolder.transform.GetChild(i);
            if (i > 0)
            {
                Transform prev = spinesHolder.transform.GetChild(i - 1);
                // Offset from previous to current
                Vector3 offset = current.transform.position - prev.transform.position; 
                
                // Rotating from world back to local
                offset = Quaternion.Inverse(prev.transform.rotation) * offset;
                offsets.Add(offset);                
            }            
            children.Add(current);
        }
        */
    }

    public void GetSpineObjRecursively(GameObject obj)
    {
        MakeSpineObjHolder(obj);

        foreach (Transform child in obj.transform)//expect only one or 0 children but foreach works better
        {
            GetSpineObjRecursively(child.gameObject);
        }
    }


    public void MakeSpineObjHolder(GameObject spineObj)
    {
        
        GameObject spineContainer = new GameObject("Spine"+spineContainers.Count+" container");
        spineContainer.transform.position = spineObj.transform.position;
        spineContainer.transform.parent = spinesHolder.transform;
        
        spineObj.transform.parent = spineContainer.transform;
        
        spineContainers.Add(spineContainer.transform);
    }

    //The legs need to stick to the closest spine in the armature model or else theyll stretch 
    void MatchLegsToSpine()
    {
        
        List<Transform> allLimbs = new List<Transform>();

        //make a list to loop through, cant do in same loop because taking objs out midloop will skip others
        foreach (Transform child in armatureBase.transform)
        {
            allLimbs.Add(child);
        }

        foreach (Transform limb in allLimbs)
        {
            print("limb"+limb);
            float closestCont = 999999f;
            foreach (var spineContainer in spineContainers)
            {
                //print("container");
                float currentContDist = Vector3.Distance(limb.transform.position, spineContainer.transform.position);
                //print("currentContDist"+currentContDist+"   closestCont"+closestCont);
                if (currentContDist<closestCont)
                {
                    //print("closer");
                    closestCont = currentContDist;//set it as new closest
                    limb.transform.parent = spineContainer.transform;//set it to that container for now
                    //print(limb.transform+":"+limb.transform.parent);
                }
            }
        }
        
    }
    
    
    // Update is called once per frame
    void Update () {
        
        
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
            current.rotation = Quaternion.Slerp(current.rotation, prev.rotation, Time.deltaTime * damping);
        }
        
    }
    
}
