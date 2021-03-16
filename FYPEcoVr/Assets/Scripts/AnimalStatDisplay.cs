using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStatDisplay : MonoBehaviour
{
    
    //public Transform animalToDisplay;
    public GameObject statCanvas;
    
    // Start is called before the first frame update
    void Start()
    {
        statCanvas = GameObject.Find("StatCanvas");
    }
    

    public void SetToPos(GameObject animal)
    {
        /*
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            Debug.Log("hit:"+hit.transform.name);
        }
        */

        statCanvas.transform.position = animal.transform.position + (animal.transform.transform.up);
    }
}
