using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGravity : MonoBehaviour
{
    public Vector3 gravityDir;
    public GameObject core;
    public float gravForce = 5000;
    public Rigidbody rb;

    

    // Start is called before the first frame update
    void Start()
    {
        core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir
        rb.AddForce(gravityDir * (gravForce * Time.deltaTime));

    }
}
