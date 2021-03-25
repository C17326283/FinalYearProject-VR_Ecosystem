using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunAngle : MonoBehaviour
{
    public GameObject core;

    public GameObject sun;
    
    private void OnEnable()
    {
        core = GameObject.Find("Core");
        sun = GameObject.Find("Sun");
    }
    

    public float GetTargetAngleToSun(GameObject target, bool reverseDir)
    {
        Vector3 toTarget = (core.transform.position-target.transform.position).normalized;
        Vector3 toSun;

        if (reverseDir)
        {
            toSun = (core.transform.position-sun.transform.position).normalized;
        }
        else
        {
            toSun = (sun.transform.position-core.transform.position).normalized;
        }
        
        float angleToTarget = Vector3.Angle(toSun, toTarget);
        return angleToTarget;//returns value between 0 and 180 based on angle to sun
    }



}
