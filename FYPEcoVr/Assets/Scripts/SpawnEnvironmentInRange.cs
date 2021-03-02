using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnvironmentInRange : MonoBehaviour
{
    public float minDist = 5;

    public float radius = 10;

    public Transform core;
    public GameObject preFab;
    public float maxToSpawn = 200;

    public List<GameObject> spawned;
    public float timeBeforeCheck=0.01f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        spawned = new List<GameObject>();
        
        StartCoroutine(RepeatSpawn());
        StartCoroutine(RepeatDespawn());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetPoint()
    {
        Vector3 point = this.transform.position+(Random.insideUnitSphere * radius);
        Vector3 locYPoint = transform.InverseTransformPoint(point);
        locYPoint.y = 0;
        Vector3 cancelledYPoint = transform.TransformPoint(locYPoint);
        Vector3 awayDir = (cancelledYPoint-transform.position).normalized;
        Vector3 offSetPoint = cancelledYPoint + (awayDir * minDist);
        Vector3 aboveGroundPoint = offSetPoint+(transform.up*20);

        return aboveGroundPoint;
    }

    public void SpawnOnSurface(Vector3 point)
    {
        Vector3 gravityDir = (core.transform.position-transform.position).normalized;//todo flip dir

        RaycastHit hit;
        if (Physics.Raycast(point, gravityDir, out hit, 2000, 1 << 8))
        {
            GameObject ob = Instantiate(preFab, hit.point,Quaternion.identity);
            ob.transform.up = hit.normal;
            spawned.Add(ob);
        }
    }

    public void Spawn()
    {
        if (spawned.Count < maxToSpawn)
        {
            Vector3 point = GetPoint();
            SpawnOnSurface(point);
        }
    }

    //continuously check despawn but not everythign every frame
    IEnumerator RepeatSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBeforeCheck);
            Spawn();
        }
    }
    //continuously check despawn but not everythign every frame
    IEnumerator RepeatDespawn()
    {
        print("RepeatDespawn");
        int i = 0;
        while (true)
        {
            yield return new WaitForSeconds(timeBeforeCheck);
            if (spawned.Count>=1&&Vector3.Distance(spawned[i].transform.position, transform.position) > radius+minDist)
            {
                print("destroy");
                Destroy(spawned[i]);
                spawned.Remove(spawned[i]);
                i--;
            }
            else
            {
                print("no despawn");
            }
            
            i++;
            if (i>=spawned.Count-1)//loop back around
            {
                i = 0;
            }
        }
    }
    
}
