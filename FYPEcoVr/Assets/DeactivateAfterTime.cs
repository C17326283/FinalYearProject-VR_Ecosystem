using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
    public float timeToWait = 1;
    public bool DestoryInsteadOfDisable = false;
    void Start()
    {
        //Start the coroutine we define below named ExampleCoroutine.
        StartCoroutine(DeactivateAfterX());
    }

    IEnumerator DeactivateAfterX()
    {
        print("start");
        yield return new WaitForSeconds(timeToWait);
        if (DestoryInsteadOfDisable)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);

        }
    }
}
