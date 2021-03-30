using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunAngle : MonoBehaviour
{
    public GameObject core;

    public GameObject sun;
    public GameObject moon;
    
    private void OnEnable()
    {
        core = GameObject.Find("Core");
        sun = GameObject.Find("Sun");
        moon = GameObject.Find("Moon");
    }
    

    public float GetTargetAngleToSun(GameObject target, bool moonInstead)
    {
        Vector3 toTarget = (core.transform.position-target.transform.position).normalized;
        Vector3 toSkyObj;

        if (moonInstead)
        {
            toSkyObj = (moon.transform.position-core.transform.position).normalized;
        }
        else
        {
            toSkyObj = (sun.transform.position-core.transform.position).normalized;
        }
        
        float angleToTarget = Vector3.Angle(toSkyObj, toTarget);
        return angleToTarget;//returns value between 0 and 180 based on angle to sun
    }
    
    /*
    
    public float GetTargetAngleToMoon(GameObject target, bool reverseDir)
    {
        Vector3 toTarget = (core.transform.position-target.transform.position).normalized;
        Vector3 toMoon;

        
        if (reverseDir)
        {
            toMoon = (core.transform.position-sun.transform.position).normalized;
        }
        else
        {
            toMoon = (sun.transform.position-core.transform.position).normalized;
        }
        
        float angleToTarget = Vector3.Angle(toMoon, toTarget);
        return angleToTarget;//returns value between 0 and 180 based on angle to sun
    }
    
    */



}
