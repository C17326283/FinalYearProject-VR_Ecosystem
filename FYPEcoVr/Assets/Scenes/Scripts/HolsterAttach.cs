using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterAttach : MonoBehaviour
{
    public GameObject holster;

    public float lerpSpeed = 5;

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, holster.transform.position, Time.deltaTime * lerpSpeed);
        if(Vector3.Distance(transform.position, holster.transform.position)>0.05f)
            transform.position = Vector3.Lerp(transform.position, holster.transform.position, Time.deltaTime * lerpSpeed);

            
        transform.rotation = Quaternion.Lerp(transform.rotation, holster.transform.rotation, Time.deltaTime * lerpSpeed);
    }
}
