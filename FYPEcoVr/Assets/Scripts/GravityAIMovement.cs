using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAIMovement : MonoBehaviour
{
    public GameObject core;
    public bool grounded = false;
    public Rigidbody rb;
    public float gravityStrength = 100;
    public Vector3 groundNormal;
    public float moveSpeed = 50;
    public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
        /*
        if (target != null)
        {
            //LookAtTarget();//aim towards target
            MoveForward();
        }
        */
        //Vector3 toTar = player.position - transform.position;
        //Vector3 dotToTar = new Vector3.Dot(transform.forward, target.transform.position);
        
        
        
        //Get ground position
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -transform.up, out hit, 100))
        {
            Debug.DrawRay(transform.position, -transform.up, Color.green);
            groundNormal = hit.normal;
        }

        //Add gravity down
        Vector3 gravityDir = (transform.position - core.transform.position).normalized;
        rb.AddForce(gravityDir*-gravityStrength);
        
        //Rotate so body is to ground
        //Quaternion toRotation = Quaternion.FromToRotation(transform.up,groundNormal)*transform.rotation;
        
        //transform.rotation = Quaternion.Lerp(transform.rotation,toRotation,0.2f);
        
        /*
        if (target != null)
        {
            Vector3 relativePos = target.transform.position - transform.position;
            
            Quaternion toRotation = Quaternion.LookRotation(relativePos,gravityDir);
            //Quaternion toRotation = Quaternion.FromToRotation(transform.forward,target.transform.position)*transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation,toRotation,0.2f);
        }
        */
    }

    public void MoveForward()
    {
        
        transform.Translate(0,0,moveSpeed);
    }
    
    public void LookAtTarget()
    {
        Vector3 targetPostition = new Vector3( this.transform.position.x, target.transform.position.x, this.transform.position.z ) ;
        this.transform.LookAt( targetPostition ) ;
        
        Quaternion toRotation = Quaternion.LookRotation(target.transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 10 * Time.deltaTime);
    }
    
    
}
