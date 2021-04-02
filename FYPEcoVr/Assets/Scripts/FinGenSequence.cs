using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class FinGenSequence : MonoBehaviour
{
    public PlanetSpawner PlanetSpawner;

    public GameObject loadingGUI;
    
    public GameObject spaceCam;
    public GameObject nonVrPlayer;
    public GameObject nonVrPlayerCam;
    public GameObject VrPlayer;
    public TeleportPlanetOrienter VrOrienter;

    public LightAtAngle[] lights;
    public GameObject viewingRoom;
    public Slider progressSlider;

    public XrGravity xrGravity;
    public TextMeshProUGUI progressText;

    public void Perform()
    {
        StartCoroutine(StartSequence());
    }
    
    IEnumerator StartSequence()
    {
        IncreaseLoadProgress(5, "Starting Generation");
        loadingGUI.SetActive(true);
        yield return new WaitForSeconds(.1f);

        IncreaseLoadProgress(5, "Generating Colliders");
        PlanetSpawner.Explore();
        yield return new WaitForSeconds(1f);
        
        IncreaseLoadProgress(5, "Generating Atmosphere");
        StartCoroutine(PlanetSpawner.AddExtras(this));
        yield return new WaitForSeconds(6f);//this needs to be longer than the waits inside the add extras function
        
        IncreaseLoadProgress(5, "Generating Lighting");
        //Turn on here because this relies on player being on planet
        foreach (var light in lights)
        {
            light.enabled = true;
        }
        yield return new WaitForSeconds(.5f);
        
        IncreaseLoadProgress(10, "Finalising & Teleporting player");
        SpawnPlayer();
        viewingRoom.SetActive(false);
        yield return new WaitForSeconds(.1f);
        
        //StartCoroutine(SpawnPlayer());
    }

    public void IncreaseLoadProgress(float amount, String statusText)
    {
        progressSlider.value += amount;
        progressText.text = progressSlider.value+"% - "+statusText;
        print("slider val: "+progressSlider.value);
    }

    public void SpawnPlayer()
    {
        if (spaceCam.activeInHierarchy)
        {
            nonVrPlayer.SetActive(true);
        
            loadingGUI.SetActive(false);
            spaceCam.SetActive(false);
            nonVrPlayerCam.SetActive(true);
            print("spawning non vr player");
        }
        else//is vr player
        {
            print("spawning vr player");

            GetPointOnPlanet pointFinder = GameObject.Find("Core").GetComponent<GetPointOnPlanet>();
            RaycastHit? hitPoint = pointFinder.GetPoint("Ground", 1000);
            if (hitPoint != null)
            {
                GameObject core = GameObject.Find("Core");
                RaycastHit hit = hitPoint.Value;
                loadingGUI.SetActive(false);
                VrPlayer.transform.position = hit.point;
                VrPlayer.transform.parent = core.transform;
                VrOrienter.core = core;
                VrOrienter.enabled = true;
                xrGravity.core = core;
                xrGravity.enabled = true;
            }
            else
            {
                print("couldnt find spawn point");
            }
            
        }
    }
}
