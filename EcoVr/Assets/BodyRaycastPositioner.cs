using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRaycastPositioner : MonoBehaviour
{
    public GameObject bodyObj;
    public float heightDistance = 2;
    public float wiggleRoom = .1f;
    public float moveIncrements = .1f;

    private int layerMask;//Mask for choosing what layer the raycast hits
    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        bodyObj.transform.position = new Vector3(bodyObj.transform.position.x, 2, bodyObj.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100,layerMask))
        {
            //Debug.DrawRay(transform.position, Vector3.down, Color.blue, 1000);
//            Debug.Log("hoverboj hit: "+hit.transform.name+" hit distance: "+hit.distance+ " hover dist: "+heightDistance);
            
            var transformPosition = bodyObj.transform.position;
            if (hit.distance < heightDistance - wiggleRoom)//is too low so go higher
            {
                Debug.Log("too low");
                bodyObj.transform.position = new Vector3(bodyObj.transform.position.x, bodyObj.transform.position.y + moveIncrements, bodyObj.transform.position.z);
            }
            else if (hit.distance > heightDistance + wiggleRoom)//is too high so go lower
            {
                Debug.Log("too high");
                bodyObj.transform.position = new Vector3(bodyObj.transform.position.x, bodyObj.transform.position.y - moveIncrements, bodyObj.transform.position.z);
                
            }
        }
    }
    
    
}
