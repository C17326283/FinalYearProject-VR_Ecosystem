using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlayer : MonoBehaviour
{
    public GameObject core;
    public bool grounded = false;
    public Rigidbody rb;
    public float gravityStrength = 100;
    public Vector3 groundNormal;
    public float maxSpeed = 100;
    public float moveSpeed = 50;
    public Vector3 gravityDir;
    
    // Start is called before the first frame update
    void Start()
    {
        core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

    }

    private void OnEnable()
    {
        core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        gravityDir = (transform.position - core.transform.position).normalized;
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, -gravityDir, out hit, 100))
        {
            print("pos");
            transform.position = hit.point + (gravityDir * 5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Test Movement

        float x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        
        
        transform.Translate(x,0,z);
        
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, 150 * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -150 * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = maxSpeed;
        }
        else
        {
            moveSpeed = maxSpeed / 2;
        }
        
        
        
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position+(transform.up*5), -transform.up, out hit, 100))
        {
            Debug.DrawRay(transform.position, -transform.up, Color.green);
            float distanceToGround = hit.distance;
            groundNormal = hit.normal;
        }

        gravityDir = (transform.position - core.transform.position).normalized;
        rb.AddForce(gravityDir*-gravityStrength);
        
        Quaternion toRotation = Quaternion.FromToRotation(transform.up,groundNormal)*transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation,toRotation,0.01f);
    }
}
