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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        yield return new WaitForSeconds(1f);
        SpawnPlayer();
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
            int layerMask = 1 << 8;
            
            

            //Get point on top of planet
            RaycastHit hit;//todo fix incase water
            //Only add if theres environment below
            if (Physics.Raycast(new Vector3(0,5000,0), -transform.up, out hit, 6000,
                layerMask))
            {
                print("Found spawn point");
                loadingGUI.SetActive(false);
                VrPlayer.transform.position = hit.point;
                VrPlayer.transform.parent = GameObject.Find("Core").transform;
                VrOrienter.core = GameObject.Find("Core");
                VrOrienter.enabled = true;
            }
            else
            {
                print("couldnt find spawn point");
            }
        }
    }
}
