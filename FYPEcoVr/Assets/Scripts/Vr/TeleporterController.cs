using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TeleporterController : MonoBehaviour
{
    public GameObject baseController;

    public GameObject teleporterController;

    public InputActionReference teleportInputRef;

    public UnityEvent onTeleportActivate;

    public UnityEvent onTeleportCancel;
    // Start is called before the first frame update
    void Start()
    {
        teleportInputRef.action.performed += TeleportModeActivate;
        teleportInputRef.action.canceled += TeleportModeCancel;
    }
    
    private void TeleportModeActivate(InputAction.CallbackContext obj)
    {
        onTeleportActivate.Invoke();
    }


    private void TeleportModeCancel(InputAction.CallbackContext obj)
    {
        Invoke("DeactivateTeleporter", .1f);
    }
    
    void DeactivateTeleporter()
    {
        onTeleportCancel.Invoke();
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
