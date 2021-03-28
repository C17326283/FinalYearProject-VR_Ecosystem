using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XrGravity : MonoBehaviour
{
    
    public Rigidbody rigRb;
    public Vector3 gravityDir;
    public GameObject core;
    public float gravForce = 100;

    public float maxDistFromCore = 1200;
    public float maxHeightFromSurface = 1200;
    public JetPack jetPackController1;
    public JetPack jetPackController2;
    
    private int layerMask;
    
    void OnEnable()
    {
        if(core==null)
            core = GameObject.Find("Core");
        layerMask = 1 << 8;

        
    }
    void FixedUpdate()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir
        
        //If ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, gravityDir, out hit, 500, layerMask))
        {
            rigRb.AddForce(gravityDir * (gravForce * Time.deltaTime));

            //If not too high then allow jetpack to get higher
            if (hit.distance < maxHeightFromSurface)
            {
                jetPackController1.allowedFly = true;
                jetPackController2.allowedFly = true;
            }
            else
            {
                jetPackController1.allowedFly = false;
                jetPackController2.allowedFly = false;
            }
        }
        
        

    }
}
