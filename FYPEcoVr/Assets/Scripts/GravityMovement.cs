using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GravityMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed;
    public float defaultSpeed = 50;
    public float turnSpeed=150;
    public GameObject core;
    public Vector3 gravityDir;

    public Vector3 rawInputMovement;
    public float turnVal;
    public PlayerInputManager inputAction;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        core = GameObject.Find("Core");

    }

    // Update is called once per frame
    void Update()
    {
        /*
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
        */
        gravityDir = (core.transform.position-transform.position).normalized;//todo flip dir
        
        //move
        rb.AddRelativeForce(rawInputMovement*moveSpeed);
        
        //transform.up = -gravityDir;
        
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up,-gravityDir)*transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,50*Time.deltaTime);

        //turn
        //transform.Rotate(0, turnVal * turnSpeed*Time.deltaTime, 0,Space.Self);
        //transform.Rotate( new Vector3(0, turnVal * turnSpeed, 0), Space.Self );
        transform.RotateAround(transform.position, transform.up, turnVal* turnSpeed * Time.deltaTime);
        
        //print("rawInputMovement"+rawInputMovement);
        

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
        if(context.performed)
            print("debugging2");
        if(context.canceled)
            print("debugging3");
    }
}
