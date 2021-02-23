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
        gravityDir = (core.transform.position-transform.position).normalized;//todo flip dir
        
        //move
        rb.AddRelativeForce(rawInputMovement * (moveSpeed * Time.deltaTime*2),ForceMode.Acceleration);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, gravityDir, out hit, 2000, 1 << 8))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up,hit.normal)*transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,5*Time.deltaTime);

            transform.RotateAround(transform.position, transform.up, turnVal* turnSpeed * Time.deltaTime);
        }
        
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
