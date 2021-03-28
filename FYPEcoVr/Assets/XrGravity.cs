using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XrGravity : MonoBehaviour
{
    
    public Rigidbody rigRb;
    public Vector3 gravityDir;
    public GameObject core;
    public float gravForce = 100;

    public float maxHeightFromSurface = 150;
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
        if (Physics.Raycast(transform.position, gravityDir, out hit, maxHeightFromSurface*4, layerMask))
        {
            rigRb.AddForce(gravityDir * (gravForce * Time.deltaTime));

            //If not too high then allow jetpack to get higher
            if (hit.distance < maxHeightFromSurface)
            {
//                print("current height"+hit.distance);
                jetPackController1.allowedFly = true;
                jetPackController2.allowedFly = true;
            }
            else
            {
 //               print("cancel fly");
                jetPackController1.allowedFly = false;
                jetPackController2.allowedFly = false;
            }
        }
        else//starting to fall through ground so cancel vel
        {
            rigRb.velocity = Vector3.zero;
        }
        
        

    }
}
