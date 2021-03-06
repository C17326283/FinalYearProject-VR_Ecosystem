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
    private int layerMask;

    

    // Start is called before the first frame update
    void Awake()
    {
        if(core==null)
            core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        layerMask = 1 << 8;


    }

    // Update is called once per frame
    void Update()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir
        //If abovce groudn then add gravity else push back up above ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, gravityDir, out hit, 100, layerMask))
        {
            //Is above ground add gravity
            if (hit.transform.CompareTag("Ground"))
            {
                rb.AddForce(gravityDir * (gravForce * Time.deltaTime));
            }
            else//is above water but may be under ground
            {
                FixPos();
            }

        }
        else
        {
            FixPos();
        }

    }
    
    public void FixPos()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position + (-gravityDir * 100), gravityDir, out rayHit, 200, layerMask))
        {
            if (rayHit.transform.CompareTag("Ground"))
            {
                transform.position = rayHit.point;
                rb.velocity = Vector3.zero;
            }
            else
            {
                rb.AddForce(gravityDir * (gravForce * Time.deltaTime));
            }
        }
        else
        {
            rb.AddForce(gravityDir * (gravForce * Time.deltaTime));
        }
    }

    private void OnEnable()
    {
        rb.isKinematic = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Ground")||other.transform.CompareTag("WaterMesh")||other.transform.CompareTag(transform.tag))
        {
            Invoke("Disable",1);

        }
    }

    public void Disable()
    {
        this.enabled = false;
        rb.isKinematic = true;
    }
}
