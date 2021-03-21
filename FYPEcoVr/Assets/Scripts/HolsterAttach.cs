using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterAttach : MonoBehaviour
{
    public GameObject holster;

    public float lerpSpeed = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, holster.transform.position, Time.deltaTime * lerpSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, holster.transform.rotation, Time.deltaTime * lerpSpeed);
    }
}