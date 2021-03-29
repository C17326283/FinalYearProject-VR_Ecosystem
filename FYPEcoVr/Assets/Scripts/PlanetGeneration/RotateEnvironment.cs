using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simply rotate at set speed
public class RotateEnvironment : MonoBehaviour
{
    public float rotateSpeedX = .5f;
    public float rotateSpeedY = .1f;
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(-rotateSpeedX * Time.deltaTime ,-rotateSpeedY * Time.deltaTime,0 );
    }
}
