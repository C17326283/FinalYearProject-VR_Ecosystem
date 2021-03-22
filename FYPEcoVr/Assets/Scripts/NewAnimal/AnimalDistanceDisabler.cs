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
    public float aiDistance = 200;

    public SkinnedMeshRenderer renderer;
    public List<AnimalFeetPositioner> FeetPositioners;

    public AnimalAudioManager audioManager;

    public CapsuleCollider col;
    // Start is called before the first frame update
    void Start()
    {
        renderer = animal.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        FeetPositioners = animalHolder.GetComponent<AnimalInitializer>().feetPositioners;
        audioManager = animal.gameObject.GetComponent<AnimalAudioManager>();
        col = animal.gameObject.GetComponent<CapsuleCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(player.position, animal.position);
        
        //too far away turn off non essential scripts
        if (dist > activeDistance)
        {
            if (renderer.enabled)
            {
                audioManager.StopAudio();
                col.enabled = false;
                renderer.enabled = false;
                foreach (var foot in FeetPositioners)
                {
                    foot.enabled = false;
                }
            }
            
            //way too far away, stop running ai
            if (dist > aiDistance)
            {
                animalHolder.gameObject.SetActive(false);
            }
            else if (animalHolder.gameObject.activeInHierarchy == false)
            {
                animalHolder.gameObject.SetActive(true);
                bt.Apply();//start ai again
            }
        }
        else//renable
        {
            if (animalHolder.gameObject.activeInHierarchy == false)
            {
                animalHolder.gameObject.SetActive(true);
                bt.Apply();//start ai again
                audioManager.StartAudio();
                
                

            }
            if (renderer.enabled == false)
            {
                renderer.enabled = true;
                col.enabled = true;
                foreach (var foot in FeetPositioners)
                {
                    foot.enabled = true;
                }
            }
        }
    }
    
}
