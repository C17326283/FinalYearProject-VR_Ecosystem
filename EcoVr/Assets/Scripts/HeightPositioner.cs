using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightPositioner : MonoBehaviour
{
    public GameObject raycastStartObj;//This is the point for measuring dist from ground//put at front of biggest mass
    public float heightDistance = 2;
    public float deadZone = .01f;//Small area where the height isnt moved up or down everyfram if havent moved too much
    public float lerpSpeed = 1f;
    public float raydistance = 100;
    private Rigidbody rb;

    private int layerMask;//Mask for choosing what layer the raycast hits
    void Start()
    {
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        if (raycastStartObj == null)
            raycastStartObj = this.gameObject;
        
        RaycastHit hit;
        if (Physics.Raycast(raycastStartObj.transform.position, Vector3.down, out hit, 100, layerMask))
        {
            this.transform.position = new Vector3(this.transform.position.x, hit.point.y+heightDistance, this.transform.position.z);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        //Todo have an isGrounded system so the animal can fall convincingly
        RaycastHit hit;
        if (Physics.Raycast(raycastStartObj.transform.position, Vector3.down, out hit, raydistance,layerMask))
        {
            Vector3 transformPosition = this.transform.position;
            if (hit.distance != heightDistance)//is too low so go higher//+wiggleRoom || hit.distance > heightDistance-wiggleRoom
            {
                this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(transformPosition.x, hit.point.y+heightDistance, transformPosition.z), lerpSpeed*Time.deltaTime);
            }
        }
        
    }
    
    
}
