using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For holding a list of objects to be called randomly and adding them back here when they are despawned.
public class ObjectPool : MonoBehaviour
{

    //prefab that the pool will use
    //public GameObject poolPrefab;
    public int poolIndex;
    public List<GameObject> poolPrefabList;
    public List<List<GameObject>> lists;

    //initial number of element
    public int initialNum = 0;
    public int maxAmountActive = 100;

    //collection
    List<GameObject> pooledObjects;

    //init pool
    void Awake()
    {
        // if the pool has already been init, don't init again
        if(pooledObjects == null)
        {
            InitPool();
        }
    }

    //init pool for size chosen
    public void InitPool()
    {
        //Function may be called multiple times so make sure only functions once
        if (pooledObjects == null)
        {
            pooledObjects = new List<GameObject>();
            
            // create this initial number of objects
            for (int i = 0; i < initialNum; i++)
            {
                // create a new object
                CreateObj();
            }
        }
        
    }

    //create a new object
    GameObject CreateObj()
    {
        // create a new object
        GameObject newObj = Instantiate(getRandomFromPrefabList());

        // set this new object to inactive
        newObj.SetActive(false);

        // add it to the list
        pooledObjects.Add(newObj);

        return newObj;
    }

    //For choosing a random prefab out of the prefablist
    GameObject getRandomFromPrefabList()
    {
        GameObject randomPrefab;
        if (poolPrefabList.Count > 1)
        {
            int randNum = Random.Range(0, poolPrefabList.Count);//gets a random number for position in prefablist
            randomPrefab = poolPrefabList[randNum];
        }
        else
        {
            randomPrefab = poolPrefabList[0];
        }
        return randomPrefab;

    }

    // retrieve an object from the pool
    public GameObject GetObj()
    {
        //Debug.Log("gameobject: "+this.gameObject.name+ ", getNumOfActiveObjects(): "+getNumOfActiveObjects()+", maxAmountActive: "+maxAmountActive);
        //Check doesnt excede maximum spawn capacity
        if (getNumOfActiveObjects() < maxAmountActive)
        {
            // search our list for an inactive object
            for(int i = 0; i < pooledObjects.Count; i++)
            {
                // if we find an inactive object
                if(!pooledObjects[i].activeInHierarchy)
                {
                    //enable it (set it to active)
                    pooledObjects[i].SetActive(true);

                    // return that object
                    return pooledObjects[i];//Exits code here so doesnt go on to try make new object
                }
            }

            // increase our pool (create a new object)
            GameObject newObj = CreateObj();

            // enable that new object
            newObj.SetActive(true);//temp false

            // return that object
            return newObj;
        }
        else
        {
//            Debug.Log(this.gameObject.name+" is trying to spawn object above pool capacity");
        }

        return null;
    }

    
    //For active and deactive stuff to be used later with animals, from old project
    // get all active objects
    public List<GameObject> GetAllActive()
    {
        List<GameObject> activeObjs;
        activeObjs = new List<GameObject>();

        // search our list for active objects
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // if we find an active object
            if (pooledObjects[i].activeInHierarchy)
            {
                activeObjs.Add(pooledObjects[i]);
            }
        }

        return activeObjs;
    }

    public int getNumOfActiveObjects()//TODO make for efficient using get all actives
    {
        int amountActiveObjs = 0;
        
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // if we find an active object
            if (pooledObjects[i].activeInHierarchy)
            {
                amountActiveObjs++;
            }
        }
        return amountActiveObjs;
    }
}