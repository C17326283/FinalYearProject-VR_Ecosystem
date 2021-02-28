using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtVel : MonoBehaviour
{
    public Rigidbody rb;

    public float speed=5;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude>.1f)
        {
            Quaternion rotation = Quaternion.LookRotation(rb.velocity, rb.transform.up);//look to velocity, align with ground
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed*Time.deltaTime);
        }
    }
}
