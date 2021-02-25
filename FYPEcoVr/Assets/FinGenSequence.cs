using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinGenSequence : MonoBehaviour
{
    public PlanetSpawner PlanetSpawner;

    public GameObject loadingGUI;
    public GameObject spaceCam;

    public GameObject player;
    public GameObject playerCam;

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
        
        yield return new WaitForSeconds(5f);
        foreach (var light in lights)
        {
            light.enabled = true;
        }
        player.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        spaceCam.SetActive(false);
        loadingGUI.SetActive(false);
        playerCam.SetActive(true);
    }
}
