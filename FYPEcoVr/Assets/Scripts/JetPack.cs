using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JetPack : MonoBehaviour
{
    public GameObject rig;
    private Rigidbody rigRb;
    private XrGravity rigGravity;
    public float jetForce = 100;
    public Transform controller;
    
    [SerializeField] InputActionReference controllerActionGrip;
    
    public float gripValue;
    public ParticleSystem particles;
    public int particleRateAtStart;
    public AudioSource audioSource;

    public float maxMagnitude = 100;
    public float skyMagLimit = 10;
    
    


    // Start is called before the first frame update
    void Awake()
    {
        rigRb = rig.GetComponent<Rigidbody>();
        rigGravity = rig.GetComponent<XrGravity>();

        //Create interactions for new input system
        controllerActionGrip.action.performed += GripPress;
        controllerActionGrip.action.canceled += GripCancel;
        if(controller==null)
            controller = this.transform;
        
        particles = controller.GetComponent<ParticleSystem>();
        particleRateAtStart = particles.main.maxParticles;


    }

    private void FixedUpdate()
    {
        var particlesMain = particles.main;
        if (gameObject.activeInHierarchy && rigGravity.allowedFly && rigGravity.hasCore && particles)
        {
//            print("jetpack mag"+rigRb.velocity.magnitude);
            if(rigRb.velocity.magnitude < maxMagnitude)
                rigRb.AddForce(controller.transform.forward * ((gripValue*jetForce) * Time.deltaTime));
            audioSource.volume = gripValue/2;
            
            particlesMain.maxParticles = Mathf.FloorToInt(gripValue*particleRateAtStart);

//            print("gripValue*jetForce"+(gripValue*jetForce));
        }
        else
        {
            gripValue = 0;
            particlesMain.maxParticles = 0;
            audioSource.volume = 0;
            //Vector3 locVel = rigRb.transform.InverseTransformDirection(rigRb.velocity);//Find velocity in relation to an object oriented to ground
            //locVel.y = locVel.y*0.99f;//lower the vel exponentially rather than cancelling because that is jarring
            //rigRb.velocity = rigRb.transform.TransformDirection(locVel);//set the new cancelled related velocity
            if(rigRb.velocity.magnitude > skyMagLimit)
                rigRb.velocity = rigRb.velocity * (0.9999f * Time.deltaTime);//This lowers it by 1% every frame to avoid the playerhaving a harsh stop
            

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
