using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//The alctual core object
public class Planet : MonoBehaviour
{
    //resolution for the amount of square that makes up a face, max 256
    [Range(20, 256)] 
    public int res = 100;
    
    //settings to pull from for generation
    public PlanetSettings planetSettings;
    
    //Material genration based on input gradients//todo make this allow random gradients
    private ColourGenerator colourGenerator = new ColourGenerator();
    
    //The actual face objects for the terrain and the water
    [SerializeField, HideInInspector]
    private MeshFilter[] meshFilters;
    [SerializeField, HideInInspector]
    private MeshFilter[] waterMeshFilters;
    private TerrainFace[] terrainFaces;
    private TerrainFace[] waterFaces;//I definitely could have made the water faces in abetter way but i just generated terrain again without noise
    
    
    public TerrainMinMaxHeights elevationMinMax;//for getting the highest and lowest points
    public GameObject[] biomeObjs;//4
    

    //Make all the faces and filters if they arent already or update them if they are
    void Create()
    {
        colourGenerator.UpdateSettings(planetSettings);
        
        //make the 6 sides of the cube that will be spherized
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
            waterMeshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];
        waterFaces = new TerrainFace[6];
        
        
        //get all the directions to be used as the sides of the cube
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};
    
        //create objects and components for all the faces
        for (int i = 0; i < 6; i++)
        {
            UpdateSettings(planetSettings);
            //todo try to split face further
            if (meshFilters[i] == null)
            {
                //Make square faces and set details that will be used
                GameObject meshObj = new GameObject("PlanetFaceMesh");
                GameObject waterObj = new GameObject("WaterMesh");
                meshObj.transform.parent = transform;
                waterObj.transform.parent = transform;
                meshObj.transform.position = transform.position;
                waterObj.transform.position = transform.position;
                meshObj.transform.tag = "Ground";//for letting the spawners hit it
                waterObj.transform.tag = "Water";
            
                meshObj.AddComponent<MeshRenderer>();
                waterObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                waterMeshFilters[i] = waterObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                waterMeshFilters[i].sharedMesh = new Mesh();
                meshFilters[i].sharedMesh.name = "sharedMesh";
                waterMeshFilters[i].sharedMesh.name = "sharedWaterMesh";
                MeshCollider collider = meshObj.AddComponent<MeshCollider>();
                collider.sharedMesh = meshFilters[i].sharedMesh;
                MeshCollider waterCollider = waterObj.AddComponent<MeshCollider>();
                waterCollider.sharedMesh = waterMeshFilters[i].sharedMesh;
            }
            
            //Update the biome objects and shader
            SetBiomes();
            
            //update materials
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = planetSettings.planetMaterial;
            waterMeshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = planetSettings.waterMaterial;
            
            //add to list of faces
            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh,res,directions[i],elevationMinMax, planetSettings,true);
            waterFaces[i] = new TerrainFace(waterMeshFilters[i].sharedMesh,res,directions[i],elevationMinMax, planetSettings,false);

            //Reset the mesh colliders//Didnt work without this, raycasts were going straight through
            meshFilters[i].GetComponent<MeshCollider>().convex = true;
            meshFilters[i].GetComponent<MeshCollider>().convex = false;
            waterMeshFilters[i].GetComponent<MeshCollider>().convex = true;
            waterMeshFilters[i].GetComponent<MeshCollider>().convex = false;
        }
    }

    //Call the functions needed for planet
    public void GeneratePlanet()
    {
        Create();
        GenerateMesh();
        GenerateColours();//turn this back on, its just annoying for editing
        
    }
    
    //Make mesh from all terrain faces
    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
        foreach (TerrainFace face in waterFaces)
        {
            face.ConstructMesh();
        }
        colourGenerator.UpdateHeightInShader(elevationMinMax);
    }

    //Update textures
    void GenerateColours()
    {
        colourGenerator.UpdateTextureInShader(biomeObjs[0],biomeObjs[1],biomeObjs[2],biomeObjs[3]);
    }
    
    //Update the evevation info so shader can do proper height variation
    public void UpdateSettings(PlanetSettings settings)
    {
        elevationMinMax = new TerrainMinMaxHeights();
    }

    //Make the objects for biome distance
    public void SetBiomes()
    {
        
        if (biomeObjs == null)
        {
            biomeObjs =  new GameObject[4];
            for (int j = 0; j < 4; j++)
            {
                biomeObjs[j] = new GameObject("BiomePlacementObject");
                biomeObjs[j].transform.parent = this.transform;
            }
        }

        for (int i = 0; i < biomeObjs.Length; i++)
        {
            //Can have poles to begin with to simulate earth
            if (planetSettings.havePoles && i<2)
            {
                if (i == 0)
                {
                    biomeObjs[i].transform.position = transform.up * planetSettings.planetRadius;
                }
                else if(i==1)
                {
                    biomeObjs[i].transform.position = -transform.up * planetSettings.planetRadius;
                }
            }
            else
            {
                biomeObjs[i].transform.position = Random.onUnitSphere * planetSettings.planetRadius;
            }
        }
        
    }
}