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

    private void Start()
    {
        core = GameObject.Find("Core");
    }

    // Update is called once per frame
    void Update()
    {
        toPlayer = (core.transform.position-player.transform.position).normalized;

        if (!reverseDir)
        {
            toSun = (core.transform.position-sun.transform.position).normalized;
        }
        else
        {
            toSun = (sun.transform.position-core.transform.position).normalized;
        }
        float angleToTarget = Vector2.Angle(toSun, toPlayer);

        float lightAmount = Mathf.Clamp((angleToTarget-45)/90,0, 2);
        Light.intensity = lightAmount;
        
//        print(angleToTarget);
    }
}
