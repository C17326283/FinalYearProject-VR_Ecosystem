
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TeleporterController : MonoBehaviour
{
    public GameObject baseController;

    public GameObject teleporterController;
    
    public GameObject jetpackController;

    public InputActionReference teleportInputRef;
    public InputActionReference jetpackInputRef;

    public UnityEvent onTeleportActivate;
    public UnityEvent onTeleportCancel;
    
    public UnityEvent onjetpackActivate;
    public UnityEvent onjetpackCancel;

    public bool jetpackEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        teleportInputRef.action.performed += TeleportModeActivate;
        teleportInputRef.action.canceled += TeleportModeCancel;
        jetpackInputRef.action.performed += JetpackModeSwitch;
    }
    
    private void TeleportModeActivate(InputAction.CallbackContext obj)
    {
        if(jetpackEnabled == false)//only enabled if this is the active controller
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
    
    private void JetpackModeSwitch(InputAction.CallbackContext obj)
    {
        print("JetpackModeSwitch");
        jetpackEnabled = !jetpackEnabled;//switch state 
        
        if (jetpackEnabled)
        {
            onjetpackActivate.Invoke();
            print("is enabled");
        }
        else
        {
            onjetpackCancel.Invoke();
            print("is disabled");

        }
    }
    
}
