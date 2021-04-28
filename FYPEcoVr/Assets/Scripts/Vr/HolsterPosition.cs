using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterPosition : MonoBehaviour
{
    public Transform head;
    public Transform rigBase;
    public Vector3 offset;
    public float maxDist;

    private void Start()
    {
        //offset = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        


        float dist = Vector3.Distance(head.position, rigBase.position);
        transform.localPosition = head.localPosition+offset;//+rigBase.InverseTransformDirection(offset)

        
    }

    //testing but taking too long to implement, was trying to set holster pos to where last let go
    public void SetOffset(GameObject objToHolster)
    {
        //offset = head.InverseTransformPoint(objToHolster.transform.position);
        
        //offset = head.transform.position-objToHolster.transform.position;

        print("offset "+offset);
        transform.rotation = objToHolster.transform.rotation;
    }
}
