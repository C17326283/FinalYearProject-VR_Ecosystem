using System.Collections;
using System.Collections.Generic;
using Panda;
using UnityEngine;

public class AnimalDistanceDisabler : MonoBehaviour
{
    public Transform player;

    public Transform animal;
    public GameObject animalHolder;
    public BehaviourTree bt;//for applying since it didnt work without

    public float activeDistance = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.position, animal.position) > activeDistance)
        {
            animalHolder.gameObject.SetActive(false);
            //print("disable");
        }
        else
        {
            if (animalHolder.gameObject.activeInHierarchy == false)
            {
                animalHolder.gameObject.SetActive(true);
                bt.Apply();//start ai again
                //print("enable");

            }
        }
    }
}
