using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadObjs : MonoBehaviour
{
    public List<GameObject> objs;
    public float LoadDist = 1000;

    private void Update()
    {
        foreach (var obj in objs)
        {
            if (Vector3.Distance(this.transform.position, obj.transform.position) < LoadDist)
            {
                obj.SetActive(true);

            }
            else
            {
                obj.SetActive(false);

            }
        }
    }
}
