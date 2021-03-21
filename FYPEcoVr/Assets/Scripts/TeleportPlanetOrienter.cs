using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportPlanetOrienter : MonoBehaviour
{
    public GameObject core;
    public GameObject xrRigObject;
    public GameObject xrHeadset;
    public Vector3 gravityDir;
    public XRRig xrRig;

    public float rayHeight=100;

    public float rayDist=1000;
    private int layerMask;    // Start is called before the first frame update

    [Header("gravity up or normal up")]
    public bool matchNormalUp = false;
    void OnEnable()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir
        core = GameObject.Find("Core");
        layerMask = 1 << 8;
        if (xrRigObject == null)
            xrRigObject = this.transform.gameObject;
        if (xrRig == null)
            xrRig = this.transform.gameObject.GetComponent<XRRig>();
        if (xrHeadset == null)
            xrHeadset = transform.GetComponentInChildren<Camera>().gameObject;

    }

    public void Teleport()
    {
        gravityDir = (core.transform.position - xrRigObject.transform.position).normalized; //todo flip dir
        //Vector3 teleportDir = (core.transform.position - xrRig.transform.position).normalized;
        //Vector3 localBeforeDir = transform.InverseTransformDirection(xrHeadset.transform.forward);
        
        //xrRig.transform.rotation = Quaternion.LookRotation(xrHeadset.transform.forward, -gravityDir);
        //xrRig.transform.up = -gravityDir;
        //xrRig.transform.rotation = Quaternion.LookRotation(xrRig.transform.forward, -gravityDir);
        //Vector3 playerForward = xrRig.transform.TransformDirection(Vector3.forward);
        //xrRig.transform.rotation = Quaternion.LookRotation(playerForward, -gravityDir);
        //xrRig.transform.rotation = Quaternion.FromToRotation(xrRig.transform.transform.up, -gravityDir);


        if (matchNormalUp)
        {
            RaycastHit hit; //shoot ray and if its ground then spawn at that location
            if (Physics.Raycast(transform.position+(-gravityDir*5), gravityDir, out hit, 1000, layerMask))
            {
                xrRig.MatchRigUp(hit.normal);
            }
            
        }
        else
        {
            xrRig.MatchRigUp(-gravityDir);
        }
        
        
        //xrRig.MatchRigUpRigForward(-gravityDir);

        /*
        RaycastHit hit;
        //Only add if theres environment below
        if (Physics.Raycast(xrRig.transform.position + (-gravityDir * rayHeight), gravityDir, out hit, rayDist, layerMask))
        {
            xrRig.transform.rotation = Quaternion.LookRotation(xrRig.transform.forward, -gravityDir);
            print("teleport");
        }
        */

    }

    private void Update()
    {
        Teleport();
    }
}
