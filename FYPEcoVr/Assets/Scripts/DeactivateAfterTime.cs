using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
    public float timeToWait = 1;
    public bool destroyInsteadOfDisable = false;
    void OnEnable()
    {
        //Start the coroutine we define below named ExampleCoroutine.
        Invoke(nameof(DeactivateAfterX),timeToWait);//coroutine didnt work
    }

    public void DeactivateAfterX()
    {
        if (destroyInsteadOfDisable)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);

        }
    }
}
