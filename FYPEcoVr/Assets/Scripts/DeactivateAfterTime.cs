using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
    public float timeToWait = 1;
    public bool DestroyInsteadOfDisable = false;
    void Start()
    {
        //Start the coroutine we define below named ExampleCoroutine.
        Invoke("DeactivateAfterX",timeToWait);//coroutine didnt work
    }

    public void DeactivateAfterX()
    {
        if (DestroyInsteadOfDisable)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);

        }
    }
}
