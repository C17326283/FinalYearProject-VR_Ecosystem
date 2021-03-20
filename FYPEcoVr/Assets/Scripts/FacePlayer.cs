using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform camPos;
    public float lerpSpeed = 4;
    public bool reverse = false;//Need this because some things such as guis face forward in wrong direction
    
    // Start is called before the first frame update
    void Start()
    {
        camPos = GameObject.FindWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos;//For holding point to look
        
        if (reverse)//Flip direction
        {
            relativePos = transform.position - camPos.transform.position;
        }
        else
        {
            relativePos = camPos.transform.position - transform.position;
        }
        
        Quaternion toRotation = Quaternion.LookRotation(relativePos, camPos.transform.up);//For holding new rotation
        transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, lerpSpeed * Time.deltaTime );

    }
}
