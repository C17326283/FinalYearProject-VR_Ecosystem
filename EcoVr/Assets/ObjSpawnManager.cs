using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjSpawnManager : MonoBehaviour {

    public int targetCount = 5;
    public float radius = 10;
    public GameObject spawnPrefab;
    public String tag;
    public float waveSpawnTime = 5;
    
    private int layerMask;//Mask for choosing what layer the raycast hits

    IEnumerator WaveSpawner()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            GameObject[] allObjs = GameObject.FindGameObjectsWithTag(tag);
            if (allObjs.Length < targetCount)
            {
                Spawn();
            }
            yield return new WaitForSeconds(waveSpawnTime);
        }
    }

	// Use this for initialization
	void Start ()
    {
        layerMask = 1 << 8;//THis is bit shifting layer 8 so that only hit colliders on layer 8
        InitialFill();
        StartCoroutine(WaveSpawner());
        
    }

    void InitialFill()
    {
        for (int i = 0; i < targetCount; i++)
        {
            Spawn();
        }
            
    }
    
    void Spawn()
    {
        Vector2 spawnPointOnCirleXY = Random.insideUnitCircle * radius;//Important to check the rotation of obj
        Vector3 worldSpawnPoint = new Vector3(spawnPointOnCirleXY.x,this.gameObject.transform.position.y, spawnPointOnCirleXY.y);//convert x,y to x,y,z
        
//        Debug.Log(worldSpawnPoint);
        RaycastHit hit;
        if (Physics.Raycast(worldSpawnPoint, Vector3.down, out hit, 50,layerMask))
        {
            GameObject obj = GameObject.Instantiate(spawnPrefab);
            obj.transform.position = hit.point;
        }
    }
}
