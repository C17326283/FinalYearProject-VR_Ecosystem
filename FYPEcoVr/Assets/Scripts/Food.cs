using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float timeToReGrow = 10;
    public bool regrows = true;
    public int worth =2 ;
    
    
    public void isEaten()
    {
        StartCoroutine(Regrow());
        
    }

    IEnumerator Regrow()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(timeToReGrow);
        transform.GetChild(0).gameObject.SetActive(true);
        
    }
}
