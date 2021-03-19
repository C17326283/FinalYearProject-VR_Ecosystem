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


    public float rayDist = 4f;
    public int incrementAmount =2;

    public int res=100;
    // Start is called before the first frame update
    void Start()
    {
        if(runOnStart)
            Run();
         
    }

    public void Run()
    {
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
        Debug.Log("vertCount "+vertCount);
        for (int i = 0; i < vertCount; i+=incrementAmount)
        {
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            gravityDir = (core.transform.position - worldPos).normalized;
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(worldPos+(-gravityDir*rayDist), gravityDir, out hit, rayDist))
            {
                if (hit.transform.name != transform.name)//if not a water mesh
                {
                    //Debug.DrawRay(vertices[i], gravityDir,Color.red,rayDist);
                    GameObject obj = new GameObject("water");
                    obj.transform.tag = "Water";
                    SphereCollider col =obj.AddComponent<SphereCollider>();//so it can be sensed
                    col.radius = 0.1f;
                    obj.transform.position = hit.point;
                    print("hit");
                }
            }
            else
            {
                //Debug.DrawRay(vertices[i], gravityDir,Color.red,rayDist);
                print("no hit: " + vertices[i]);
            }
        }
    }

    
}
