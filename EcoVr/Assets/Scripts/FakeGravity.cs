using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGravity : MonoBehaviour
{
    public GameObject gravityObject;
    public float gravityAmount = -20;
    public Rigidbody rb; 
    
    // Start is called before the first frame update
    void Start()
    {
        
        if (rb == null)
        {
            rb = transform.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
        else
        {
            rb = transform.GetComponent<Rigidbody>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Vector3 gravityUp = (this.transform.position - gravityObject.transform.position).normalized;
        rb.AddForce(gravityUp*gravityAmount);
        Vector3 objUp = transform.up;

        Quaternion targetRotation = Quaternion.FromToRotation(objUp,gravityUp)*transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,50*Time.deltaTime);
*/
    }
}
