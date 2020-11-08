using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script to add nearby objs to a 
public class SensoryScript : MonoBehaviour
{
    public CreatureTaskManager creatureTaskManager;//Script to pass the sensed objects back to
    public CreatureStats creatureStats;//Script to pass the sensed objects back to
    public List<GameObject> forgettingObjs = new List<GameObject>();//List for objects that animal is no longer touching but are still in memory

    
    //If doesnt have creature task then get one from parent
    private void Start()
    {
        if (creatureTaskManager == null)
            creatureTaskManager = this.transform.parent.GetComponent<CreatureTaskManager>();
        if (creatureStats == null)
            creatureStats = this.transform.parent.GetComponent<CreatureStats>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (forgettingObjs.Count> 0 && forgettingObjs.Contains(other.gameObject)) //Certain tags the animal shouldnt care about and it shouldnt add objs twice
        {
            forgettingObjs.Remove(other.gameObject);
        }
        else if (!other.transform.CompareTag("Untagged") && other.gameObject != this.transform.parent.gameObject && !creatureTaskManager.objSensedMemory.Contains(other.gameObject)) //Certain tags the animal shouldnt care about and it shouldnt add objs twice
        {
            creatureTaskManager.objSensedMemory.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (creatureTaskManager.objSensedMemory.Contains(other.gameObject)) //Certain tags the animal shouldnt care about and it shouldnt add objs twice
        {
            forgettingObjs.Add(other.gameObject);
            StartCoroutine(MemoryLoss(other.gameObject));
        }
    }
    
    

    IEnumerator  MemoryLoss(GameObject obj)
    {
        //Wait before deleting to simulate memory and give a chance to retrigger
        yield return new WaitForSeconds(creatureStats.memoryLossRate);
        
        //If too far away then remove it//also may have been removed elsewhere so check
        if (forgettingObjs.Contains(obj))
        {
            creatureTaskManager.objSensedMemory.Remove(obj);
            forgettingObjs.Remove(obj);
        }

    }
}
