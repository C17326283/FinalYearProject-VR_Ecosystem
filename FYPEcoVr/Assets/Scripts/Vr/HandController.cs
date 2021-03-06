using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField] InputActionReference controllerActionGrip;
    [SerializeField] InputActionReference controllerActionTrigger;

    public Animator _handAnimator;
    // Start is called before the first frame update
    void Awake()
    {
        //Create interactions for new input system
        controllerActionGrip.action.performed += GripPress;
        controllerActionTrigger.action.performed += TriggerPress;

        _handAnimator = GetComponent<Animator>();
        
    }

    private void TriggerPress(InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("Trigger",obj.ReadValue<float>());
        print(obj.ReadValue<float>());
        
    }

    private void GripPress(InputAction.CallbackContext obj)
    {
        _handAnimator.SetFloat("Grip",obj.ReadValue<float>());
    }
}
