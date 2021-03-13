﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using URandom = UnityEngine.Random;

//Making the indivual faces
public class TerrainFace : MonoBehaviour
{
    //private ShapeGenerator shapeGenerator;
    private Mesh mesh;
    private int res;

    //info about direction of face
    private Vector3 localUp;
    private Vector3 axisA;
    private Vector3 axisB;
    
    //All the settings that are needed
    private PlanetSettings settings;
    private NoiseLayer[] noiseLayers;
    private TerrainMinMaxHeights elevationMinMax;
    private Boolean isLand;
    private float waterHeight =0.3f;



    //constructor for initalising the terrain face parameters
    public TerrainFace(Mesh mesh, int res, Vector3 localUp, TerrainMinMaxHeights elevationMinMax,PlanetSettings planetSettings, Boolean isLand)
    {
        this.mesh = mesh;
        this.res = res;
        this.localUp = localUp;
        this.elevationMinMax = elevationMinMax;
        this.isLand = isLand;
        this.settings = planetSettings;
        this.noiseLayers = planetSettings.noiseLayers;//copy all noise layers from planetSettings
        
        
        axisA = new Vector3(localUp.y, localUp.z,localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    //Make the actual mesh of the face with verticies and triangles
    public void ConstructMesh()
    {
        //make an array for verticies and triangles based on the resolution
        Vector3[] vertices = new Vector3[res*res];
        int[] triangles = new int[(res-1)*(res-1)*6];//-1 to avoid the points at end that dont need triangles
        int triIndex = 0;//Index for each individual point

        //edit each vertex to the right position on sphere
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                int i = x + y * res;//get the point on the grid
                Vector2 percent = new Vector2(x,y) / (res-1);//percentage of width for even spacing
                Vector3 pointOnUnitCube;
                
                //If not edge of side then shift point on cube to make them non uniform
                if (x > 0 && x < res - 1 && y > 0 && y < res - 1)
                {
                    //float movement = 0;//shift up to 25% in each direction

                    float movement = .28f/res;//shift up to 25% in each direction
                    //Debug, too expensive to do every verticie
                    if (i % 500 == 1)
                    {
                        //print(movement);
                    }
                    pointOnUnitCube = localUp + (URandom.Range(percent.x-movement,percent.x+movement) - .5f)*2*axisA + (URandom.Range(percent.y-movement,percent.y+movement) - .5f)*2*axisB;//get position of individual point

                }
                else
                {
                    pointOnUnitCube = localUp + (percent.x - .5f)*2*axisA + (percent.y - .5f)*2*axisB;//get position of individual point

                }
                
                
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;//normalise it to get where it should be on sphere
                //use the spherized point with noise to find where it should be
                if (isLand)
                {
                    
                    vertices[i] = AddHeightToVertex(pointOnUnitSphere);
                    //todo add movement to points for better uneven low poly
                    /*
                    if (x > 0 && x < res - 1 && y > 0 && y < res - 1)
                    {
                        float movement = 5;
                        //movement = Mathf.Clamp(movement,0, (percent.x));
                        vertices[i] = localUp + vertices[i]+new Vector3(URandom.Range(-movement,movement),URandom.Range(-movement,movement),URandom.Range(-movement,movement));
                    }
                    */
                    
                    
                }
                else
                {
                    vertices[i] = pointOnUnitSphere * (settings.planetRadius+waterHeight+5f);//default height +height the water is at+extra to cover bottom colour layer
                }

                
                    

                
                //get trianle points from points on mesh
                if(x != res-1 &&  y != res-1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex+1] = i+res+1;
                    triangles[triIndex+2] = i+res;
                    
                    triangles[triIndex+3] = i;
                    triangles[triIndex+4] = i+1;
                    triangles[triIndex+5] = i+res+1;

                    triIndex += 6;
                }
            }
        }
        
        //get the mesh points from the made points
        mesh.Clear();//clear data from mesh so no errors
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
    
    //Addign radius and noise to point to make it the surface of the planet
    public Vector3 AddHeightToVertex(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;
        
        //Use the previous layers as a mask so spikes go on top of other mountains not randomly
        if (noiseLayers.Length > 0)
        {
            firstLayerValue = noiseLayers[0].AddNoise(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }
            
            
        for (int i = 1; i < noiseLayers.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;//? if true do this else set default of 1
                elevation += noiseLayers[i].AddNoise(pointOnUnitSphere) * mask;
            }
        }

        elevation = settings.planetRadius * (1 + elevation);
        //Debug.Log(elevation);
        //check if is exact radius, if so then lower it to allow for the water mesh to be fitted properly
        if (elevation < settings.planetRadius+waterHeight)
        {
            //dont evaluated point so the colours dont rely on water level
            elevation = (elevation)-10f;
            return pointOnUnitSphere * elevation;
        }
        else
        {
            elevationMinMax.AddValue(elevation);
            return pointOnUnitSphere * elevation;
        }

    }
}