using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed;
    public float defaultSpeed = 50;
    public float turnSpeed=150;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        
        //transform.Translate(x,0,z);
        
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
        }
        
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = defaultSpeed*2;
        }
        else
        {
            moveSpeed = defaultSpeed;
        }
        
        rb.AddRelativeForce(transform.forward * (moveSpeed * z));
    }
}
