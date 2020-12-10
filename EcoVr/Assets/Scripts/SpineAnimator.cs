using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpineAnimator : MonoBehaviour {
    public List<Vector3> offsets = new List<Vector3>();
    public List<Quaternion> rotOffsets = new List<Quaternion>();
    public List<Transform> bones = new List<Transform>();
    public float damping = 10.0f;

    public bool useSpineAnimatorSystem = false;

    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < bones.Count; i++)//perform operation on every child segment
        {
            Transform current = bones[i];
            
            //save the rotation that eahc section has at start
            Quaternion rotationOffset = current.transform.rotation; //get the offset between the legs
            rotOffsets.Add(rotationOffset);//Save offset to list
            
            if (i > 0)//save all offsets between each section
            {
                Transform prev = bones[i-1];//use last segment as prev for positioning current
                Vector3 offset = current.transform.position - prev.transform.position; //get the offset between the legs
//                Debug.Log("offset"+offset+""+current.transform.position+""+prev.transform.position);
               offset = Quaternion.Inverse(prev.transform.rotation) * offset;//I think we only need this if we want the position relative to head rotation which we dont because if head looks up then we dont want body to go down
                offsets.Add(offset);//Save offset to list
            }            
        }
        if (useSpineAnimatorSystem)
        {
            SpineAnimatorManager.Instance.AddSpine(this);//add spine to spine animator
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (useSpineAnimatorSystem)//check spine animator working
        {
            return;
        }
        for (int i = 1; i < bones.Count; i++)//Lerp all points to new position
        {
            //todo add max stretch clamp
            Transform prev = bones[i - 1];
            Transform current = bones[i];
            Vector3 wantedPosition = prev.position + ((prev.rotation * offsets[i-1]));//i-1 because head doesnt have offset
            //Vector3 wantedPosition = prev.position + (transform.forward*offsets[i-1]);//i-1 because head doesnt have offset

            Quaternion wantedRotation = prev.rotation ;//+ rotOffsets[i-1]
            wantedRotation.z = rotOffsets[i].z;
            //Quaternion toRotation = Quaternion.Euler(wantedRotation.x, wantedRotation.y, wantedRotation.z);

            current.position = Vector3.Lerp(current.position, wantedPosition, Time.deltaTime * damping);
            //current.rotation = Quaternion.Lerp(current.rotation, wantedRotation, Time.deltaTime * damping);
            current.rotation = Quaternion.Slerp(current.rotation, wantedRotation, Time.deltaTime * damping);
        }
    }
}



/*
public class SpineAnimator : MonoBehaviour {
    public List<Vector3> offsets = new List<Vector3>();
    public List<Transform> bonesContainers = new List<Transform>();
    public float damping = 10.0f;

    // Use this for initialization
    void Start()
    {
        
        Cursor.visible = false;
        // This iterates through all the children transforms
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform current = transform.GetChild(i);
            if (i > 0)
            {
                Transform prev = transform.GetChild(i - 1);
                // Offset from previous to current
                Vector3 offset = current.transform.position - prev.transform.position; 
                
                // Rotating from world back to local
                offset = Quaternion.Inverse(prev.transform.rotation) * offset;
                offsets.Add(offset);                
            }            
            children.Add(current);
        }
        
        // This iterates through all the children transforms
        for (int i = 0; i < bonesContainers.Count; i++)
        {
            //Transform current = transform.GetChild(i);
            Transform current = bonesContainers[i];
            if (i > 0)
            {
                Transform prev = bonesContainers[i-1];
                // Offset from previous to current
                Vector3 offset = current.transform.position - prev.transform.position; 
                
                // Rotating from world back to local
                offset = Quaternion.Inverse(prev.transform.rotation) * offset;
                offsets.Add(offset);                
            }            
        }
        
    }

    // Update is called once per frame
    void Update () {
        
        for (int i = 1; i < bonesContainers.Count; i++)
        {
            Transform prev = bonesContainers[i - 1];
            Transform current = bonesContainers[i];
            Vector3 wantedPosition = prev.position + ((prev.rotation * offsets[i-1]));
            Quaternion wantedRotation = Quaternion.LookRotation(prev.transform.position - current.position, prev.transform.up);

            Vector3 lerpedPosition = Vector3.Lerp(current.position, wantedPosition, Time.deltaTime * damping);
            
            // Dont move the segments too far apart
            Vector3 clampedOffset = lerpedPosition - prev.position;
            clampedOffset = Vector3.ClampMagnitude(clampedOffset, offsets[i-1].magnitude);
            current.position = prev.position + clampedOffset;


            current.rotation = Quaternion.Slerp(current.rotation, wantedRotation, Time.deltaTime * damping);
        }
    }
}
*/