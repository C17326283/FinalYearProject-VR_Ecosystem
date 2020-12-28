using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Just for making something look at soemthing else

public class LookAtLerp : MonoBehaviour
{
    public GameObject gameObjectToLookAt;
    public float lerpSpeed = 2f;
    
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 relativePos = gameObjectToLookAt.transform.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, lerpSpeed * Time.deltaTime );
    }
}
