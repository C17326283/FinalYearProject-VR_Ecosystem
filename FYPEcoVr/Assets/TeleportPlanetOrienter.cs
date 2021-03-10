using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlanetOrienter : MonoBehaviour
{
    public GameObject core;
    public GameObject xrRig;
    public Vector3 gravityDir;

    public float rayHeight=100;

    public float rayDist=1000;
    private int layerMask;    // Start is called before the first frame update
    void Start()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir
        core = GameObject.Find("Core");
        layerMask = 1 << 8;
        if (xrRig == null)
            xrRig = this.transform.gameObject;

    }

    public void Teleport()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir

        RaycastHit hit;
        //Only add if theres environment below
        if (Physics.Raycast(transform.position + (-gravityDir * rayHeight), gravityDir, out hit, rayDist, layerMask))
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
            print("teleport");
        }
        
    }
}
