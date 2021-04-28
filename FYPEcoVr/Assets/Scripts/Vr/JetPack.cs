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
            //The force is applied
            rigRb.AddForce(controller.transform.forward * ((gripValue*jetForce) * Time.deltaTime),ForceMode.Force);
            rigRb.velocity = Vector3.ClampMagnitude(rigRb.velocity,maxMagnitude);//prevent goinf too fast
            
            audioSource.volume = gripValue/4;//audio
            particlesMain.maxParticles = Mathf.FloorToInt(gripValue*particleRateAtStart);//particles
        }
        else
        {
            gripValue = 0;
            particlesMain.maxParticles = 0;
            audioSource.volume = 0;
            //This lowers it by .01% every frame to avoid the playerhaving a harsh stop
            if(rigRb.velocity.magnitude > skyMagLimit)
                rigRb.velocity = rigRb.velocity * (0.9999f * Time.deltaTime);
        }
    }

    private void GripPress(InputAction.CallbackContext obj)
    {
        gripValue = obj.ReadValue<float>();
    }
    
    private void GripCancel(InputAction.CallbackContext obj)
    {
        gripValue = 0;
    }
    
}
