using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*For generating the planet if auto gen is on on gui
 */
public class AutoGen : MonoBehaviour
{
    public Toggle toggle;

    public PlanetSpawner Spawner;
    public void TryGen()
    {
        if (toggle.isOn)
        {
            Spawner.Generate();
        }
    }
}
