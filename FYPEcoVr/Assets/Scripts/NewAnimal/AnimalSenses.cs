using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script to add nearby objs to a 
public class AnimalSenses : MonoBehaviour
{
    public AnimalBrain brain;



    //When it senses something, refresh its forgetting, 
    private void OnTriggerEnter(Collider other)
    {
        if (brain.forgettingObjs.Count> 0 && brain.forgettingObjs.Contains(other.gameObject)) //Certain tags the animal shouldnt care about and it shouldnt add objs twice
        {
            brain.forgettingObjs.Remove(other.gameObject);
        }
        else if (other != this.transform.parent && !other.transform.CompareTag("Untagged") && !other.transform.CompareTag("Ground") && !brain.objSensedMemory.Contains(other.gameObject)) //Certain tags the animal shouldnt care about and it shouldnt add objs twice
        {
            brain.objSensedMemory.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (brain.objSensedMemory.Contains(other.gameObject)) //Certain tags the animal shouldnt care about and it shouldnt add objs twice
        {
            brain.forgettingObjs.Add(other.gameObject);
            StartCoroutine(MemoryLoss(other.gameObject));
        }
    }
    
    

    IEnumerator  MemoryLoss(GameObject obj)
    {
        //Wait before deleting to simulate memory and give a chance to retrigger
        yield return new WaitForSeconds(brain.memoryLossRate);
        
        //If too far away then remove it//also may have been removed elsewhere so check
        if (brain.forgettingObjs.Contains(obj))
        {
            brain.objSensedMemory.Remove(obj);
            brain.forgettingObjs.Remove(obj);
        }

    }
}
