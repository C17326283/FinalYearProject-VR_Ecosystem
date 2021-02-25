using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
    public float timeToWait = 1;
    void Start()
    {
        //Start the coroutine we define below named ExampleCoroutine.
        StartCoroutine(DeactivateAfterX());
    }

    IEnumerator DeactivateAfterX()
    {
        print("start");
        yield return new WaitForSeconds(timeToWait);
        this.gameObject.SetActive(false);
    }
}
