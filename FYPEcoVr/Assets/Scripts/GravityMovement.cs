using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed;
    public float defaultSpeed = 25;
    public float turnSpeed=150;
    public GameObject core;
    public Vector3 gravityDir;

    public Vector3 rawInputMovement;
    public float turnVal;
    
    private int layerMask;



    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 8;//bit shift to get mask for raycasting so only on environment and not other animals

        rb = GetComponent<Rigidbody>();
        core = GameObject.Find("Core");
        moveSpeed = defaultSpeed;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, gravityDir, out hit, 2000, 1 << 8))
        {
            transform.position = hit.point + (transform.up * 5);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gravityDir = (core.transform.position-transform.position).normalized;//todo flip dir
        
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 500, 1 << 8))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up,hit.normal)*transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,5*Time.deltaTime);

            transform.RotateAround(transform.position, transform.up, turnVal* turnSpeed * Time.deltaTime);
        }
        
        //move
        rb.AddRelativeForce(rawInputMovement * (moveSpeed * Time.deltaTime*2),ForceMode.Acceleration);

        
    }
    
    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInputMovement = new Vector3(inputMovement.x,0,inputMovement.y);
        
        //print("onmovement"+rawInputMovement);
        //rb.AddForce(rawInputMovement*defaultSpeed);
    }
    
    public void OnTurn(InputAction.CallbackContext value)
    {
        turnVal = value.ReadValue<float>();
        
        //print("onmovement"+rawInputMovement);
        //rb.AddForce(rawInputMovement*defaultSpeed);
    }
    
    public void Debug(InputAction.CallbackContext context)
    {
        if(context.started)
            print("debugging1");
        if (context.performed)
        {
            rb.AddRelativeForce(-gravityDir * (moveSpeed*Time.deltaTime*200),ForceMode.Impulse);
        }
            
            print("debugging2");
        if(context.canceled)
            print("debugging3");
    }
    
    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.started)
            moveSpeed = defaultSpeed * 3;
        if(context.canceled)
            moveSpeed = defaultSpeed;
    }
}
