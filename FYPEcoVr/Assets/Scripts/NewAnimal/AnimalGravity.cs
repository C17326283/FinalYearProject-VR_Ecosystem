using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGravity : MonoBehaviour
{
    public GameObject core;
    public float gravityStrength = 2000;//todo check correct
    public float upMultiplier = 600f;
    public Vector3 gravityDir;
    public float animalHeight = 2;
    public float animalLength = 2;
    public Rigidbody rb;
    public GameObject headHeightPosObj;
    
    private int layerMask;
    public List<GameObject> forcePoints;
    public bool InitializeOnStart = false;
    
    
    
    // Update is called once per frame
    void FixedUpdate()
    {
        gravityDir = (core.transform.position-transform.position).normalized;//todo flip dir

        AddGravity();
        GravityHeightPositioning();
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (InitializeOnStart)
        {
            headHeightPosObj = this.transform.gameObject;
            Initialize();
        }
    }

    public void Initialize()
    {
        //Get center of planet for orienting to
        core = GameObject.Find("Core");
        
        rb = GetComponent<Rigidbody>();
        layerMask = 1 << 8;//bit shift to get mask for raycasting so only on environment and not other animals
        
        
        //Default values for proper weights
        rb.mass = 100;
        rb.drag = 2;
        rb.angularDrag = 2;
        
        //Make 2 points, for front and back balancing
        forcePoints = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            GameObject p = new GameObject();
            forcePoints.Add(p);
            p.transform.parent = headHeightPosObj.transform.parent;
        }
        //temp assume 2 points, front and back
        forcePoints[0].transform.position = headHeightPosObj.transform.position;
        forcePoints[1].transform.position = headHeightPosObj.transform.position+(-transform.forward * (animalLength));
    }

    public void AddGravity()//todo fix this mess
    {
        RaycastHit hit;
        //Only add if theres environment below
        if (Physics.Raycast(transform.position, gravityDir, out hit, 2000, layerMask))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                rb.AddForce(gravityDir * ((gravityStrength) * Time.deltaTime * 100));
            }
            else if (hit.transform.CompareTag("Water"))//hit water
            {
                RaycastHit upHit;
                //check if its below terrain and add upfroce to correct
                if (Physics.Raycast(transform.position + (transform.up * 1000), gravityDir, out upHit, 2000, layerMask))
                {
                    if (upHit.transform.CompareTag("Ground"))
                    {
                        rb.AddForce(-gravityDir * ((gravityStrength) * Time.deltaTime*100));
                    }
                    else
                    {
                        rb.AddForce(gravityDir * ((gravityStrength) * Time.deltaTime*100));
                    }
                }
            }
        }
    }

    public void GravityHeightPositioning()
    {
        foreach (var point in forcePoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(point.transform.position, gravityDir, out hit, animalHeight*1.2f, layerMask))
            {
                Debug.DrawLine(point.transform.position, hit.point, Color.white);
                float upForce = 0;

                float desiredHeight = Mathf.Min(animalHeight*.8f,animalHeight-(rb.velocity.magnitude/100));//strides get bigger at faster speeds so animate lower body too
                desiredHeight = Mathf.Clamp(desiredHeight, animalHeight *.6f, animalHeight);
                
                upForce = Mathf.Abs(desiredHeight / Vector3.Distance(hit.point, point.transform.position))*2;
                upForce = Mathf.Clamp(upForce, 0f,10f);//Stop adding too much force
                
                rb.AddForceAtPosition(-gravityDir * (upForce * upMultiplier*Time.deltaTime),
                    point.transform.position, ForceMode.Acceleration);// * desiredHeight
            }
        }
    }
}
