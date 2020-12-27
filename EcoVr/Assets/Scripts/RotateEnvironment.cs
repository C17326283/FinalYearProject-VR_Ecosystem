using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simply rotate at set speed
public class RotateEnvironment : MonoBehaviour
{
    public int rotateSpeed = 1;
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(-rotateSpeed * Time.deltaTime ,0,0 );
    }
}
