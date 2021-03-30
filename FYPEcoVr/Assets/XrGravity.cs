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
    
    public void CheckGroundPos()
    {
        //if ray hits anything above
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position + (-gravityDir * (maxHeightFromSurface*2)), gravityDir, out rayHit, maxHeightFromSurface*2, layerMask))
        {
            transform.position = rayHit.point;
            rigRb.velocity = Vector3.zero;
        }
        else
        {
            rigRb.AddForce(gravityDir * (gravForce * Time.deltaTime));
        }
    }
    
    void FixedUpdate()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir
        //If abovce groudn then add gravity else push back up above ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, gravityDir, out hit, maxHeightFromSurface*2, layerMask))
        {
            //Is above ground add gravity
            if (hit.transform.CompareTag("Ground"))
            {
                if(Vector3.Distance(transform.position,hit.point)>0.1f)
                    rigRb.AddForce(gravityDir * (gravForce * Time.deltaTime));
            }
            else if (hit.transform.CompareTag("WaterMesh"))
            {
                CheckGroundPos();
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
        else
        {
            CheckGroundPos();
        }
        

    }
}
