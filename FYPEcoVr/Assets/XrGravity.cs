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
    
    private int layerMask;
    
    public bool hasCore = false;//Making sure not in starting room
    public bool allowedFly = true;//Is above ground and not too high
    
    void OnEnable()
    {
        if (core == null)
        {
            core = GameObject.Find("Core");
        }
        hasCore = true;
        
        layerMask = 1 << 8;

        
    }
    void FixedUpdate()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir
        
        //If ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, gravityDir, out hit, maxHeightFromSurface*2, layerMask))
        {
            if (hit.transform.CompareTag("WaterMesh")) //if its water player may be under land 
            {
                //shoot another ray from above to test if underground
                RaycastHit rayHit;
                if (Physics.Raycast(transform.position+(transform.up*100), gravityDir, out rayHit, maxHeightFromSurface*2, layerMask))
                {
                    if (hit.transform.CompareTag("Ground"))
                    {
                        rigRb.velocity = Vector3.zero;
                        rigRb.transform.position += rigRb.transform.up;//Add slight bit up so can eventually get back onto ground
                    }
                    else
                    {
                        rigRb.AddForce(gravityDir * (gravForce * Time.deltaTime));
                    }
                }
            }
            else
            {
                rigRb.AddForce(gravityDir * (gravForce * Time.deltaTime));
            }

            //If not too high then allow jetpack to get higher
            if (hit.distance < maxHeightFromSurface)
            {
//                print("current height"+hit.distance);
                allowedFly = true;
            }
            else
            {
                
 //               print("cancel fly");
                allowedFly = false;
            }
        }
        else//starting to fall through ground so cancel vel
        {
            rigRb.velocity = Vector3.zero;
        }
        
        

    }
}
