using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoGen : MonoBehaviour
{
    public Toggle toggle;

    public PlanetSpawner Spawner;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryGen()
    {
        if (toggle.isOn)
        {
            Spawner.Generate();
        }
    }
}
