using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGravity : MonoBehaviour
{
    public GameObject core;

    public float gravityStrength = 600; //todo check correct

    //public float upMultiplier = 600f;
    public Vector3 gravityDir;
    public float animalHeight = 2;
    public float animalLength = 2;
    public Rigidbody rb;
    public GameObject headHeightPosObj;

    private int layerMask;
    public List<GameObject> forcePoints;
    public bool InitializeOnStart = false;

    public List<AnimalFeetPositioner> footPositioners;



    // Update is called once per frame
    void FixedUpdate()
    {
        gravityDir = (core.transform.position - transform.position).normalized; //todo flip dir

        AddGravity();
        //GravityHeightPositioning();
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
        layerMask = 1 << 8; //bit shift to get mask for raycasting so only on environment and not other animals

        footPositioners = new List<AnimalFeetPositioner>();

        
        //Default values for proper weights
        rb.mass = 100;
        rb.drag = 2;
        rb.angularDrag = 2;

        //Make 2 points, for front and back balancing
        forcePoints = new List<GameObject>();
        for (int i = 0; i < 2; i++)
        {
            GameObject p = new GameObject("force point");
            forcePoints.Add(p);
            p.transform.parent = headHeightPosObj.transform;//.parent
        }

        //temp assume 2 points, front and back
        forcePoints[0].transform.position = headHeightPosObj.transform.position;
        forcePoints[1].transform.position = headHeightPosObj.transform.position + (-transform.forward * (animalLength));
    }

    public void AddGravity() //todo fix this mess
    {
        foreach (var point in forcePoints)
        {
            RaycastHit hit;
            //Only add if theres environment below
            if (Physics.Raycast(point.transform.position + (-gravityDir * 100), gravityDir, out hit, 5000, layerMask))
            {
                
                float furthestFootDist = GetFurthestFootDist();

                //get height based on magnitude or default height
                //float desiredHeight = Mathf.Min(animalHeight * .9f, animalHeight - (rb.velocity.magnitude / 30)); //strides get bigger at faster speeds so animate lower body too

                float desiredHeight = animalHeight-((furthestFootDist)/1.5f); //height based on stride
 //               print("desiredHeight"+transform.name+desiredHeight);
                desiredHeight = Mathf.Clamp(desiredHeight, animalHeight * .7f, animalHeight * .9f);



                //get a distance away from target than can be used to reduce force// *8 to decrease range it affects
                float distForce = Vector3.Distance(hit.point + (transform.up * desiredHeight), point.transform.position) * 5; //find dist between current and desired point

//                    print("distFroce"+distForce);

                float gravForce = Mathf.Min(gravityStrength * distForce, gravityStrength); //if close to point then add less force

                Vector3 dir = (hit.point + (-gravityDir * (desiredHeight)) - point.transform.position).normalized;

                rb.AddForceAtPosition(dir * (gravForce * Time.deltaTime), point.transform.position,
                    ForceMode.Acceleration);

            }
        }
    }

    public float GetFurthestFootDist()
    {
        //print(footPositioners);
        float fDist =Mathf.Infinity;

        for (int i = 0; i < footPositioners.Count; i++)
        {
            //print("foot" + footPositioners[i]);
            if (footPositioners[i].distToNext < fDist)
            {
                fDist = Mathf.Abs(footPositioners[i].axisDifferences.z+footPositioners[i].axisDifferences.x); //add distances
            }
        }

        return fDist;
    }
}
/*
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
*/
/*
if (Physics.Raycast(point.transform.position+ (transform.up * 100), -rb.transform.up, out hit, 2000, layerMask))
            {
                if (hit.transform.CompareTag("Ground"))
                {
                    float desiredHeight = Mathf.Min(animalHeight*.9f,animalHeight-(rb.velocity.magnitude/100));//strides get bigger at faster speeds so animate lower body too
                    desiredHeight = Mathf.Clamp(desiredHeight, animalHeight *.6f, animalHeight);
                
                    float upForce = 0;
                    upForce = Mathf.Abs(desiredHeight / Vector3.Distance(hit.point, point.transform.position))*2;
                    upForce = Mathf.Clamp(upForce, 0f,10f);//Stop adding too much force
                    
                    Vector3 dir = (hit.point + (-gravityDir * (desiredHeight)) - transform.position).normalized;
                    Vector3 locdir = rb.transform.InverseTransformDirection(dir);//Find velocity in relation to an object oriented to ground
                    dir.x = 0;
                    dir.z = 0;

                    rb.AddForceAtPosition(transform.up * (locdir.y * ((gravityStrength) * Time.deltaTime * 100)), point.transform.position,
                        ForceMode.Acceleration);

                }*/