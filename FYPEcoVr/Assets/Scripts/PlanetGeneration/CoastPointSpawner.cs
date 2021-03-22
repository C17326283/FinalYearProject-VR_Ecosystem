using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoastPointSpawner : MonoBehaviour
{
    //public GameObject water;
    public bool runOnStart = false;
    public Vector3 gravityDir;
    public GameObject core;
    private int layerMask;


    public float rayStartHeight = 2f;
    public float rayDist = 2f;
    public int incrementAmount =2;

    public int res=0;
    public float overlapBoxSize = 30f;

    public LayerMask waterObjLayer;
    public bool alsoDetectAboveLand = true;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if(runOnStart)
            Run();
        
        
         
    }

    public void Run()
    {
        waterObjLayer = 1 << 10;//Get water obj layer which is 10
        
        if(res == 0)
            res = GetComponent<MeshFilter>().mesh.vertices.Length;
        layerMask = 1 << 8;
        core = GameObject.Find("Core");
        FindEdges();
        //Too many gameobjects would be an issue so do this to check a reasonable amount
        incrementAmount = Mathf.Max(Mathf.FloorToInt (res / 50),1);//Extra increment for every 50, min 1
        //StartCoroutine(Ray());
    }
    public void FindEdges()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        int vertCount = vertices.Length;
//        Debug.Log("vertCount "+vertCount);
        for (int i = 0; i < vertCount; i+=incrementAmount)
        {
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            gravityDir = (core.transform.position - worldPos).normalized;
            
            RaycastHit hit;
            
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(worldPos + (gravityDir * 0.02f), gravityDir, out hit, rayDist))
            {
                if (hit.transform.name != transform.name)//if not a water mesh
                {
                    //Spawn if not overlapping
                    noOverLapSpawn(worldPos);
                }
            }
            //If didnt hit and also check underwater is checked and does hit underwater
            else if(alsoDetectAboveLand)//start slightly underwater
            {
                if (Physics.Raycast(worldPos+(-gravityDir*rayStartHeight), gravityDir, out hit, rayDist))
                {
                    if (hit.transform.name != transform.name) //if not a water mesh
                    {
                        noOverLapSpawn(hit.point);
                    }
                }
            }
            else
            {
                //Debug.DrawRay(vertices[i], gravityDir,Color.red,rayDist);
//                print("no hit: " + vertices[i]);
            }
        }
    }

    public void noOverLapSpawn(Vector3 spawnPos)
    {
        //Avoid overlap spawning, https://www.youtube.com/watch?v=lFmKZskl45I
        Vector3 overlapTestBoxScale = new Vector3(overlapBoxSize,overlapBoxSize,overlapBoxSize);
        Collider[] collidersInOverlapBox = new Collider[1];
        int numOfCollidersFound = Physics.OverlapBoxNonAlloc(spawnPos, overlapTestBoxScale,collidersInOverlapBox,this.transform.rotation,waterObjLayer);

        //Only spawn if no other objects in range
        if (numOfCollidersFound < 1)
        {
            Spawn(spawnPos);
        }
        else
        {
            print("no spawn, overlap. cols found"+numOfCollidersFound+", size"+overlapTestBoxScale);
        }
    }

    public void Spawn(Vector3 spawnPos)
    {
        GameObject obj = new GameObject("water");
        obj.transform.position = spawnPos;
        obj.transform.tag = "Water";
        obj.layer = 10;
        SphereCollider col =obj.AddComponent<SphereCollider>();//so it can be sensed
        col.radius = 0.1f;
    }

    
}
