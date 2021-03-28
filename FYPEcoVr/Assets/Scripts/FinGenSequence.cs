using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public XrGravity xrGravity;

    public void Perform()
    {
        StartCoroutine(StartSequence());
    }
    
    IEnumerator StartSequence()
    {
        loadingGUI.SetActive(true);
        yield return new WaitForSeconds(.1f);
        
        PlanetSpawner.Explore();
        yield return new WaitForSeconds(.1f);
        
        StartCoroutine(PlanetSpawner.AddExtras());
        print("after while"+PlanetSpawner.finishedAddingExtras);
        
        yield return new WaitForSeconds(1f);
        foreach (var light in lights)
        {
            light.enabled = true;
        }
        yield return new WaitForSeconds(2f);
        SpawnPlayer();
        viewingRoom.SetActive(false);
        //StartCoroutine(SpawnPlayer());
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
