using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAtAngle : MonoBehaviour
{
    public GameObject player;

    public Light Light;

    public GameObject core;

    public GameObject sun;

    public Vector3 toPlayer;
    public Vector3 toSun;
    public float dayLightIntensity = 2;
    public bool reverseDir;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, toPlayer);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, toSun);
    }

    private void OnEnable()
    {
        core = GameObject.Find("Core");
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        toPlayer = (core.transform.position-player.transform.position).normalized;

        if (reverseDir)
        {
            toSun = (core.transform.position-sun.transform.position).normalized;
        }
        else
        {
            toSun = (sun.transform.position-core.transform.position).normalized;
        }
        float angleToTarget = Vector3.Angle(toSun, toPlayer);
        //print("angleToTarget"+angleToTarget);
        float angleToTargetNorm = (angleToTarget/180)*3;//Bring everything in range 0-3 with 1.5 being midpoint between bright and dark 
        //print("angleToTargetNorm"+angleToTargetNorm);
        angleToTargetNorm = angleToTargetNorm-1f;//Bring everything in range -1to2 with .5 being midpoint between bright and dark 
        //print("angleToTargetNorm"+angleToTargetNorm);
        angleToTargetNorm = Mathf.Clamp(angleToTargetNorm,0,1);//1f clamped on both sides meaning <0 is 0 to 60deg/0 to 1 is 60to120 degrees and >1 is 120degrees. but all clamped so full brightness if >2/3
        //print("angleToTargetNorm"+angleToTargetNorm);
        //angleToTarget += 45;//ant
        //float
        

        float lightAmount = angleToTargetNorm*dayLightIntensity;
        //print("lightAmount"+lightAmount);
        Light.intensity = lightAmount;
        
//        print(angleToTarget);
    }
}
