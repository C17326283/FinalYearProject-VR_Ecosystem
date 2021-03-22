using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGravity : MonoBehaviour
{
    public Vector3 gravityDir;
    public GameObject core;
    public float gravForce = 1000;
    public Rigidbody rb;
    public bool disableOnCollision = true;

    

    // Start is called before the first frame update
    void Awake()
    {
        if(core==null)
            core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

    }

    // Update is called once per frame
    void Update()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir
        rb.AddForce(gravityDir * (gravForce * Time.deltaTime));

    }

    private void OnEnable()
    {
        rb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            this.enabled = false;
            rb.isKinematic = true;

        }
    }
}
