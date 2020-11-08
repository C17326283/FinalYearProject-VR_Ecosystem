using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Range(0,100)]
    public float timeSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DisplayNums",0,10);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeSpeed != 1.0f)
        {
            Time.timeScale = timeSpeed;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }

    void DisplayNums()
    {
        Debug.Log(GameObject.FindGameObjectsWithTag("Fox").Length+" Foxes, "+GameObject.FindGameObjectsWithTag("Chicken").Length+" Chickens  ,"+Time.time);
    }
}
