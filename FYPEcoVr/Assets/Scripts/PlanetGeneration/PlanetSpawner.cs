using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// This is for spawning the planet object and managing the generation as a whole
public class PlanetSpawner : MonoBehaviour
{
    //All the references
    public GameObject player;
    [HideInInspector] 
    public GameObject planetObj;
    private PlanetTerrainGenerator planetScript;
    public PlanetSettings planetSettings;
    public PlanetSettings defaultPlanetSettings;//The default eath like settings to copy from for the start
    public Material mat;
    public Material waterMat;
    public Material atmosphereMat;
    //public GameObject extras;//Spawners
    public GameObject allSpawnersObjects;//object in scene thats the parent of all the spawners

    private GameObject spawnedExtras;

    public bool finishedAddingExtras = false;//todo find better way than this

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
    public GameObject noiseCyclesOption;
    private Slider noiseCyclesSlider;
    private TextMeshProUGUI noiseCyclesTextMeshPro;
    public GameObject minValueOption;
    private Slider minValueSlider;
    private TextMeshProUGUI minValueTextMeshPro;
    //for the detail settings
    public GameObject strengthOption2;
    private Slider strengthSlider2;
    private TextMeshProUGUI strengthTextMeshPro2;
    public GameObject roughnessOption2;
    private Slider roughnessSlider2;
    private TextMeshProUGUI roughnessTextMeshPro2;
    public GameObject persistanceOption2;
    private Slider persistanceSlider2;
    private TextMeshProUGUI persistanceTextMeshPro2;
    public GameObject baseRoughnessOption2;
    private Slider baseRoughnessSlider2;
    private TextMeshProUGUI baseRoughnessTextMeshPro2;
    public GameObject noiseCyclesOption2;
    private Slider noiseCyclesSlider2;
    private TextMeshProUGUI noiseCyclesTextMeshPro2;
    public GameObject minValueOption2;
    private Slider minValueSlider2;
    private TextMeshProUGUI minValueTextMeshPro2;

    //get the references of the sliders and text
    public void Start()
    {
        AssignGUI();
    }

    // Make the planet object, adding compontents adn materials
    public void Generate()
    {
        if (planetObj == null)
        {
            planetSettings = Instantiate(defaultPlanetSettings);
            Debug.Log("Gen planet");
            planetObj = new GameObject("Spawned planet");
            planetObj.transform.parent = this.transform;
            planetScript = planetObj.AddComponent<PlanetTerrainGenerator>();
            planetScript.planetSettings = planetSettings;
            planetScript.planetSettings.planetMaterial = mat;
            planetScript.planetSettings.waterMaterial = waterMat;
            ResetSettings();
            planetScript.planetSettings.planetRadius *= 10;//make 10 times bigger
        }
        updateSettingsFromGUI();
        planetScript.GenerateFace();
    }

    public void AssignGUI()
    {
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
        noiseCyclesSlider = noiseCyclesOption.GetComponentInChildren<Slider>();
        noiseCyclesTextMeshPro = noiseCyclesOption.GetComponentInChildren<TextMeshProUGUI>();
        minValueSlider = minValueOption.GetComponentInChildren<Slider>();
        minValueTextMeshPro = minValueOption.GetComponentInChildren<TextMeshProUGUI>();
        
        strengthSlider2 = strengthOption2.GetComponentInChildren<Slider>();
        strengthTextMeshPro2 = strengthOption2.GetComponentInChildren<TextMeshProUGUI>();
        roughnessSlider2 = roughnessOption2.GetComponentInChildren<Slider>();
        roughnessTextMeshPro2 = roughnessOption2.GetComponentInChildren<TextMeshProUGUI>();
        persistanceSlider2 = persistanceOption2.GetComponentInChildren<Slider>();
        persistanceTextMeshPro2 = persistanceOption2.GetComponentInChildren<TextMeshProUGUI>();
        baseRoughnessSlider2 = baseRoughnessOption2.GetComponentInChildren<Slider>();
        baseRoughnessTextMeshPro2 = baseRoughnessOption2.GetComponentInChildren<TextMeshProUGUI>();
        noiseCyclesSlider2 = noiseCyclesOption2.GetComponentInChildren<Slider>();
        noiseCyclesTextMeshPro2 = noiseCyclesOption2.GetComponentInChildren<TextMeshProUGUI>();
        minValueSlider2 = minValueOption2.GetComponentInChildren<Slider>();
        minValueTextMeshPro2 = minValueOption2.GetComponentInChildren<TextMeshProUGUI>();
    }


    //Chnage the settings when something is updated on screen
    public void updateSettingsFromGUI()
    {
        planetSettings.res = (int)resSlider.value;
        planetSettings.noiseLayers[0].strength = strengthSlider.value;
        planetSettings.noiseLayers[0].roughness = roughnessSlider.value;
        planetSettings.noiseLayers[0].persistance = persistanceSlider.value;
        planetSettings.noiseLayers[0].baseRoughness = baseRoughnessSlider.value;
        planetSettings.noiseLayers[0].NumOfNoiseCycles = (int)noiseCyclesSlider.value;
        planetSettings.noiseLayers[0].minValue = minValueSlider.value;
        
        planetSettings.noiseLayers[1].strength = strengthSlider2.value;
        planetSettings.noiseLayers[1].roughness = roughnessSlider2.value;
        planetSettings.noiseLayers[1].persistance = persistanceSlider2.value;
        planetSettings.noiseLayers[1].baseRoughness = baseRoughnessSlider2.value;
        planetSettings.noiseLayers[1].NumOfNoiseCycles = (int)noiseCyclesSlider2.value;
        planetSettings.noiseLayers[1].minValue = minValueSlider2.value;
    }
    
    //sets the gui details whenever the sliders are moved
    public void UpdateGUIDetails()
    {
        resTextMeshPro.text = "Resolution: "+System.Math.Round(resSlider.value,2);
        
        strengthTextMeshPro.text = "Strength: "+System.Math.Round(strengthSlider.value,2);
        roughnessTextMeshPro.text = "Roughness: "+System.Math.Round(roughnessSlider.value,2);
        persistanceTextMeshPro.text = "Amplification: "+System.Math.Round(persistanceSlider.value,2);
        baseRoughnessTextMeshPro.text = "Continent Spread: "+System.Math.Round(baseRoughnessSlider.value,2);
        noiseCyclesTextMeshPro.text = "Cycles: "+System.Math.Round(noiseCyclesSlider.value,2);
        minValueTextMeshPro.text = "Sea Amount: "+System.Math.Round(minValueSlider.value,2);
        
        strengthTextMeshPro2.text = "Strength: "+System.Math.Round(strengthSlider2.value,2);
        roughnessTextMeshPro2.text = "Roughness: "+System.Math.Round(roughnessSlider2.value,2);
        persistanceTextMeshPro2.text = "Amplification: "+System.Math.Round(persistanceSlider2.value,2);
        baseRoughnessTextMeshPro2.text = "Continent Spread: "+System.Math.Round(baseRoughnessSlider2.value,2);
        noiseCyclesTextMeshPro2.text = "Cycles: "+System.Math.Round(noiseCyclesSlider2.value,2);
        minValueTextMeshPro2.text = "Mountain Amount: "+System.Math.Round(minValueSlider2.value,2);
    }
    
    //sets the value and slider at beginning from the base template
    public void ResetSettings()
    {
        
        planetSettings.res = defaultPlanetSettings.res;
        resSlider.value = planetSettings.res;
        
        planetSettings.noiseLayers[0].strength = defaultPlanetSettings.noiseLayers[0].strength;
        strengthSlider.value = defaultPlanetSettings.noiseLayers[0].strength;
        planetSettings.noiseLayers[0].roughness = defaultPlanetSettings.noiseLayers[0].roughness;
        roughnessSlider.value = defaultPlanetSettings.noiseLayers[0].roughness;
        planetSettings.noiseLayers[0].persistance = defaultPlanetSettings.noiseLayers[0].persistance;
        persistanceSlider.value = defaultPlanetSettings.noiseLayers[0].persistance;
        planetSettings.noiseLayers[0].baseRoughness = defaultPlanetSettings.noiseLayers[0].baseRoughness;
        baseRoughnessSlider.value = defaultPlanetSettings.noiseLayers[0].baseRoughness;
        planetSettings.noiseLayers[0].NumOfNoiseCycles = defaultPlanetSettings.noiseLayers[0].NumOfNoiseCycles;
        noiseCyclesSlider.value = defaultPlanetSettings.noiseLayers[0].NumOfNoiseCycles;
        planetSettings.noiseLayers[0].minValue = defaultPlanetSettings.noiseLayers[0].minValue;
        minValueSlider.value = defaultPlanetSettings.noiseLayers[0].minValue;
        
        planetSettings.noiseLayers[1].strength = defaultPlanetSettings.noiseLayers[1].strength;
        strengthSlider2.value = defaultPlanetSettings.noiseLayers[1].strength;
        planetSettings.noiseLayers[1].roughness = defaultPlanetSettings.noiseLayers[1].roughness;
        roughnessSlider2.value = defaultPlanetSettings.noiseLayers[1].roughness;
        planetSettings.noiseLayers[1].persistance = defaultPlanetSettings.noiseLayers[1].persistance;
        persistanceSlider2.value = defaultPlanetSettings.noiseLayers[1].persistance;
        planetSettings.noiseLayers[1].baseRoughness = defaultPlanetSettings.noiseLayers[1].baseRoughness;
        baseRoughnessSlider2.value = defaultPlanetSettings.noiseLayers[1].baseRoughness;
        planetSettings.noiseLayers[1].NumOfNoiseCycles = defaultPlanetSettings.noiseLayers[1].NumOfNoiseCycles;
        noiseCyclesSlider2.value = defaultPlanetSettings.noiseLayers[1].NumOfNoiseCycles;
        planetSettings.noiseLayers[1].minValue = defaultPlanetSettings.noiseLayers[1].minValue;
        minValueSlider2.value = defaultPlanetSettings.noiseLayers[1].minValue;
        
        planetSettings.noiseLayers[0].centre = defaultPlanetSettings.noiseLayers[0].centre;
        planetSettings.havePoles = true;
        
        UpdateGUIDetails();
        Generate();
    }
    
    
    //Adding the spawner objects which fill the terrain with trees etc
    public IEnumerator AddExtras(FinGenSequence genSequenceScript)
    {
        print("planet gen "+Time.time);
        GameObject core = new GameObject("Core");
        GetPointOnPlanet getPointOnPlanetFinder = core.AddComponent<GetPointOnPlanet>();
        BiomeManager biomeManager = core.AddComponent<BiomeManager>();
        biomeManager.biomeObjs = planetScript.biomeObjs;
        SunAngle sunAngle = core.AddComponent<SunAngle>();

        //add atmosphere
        GameObject atmosphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        atmosphere.GetComponent<Collider>().enabled = false;
        atmosphere.transform.localScale = (Vector3.one * (planetSettings.planetRadius*2)) + (Vector3.one * (planetObj.GetComponent<PlanetTerrainGenerator>().elevationMinMax.Max/2));//make it outside the radius
        atmosphere.GetComponent<Renderer>().material = atmosphereMat;
        atmosphere.layer = 9;//atmosphere layer for reverse lighting
        FlipNormals(atmosphere);
        yield return new WaitForSeconds(.2f);
        float numOfSpawnerObjects = allSpawnersObjects.transform.childCount;

        //add spawners
        foreach (Transform spawnerObj in allSpawnersObjects.transform)
        {
            print("spawning "+spawnerObj.transform.name);

            if (spawnerObj.GetComponent<RandomGenSpawner>() != null)
            {
                genSequenceScript.IncreaseLoadProgress(40/(numOfSpawnerObjects-1),"Generating "+spawnerObj.transform.name);

                RandomGenSpawner sp = spawnerObj.GetComponent<RandomGenSpawner>();
                GameObject holder = new GameObject("holder");
                sp.parentObject = holder.gameObject;
                sp.planetObject = this.gameObject;
                sp.biomeManager = biomeManager;
                sp.getPointOnPlanetFinder = getPointOnPlanetFinder;
                if (sp.isRotatingObject)
                    holder.AddComponent<RotateEnvironment>();
                sp.TriggerSpawn();
            }
            else if (spawnerObj.GetComponent<AnimalsSpawner>() != null)
            {
                spawnerObj.GetComponent<AnimalsSpawner>().TriggerSpawn(genSequenceScript);
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(.5f);

        }
        
        planetObj.GetComponent<PlanetTerrainGenerator>().GenerateCoastline();
        print("planet fin "+Time.time);
        finishedAddingExtras = true;
        

    }

    

    //Make a different center point for the noise, this makes different terrain with all the same settings
    public void MoveLand()
    {
        planetSettings.noiseLayers[0].centre = new Vector3(Random.Range(-1000.0f, 1000.0f), Random.Range(-1000.0f, 1000.0f), Random.Range(-1000.0f, 1000.0f));
        Generate();
    }

    //Start playing by hiding the guis and giving access to the movement of the camera
    public void Explore()
    {
        Generate();
        planetObj.GetComponent<PlanetTerrainGenerator>().GenerateColliders();
        planetObj.GetComponent<PlanetTerrainGenerator>().AddTeleporting();
        
        
        this.GetComponent<RotateEnvironment>().enabled = false;
        //Tried to get bettr player controller but it didnt work
        //player.transform.position = (planet.transform.up * planetSettings.planetRadius)+ (Vector3.up * 110);
        //player.GetComponent<SimpleCameraController>().enabled = true;
        //FakeGravity fg = player.AddComponent<FakeGravity>();
        //fg.gravityObject = planet;

    }
    

    //Move the biome points
    public void RandomiseBiomes()
    {
        
        planetSettings.havePoles = false;
        planetObj.GetComponent<PlanetTerrainGenerator>().SetBiomes();
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
