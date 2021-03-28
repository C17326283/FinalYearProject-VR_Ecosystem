using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Escape : MonoBehaviour
{
    
    public void EscapeApplication(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Quit();
        }
    }

    //Can be called from gui button press or escape key
    public void Quit()
    {
        print("escape");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#endif
    }
}
