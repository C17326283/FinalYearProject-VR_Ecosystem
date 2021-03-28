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

    public bool allowedFly = true;

    // Start is called before the first frame update
    void Awake()
    {
        //Create interactions for new input system
        controllerActionGrip.action.performed += GripPress;
        if(controller==null)
            controller = this.transform;

        
    }

    private void GripPress(InputAction.CallbackContext obj)
    {
        if (gameObject.activeInHierarchy && allowedFly)
        {
            //_handAnimator.SetFloat("Grip",obj.ReadValue<float>());
            print("jet" + obj.ReadValue<float>());
            float forceAmount = obj.ReadValue<float>() * jetForce;
            rigRb.AddForce(controller.transform.up * forceAmount * Time.deltaTime);
        }
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
