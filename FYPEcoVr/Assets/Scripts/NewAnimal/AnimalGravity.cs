using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGravity : MonoBehaviour
{

    public AnimalBrain brain;

    public GameObject core;
    public float gravityStrength = 3000;//todo check correct
    public float upMultiplier = 25f;
    public Vector3 gravityDir;
    public float animalHeight = 2;
    public float animalLength = 2;
    public Rigidbody rb;
    public GameObject headHeightPosObj;
    
    private int layerMask;

    public List<GameObject> forcePoints;
    
    public Vector3 moveVel;//for velocity without the up and down force

    public bool InitializeOnStart = false;
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveVel);


    }
    
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
            Initialize();
    }

    public void Initialize()
    {
        //Get center of planet for orienting to
        if(core == null)
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

    public void AddGravity()
    {
        RaycastHit hit;
        //Only add if theres environment below
        if (Physics.Raycast(transform.position, gravityDir, out hit, 2000, layerMask))
        {
            rb.AddForce(gravityDir * (gravityStrength));
        }
    }

    public void GravityHeightPositioning()
    {
        foreach (var point in forcePoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(point.transform.position, gravityDir, out hit, animalHeight, layerMask))
            {
                Debug.DrawRay(point.transform.position, gravityDir, Color.green);
                float upForce = 0;
                upForce = Mathf.Abs(1 / ((hit.point.y - point.transform.position.y)));
                upForce = Mathf.Clamp(upForce, 0f,2f);//Stop adding too much force
                float desiredHeight = Mathf.Min(animalHeight*.9f,(animalHeight / rb.velocity.magnitude)*3);//strides get bigger at faster speeds so animate lower body too
                desiredHeight = Mathf.Clamp(desiredHeight, animalHeight *.6f, animalHeight);
                
                rb.AddForceAtPosition(-gravityDir * (upForce * upMultiplier * desiredHeight),
                    point.transform.position, ForceMode.Acceleration);
            }
        }
    }
}
