using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAIMovement : MonoBehaviour
{
    public GameObject core;
    public Rigidbody rb;
    public float gravityStrength = 100;
    public Vector3 groundNormal;
    public float moveSpeed = 50;
    public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        //core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        gravityStrength = 100;
        moveSpeed = 0.2f;//temp, assigned by stats in future
    }

    // Update is called once per frame
    void Update()
    {

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

        if (target!= null && Vector3.Distance(this.transform.position, target.transform.position) > 2.0f)
        {
            transform.LookAt(target.transform, gravityDir);
            MoveForward();
        }
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
