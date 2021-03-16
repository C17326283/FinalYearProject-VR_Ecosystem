using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AddStatDetector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        XRSimpleInteractable interactable = gameObject.AddComponent<XRSimpleInteractable>();
        
        //interactable.onSelectExited()
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
