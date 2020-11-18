using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAnimator : MonoBehaviour {
    public List<Vector3> offsets = new List<Vector3>();
    public List<Transform> bones = new List<Transform>();
    public float damping = 10.0f;

    public bool useSpineAnimatorSystem = false;

    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < bones.Count; i++)//perform operation on every child segment
        {
            Transform current = bones[i];
            if (i > 0)
            {
                Transform prev = bones[i-1];//use last segment as prev for positioning current
                Vector3 offset = current.transform.position - prev.transform.position; //get the offset between the legs
                Quaternion rotationOffset = current.transform.rotation; //get the offset between the legs
                Debug.Log("offset"+offset+""+current.transform.position+""+prev.transform.position);
                offset = Quaternion.Inverse(prev.transform.rotation) * offset;
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
            Quaternion wantedRotation = prev.rotation;

            current.position = Vector3.Lerp(current.position, wantedPosition, Time.deltaTime * damping);
            //current.rotation = Quaternion.Slerp(current.rotation, wantedRotation, Time.deltaTime * damping);
        }
    }
}
