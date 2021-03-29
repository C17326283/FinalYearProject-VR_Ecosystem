using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomHandController : MonoBehaviour
{
    [SerializeField] ActionBasedController controller;
    
    [SerializeField] Animator _handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<ActionBasedController>();
        //_handAnimator.avatar = controller.modelPrefab.GetComponentInChildren<Avatar>();
        _handAnimator = GetComponent<Animator>();

        //bool isPressed = controller.selectAction.action.ReadValue<bool>();

        controller.selectAction.action.performed += GripPress;
        controller.activateAction.action.performed += TriggerPress;
    }

    private void TriggerPress(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("Trigger",obj.ReadValue<float>());
        print("Trigger"+obj.ReadValue<float>());
    }

    private void GripPress(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("Grip",obj.ReadValue<float>());
        print("Grip"+obj.ReadValue<float>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
