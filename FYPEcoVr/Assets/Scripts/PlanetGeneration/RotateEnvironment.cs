using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simply rotate at set speed
public class RotateEnvironment : MonoBehaviour
{
    public float rotateSpeed = .5f;
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(-rotateSpeed * Time.deltaTime ,0,0 );
    }
}
