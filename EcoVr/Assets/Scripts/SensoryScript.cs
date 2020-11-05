using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryScript : MonoBehaviour
{
    public CreatureTaskManager creatureTaskManager;//Script to pass the sensed objects back to
    public CreatureStats creatureStats;//Script to pass the sensed objects back to

    
    //If doesnt have creature task then get one from parent

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Untagged") && !creatureTaskManager.objSensedMemory.Contains(other.gameObject)) //Certain tags the animal shouldnt care about and it shouldnt add objs twice
        {
//            Debug.Log("Adding to sensed list"+other.transform.name);
            creatureTaskManager.objSensedMemory.Add(other.gameObject);
            StartCoroutine(MemoryLoss(other.gameObject));
        }
    }

    IEnumerator  MemoryLoss(GameObject obj)
    {
        while (creatureTaskManager.objSensedMemory.Contains(obj))
        {
            yield return new WaitForSeconds(creatureStats.memoryLossRate);
            //If too far away then remove it//also may have been removed elsewhere so check
            if (obj != null && Vector3.Distance(transform.position, obj.transform.position) > creatureStats.sensoryRange)
            {
                creatureTaskManager.objSensedMemory.Remove(obj);
//                Debug.Log("removed obj:"+obj+Vector3.Distance(transform.position, obj.transform.position));
            }
        }
        
    }
}
