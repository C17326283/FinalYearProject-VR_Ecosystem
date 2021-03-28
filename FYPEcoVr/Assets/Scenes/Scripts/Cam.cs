using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public Transform target;

    public GameObject core;

    public float turnSpeed = 10;

    public float posUp = 2;
    // Start is called before the first frame update
    void Start()
    {
        core = GameObject.Find("Core");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir
        Vector3 forwardDir =  (target.transform.position+(-gravityDir*posUp))-transform.position;

        Quaternion rotation = Quaternion.LookRotation(forwardDir, -gravityDir);//look to velocity, align with ground
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, (turnSpeed)*Time.deltaTime);//do it over time


    }
}
