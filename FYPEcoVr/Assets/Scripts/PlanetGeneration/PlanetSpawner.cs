using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplateProjects;
using Random = UnityEngine.Random;

// This is for spawning the planet object and managing the generation as a whole
public class PlanetSpawner : MonoBehaviour
{
    //All the references
    public GameObject player;
    [HideInInspector] 
    public GameObject planet;
    public PlanetSettings planetSettings;
    public PlanetSettings defaultPlanetSettings;//The default eath like settings to copy from for the start
    public Material mat;
    public Material waterMat;
    public Material atmosphereMat;
    public GameObject extras;//Spawners

    private GameObject spawnedExtras;
    private Planet planetScript;

    //The references to the gui objects that updates the settings
    public GameObject resolutionOption;
    private Slider resSlider;
    private TextMeshProUGUI resTextMeshPro;
    public GameObject strengthOption;
    private Slider strengthSlider;
    private TextMeshProUGUI strengthTextMeshPro;
    public GameObject roughnessOption;
    private Slider roughnessSlider;
    private TextMeshProUGUI roughnessTextMeshPro;
    public GameObject persistanceOption;
    private Slider persistanceSlider;
    private TextMeshProUGUI persistanceTextMeshPro;
    public GameObject baseRoughnessOption;
    private Slider baseRoughnessSlider;
    private TextMeshProUGUI baseRoughnessTextMeshPro;
    
    public GameObject noiseLayersOption;
    private Slider noiseLayersSlider;
    private TextMeshProUGUI noiseLayersTextMeshPro;

    //Make the objects and get the references of the sliders and text
    public void Start()
    {
        planetSettings = Instantiate(defaultPlanetSettings);
        resSlider = resolutionOption.GetComponentInChildren<Slider>();
        resTextMeshPro = resolutionOption.GetComponentInChildren<TextMeshProUGUI>();
        strengthSlider = strengthOption.GetComponentInChildren<Slider>();
        strengthTextMeshPro = strengthOption.GetComponentInChildren<TextMeshProUGUI>();
        roughnessSlider = roughnessOption.GetComponentInChildren<Slider>();
        roughnessTextMeshPro = roughnessOption.GetComponentInChildren<TextMeshProUGUI>();
        persistanceSlider = persistanceOption.GetComponentInChildren<Slider>();
        persistanceTextMeshPro = persistanceOption.GetComponentInChildren<TextMeshProUGUI>();
        baseRoughnessSlider = baseRoughnessOption.GetComponentInChildren<Slider>();
        baseRoughnessTextMeshPro = baseRoughnessOption.GetComponentInChildren<TextMeshProUGUI>();
        
        noiseLayersSlider = noiseLayersOption.GetComponentInChildren<Slider>();
        noiseLayersTextMeshPro = noiseLayersOption.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Make the planet object, adding compontents adn materials
    public void Generate()
    {
        if (planet == null)
        {
            planet = new GameObject("Spawned planet");
            planet.transform.parent = this.transform;
            planetScript = planet.AddComponent<Planet>();
            planetScript.planetSettings = planetSettings;
            planetScript.planetSettings.planetMaterial = mat;
            planetScript.planetSettings.waterMaterial = waterMat;
            setDefaultSettings();
            planetScript.planetSettings.planetRadius *= 10;//make 10 times bigger
        }
        

        updateSettingsFromGUI();
        planetScript.GeneratePlanet();
    }

    //Adding the spawner objects which fill the terrain with trees etc
    public void AddExtras()
    {
        if (spawnedExtras == null)
        {
            spawnedExtras = Instantiate(extras, planet.transform);
            GameObject atmosphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            atmosphere.GetComponent<Collider>().enabled = false;
            atmosphere.transform.localScale = (Vector3.one * (planetSettings.planetRadius*2)) + (Vector3.one * (planet.GetComponent<Planet>().elevationMinMax.Max/2));//make it outside the radius
            atmosphere.GetComponent<Renderer>().material = atmosphereMat;
            atmosphere.layer = 8;//atmosphere layer for reverse lighting
            FlipNormals(atmosphere);

        }
    }

    
    //Chnage the settings when something is updated on screen
    public void updateSettingsFromGUI()
    {
        planetScript.res = (int)resSlider.value;
        planetSettings.noiseLayers[0].strength = strengthSlider.value;
        planetSettings.noiseLayers[0].roughness = roughnessSlider.value;
        planetSettings.noiseLayers[0].persistance = persistanceSlider.value;
        planetSettings.noiseLayers[0].baseRoughness = baseRoughnessSlider.value;
        
        planetSettings.noiseLayers[0].NumOfNoiseCycles = (int)noiseLayersSlider.value;
    }
    
    //sets the value and slider at beginning from the base template
    public void setDefaultSettings()
    {
        planetScript.res = 100;
        resSlider.value = planetScript.res;
        
        planetSettings.noiseLayers[0].strength = defaultPlanetSettings.noiseLayers[0].strength;
        strengthSlider.value = defaultPlanetSettings.noiseLayers[0].strength;

        planetSettings.noiseLayers[0].roughness = defaultPlanetSettings.noiseLayers[0].roughness;
        roughnessSlider.value = defaultPlanetSettings.noiseLayers[0].roughness;
        planetSettings.noiseLayers[0].persistance = defaultPlanetSettings.noiseLayers[0].persistance;
        persistanceSlider.value = defaultPlanetSettings.noiseLayers[0].persistance;
        planetSettings.noiseLayers[0].baseRoughness = defaultPlanetSettings.noiseLayers[0].baseRoughness;
        baseRoughnessSlider.value = defaultPlanetSettings.noiseLayers[0].baseRoughness;
        
        planetSettings.noiseLayers[0].NumOfNoiseCycles = defaultPlanetSettings.noiseLayers[0].NumOfNoiseCycles;
        noiseLayersSlider.value = defaultPlanetSettings.noiseLayers[0].NumOfNoiseCycles;
        UpdateGUIDetails();
    }

    //sets the gui details whenever the sliders are moved
    public void UpdateGUIDetails()
    {
        
        resTextMeshPro.text = "Resolution: "+System.Math.Round(resSlider.value,2);
        strengthTextMeshPro.text = "Strength: "+System.Math.Round(strengthSlider.value,2);
        roughnessTextMeshPro.text = "Roughness: "+System.Math.Round(roughnessSlider.value,2);
        persistanceTextMeshPro.text = "Amplification: "+System.Math.Round(persistanceSlider.value,2);
        baseRoughnessTextMeshPro.text = "Continent Spread: "+System.Math.Round(baseRoughnessSlider.value,2);
        noiseLayersTextMeshPro.text = "Cycles: "+System.Math.Round(noiseLayersSlider.value,2);
    }

    //Make a different center point for the noise, this makes different terrain with all the same settings
    public void MoveLand()
    {
        planetSettings.noiseLayers[0].centre = new Vector3(Random.Range(-1000.0f, 1000.0f), Random.Range(-1000.0f, 1000.0f), Random.Range(-1000.0f, 1000.0f));
        Generate();
    }

    //Set the values back to what the were at the beginning
    public void Reset()
    {
        planetScript.res = 100;
        resSlider.value = planetScript.res;
        
        planetSettings.noiseLayers[0].strength = defaultPlanetSettings.noiseLayers[0].strength;
        strengthSlider.value = defaultPlanetSettings.noiseLayers[0].strength;

        planetSettings.noiseLayers[0].roughness = defaultPlanetSettings.noiseLayers[0].roughness;
        roughnessSlider.value = defaultPlanetSettings.noiseLayers[0].roughness;
        planetSettings.noiseLayers[0].persistance = defaultPlanetSettings.noiseLayers[0].persistance;
        persistanceSlider.value = defaultPlanetSettings.noiseLayers[0].persistance;
        planetSettings.noiseLayers[0].baseRoughness = defaultPlanetSettings.noiseLayers[0].baseRoughness;
        baseRoughnessSlider.value = defaultPlanetSettings.noiseLayers[0].baseRoughness;
        
        planetSettings.noiseLayers[0].NumOfNoiseCycles = defaultPlanetSettings.noiseLayers[0].NumOfNoiseCycles;
        noiseLayersSlider.value = defaultPlanetSettings.noiseLayers[0].NumOfNoiseCycles;
        planetSettings.noiseLayers[0].centre = defaultPlanetSettings.noiseLayers[0].centre;
        planetSettings.havePoles = true;
        Generate();
    }

    //Start playing by hiding the guis and giving access to the movement of the camera
    public void Explore()
    {
        Generate();
        planet.GetComponent<Planet>().GenerateColliders();
        this.GetComponent<RotateEnvironment>().enabled = false;
        //Tried to get bettr player controller but it didnt work
        //player.transform.position = (planet.transform.up * planetSettings.planetRadius)+ (Vector3.up * 110);
        player.GetComponent<SimpleCameraController>().enabled = true;
        //FakeGravity fg = player.AddComponent<FakeGravity>();
        //fg.gravityObject = planet;

    }

    //Move the biome points
    public void RandomiseBiomes()
    {
        
        planetSettings.havePoles = false;
        planet.GetComponent<Planet>().SetBiomes();
        Generate();
    }

    //Flip the normals of the atmosphere object to allow an inward material
    public void FlipNormals(GameObject atmoObj)
    {
        //get normals from mesh
        Mesh mesh = atmoObj.GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;
        //flip mesh
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -1 * normals[i];
        }

        mesh.normals = normals;
        //reverse clockwise triangles
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] tris = mesh.GetTriangles(i);
            for (int j = 0; j < tris.Length; j+=3)
            {
                int temp = tris[j];
                tris[j] = tris[j + 1];
                tris[j + 1] = temp;
            }
            mesh.SetTriangles(tris,i);
        }

    }
}
