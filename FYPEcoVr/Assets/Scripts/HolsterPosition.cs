using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterPosition : MonoBehaviour
{
    public Transform head;
    public Transform rigBase;
    public Vector3 offset;

    private void Start()
    {
        //offset = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        


        float dist = Vector3.Distance(head.position, rigBase.position);
        transform.localPosition = head.localPosition+offset;//+rigBase.InverseTransformDirection(offset)

        
    }
}
