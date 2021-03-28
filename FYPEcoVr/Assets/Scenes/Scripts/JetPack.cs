using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JetPack : MonoBehaviour
{
    public GameObject rig;
    public Rigidbody rigRb;
    public float jetForce = 100;
    public Transform controller;
    
    [SerializeField] InputActionReference controllerActionGrip;
    
    public float gripValue;

    public bool allowedFly = true;

    // Start is called before the first frame update
    void Awake()
    {
        //Create interactions for new input system
        controllerActionGrip.action.performed += GripPress;
        controllerActionGrip.action.canceled += GripCancel;
        if(controller==null)
            controller = this.transform;

        
    }

    private void FixedUpdate()
    {
        if (gameObject.activeInHierarchy && allowedFly)
        {
            rigRb.AddForce(controller.transform.up * ((gripValue*jetForce) * Time.deltaTime));
        
//            print("gripValue*jetForce"+(gripValue*jetForce));
        }
        else
        {
            gripValue = 0;
            Vector3 locVel = rigRb.transform.InverseTransformDirection(rigRb.velocity);//Find velocity in relation to an object oriented to ground
            locVel.y = locVel.y*0.99f;//lower the vel exponentially rather than cancelling because that is jarring
            rigRb.velocity = rigRb.transform.TransformDirection(locVel);//set the new cancelled related velocity

        }
    }

    private void GripPress(InputAction.CallbackContext obj)
    {
        gripValue = obj.ReadValue<float>();
        //_handAnimator.SetFloat("Grip",obj.ReadValue<float>());
//        print("jet" + obj.ReadValue<float>());
    }
    
    private void GripCancel(InputAction.CallbackContext obj)
    {
        gripValue = 0;
    }
    
    //[SerializeField] InputActionReference controllerActionGrip;
    
    /*
    // Start is called before the first frame update
    void Awake()
    {
        //Create interactions for new input system
        controllerActionGrip.action.performed += Jetting;

        
    }

    private void Jetting(InputAction.CallbackContext context)
    {
        if (context.started)
            print("jetting");
            
        
        rigRb.AddForce(rig.transform.up*context.ReadValue<float>()*Time.deltaTime,ForceMode.Acceleration);
        
    }
    */
}
