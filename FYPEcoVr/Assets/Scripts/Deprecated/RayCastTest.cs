using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Just for testign the mesh colliders, dont need in finished
public class RayCastTest : MonoBehaviour
{
    public Camera camera;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            Debug.Log(hit.transform.name);
            
            // Do something with the object that was hit by the raycast.
        }
    }
}
