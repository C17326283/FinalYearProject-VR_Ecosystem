using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AnimalGravity : MonoBehaviour
{
    public GameObject core;

    public float gravityStrength = 600; //todo check correct

    //public float upMultiplier = 600f;
    public Vector3 gravityDir;
    public AnimalBrain brain;
    public float animalHeight = 2;
    public float animalStartingHeight = 2;
    public float animalLength = 2;
    public Rigidbody rb;
    public GameObject headHeightPosObj;

    private int layerMask;
    public List<GameObject> forcePoints;
    public bool InitializeOnStart = false;

    public List<AnimalFeetPositioner> footPositioners;

    public float animalHCorrectorAmount = 0.8f;
    public Collider col;



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
        col = GetComponent<CapsuleCollider>();
        layerMask = 1 << 8; //bit shift to get mask for raycasting so only on environment and not other animals
        
        if (brain != null)
        {
            animalHeight = brain.animalHeight;
            animalStartingHeight = brain.animalHeight;//for reseting to if animal was swimming and normal height was changed
            animalLength = brain.animalLength;
        }

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
        var point = forcePoints[0];
        RaycastHit hit;
        //Only add if theres environment below
        if (Physics.Raycast(point.transform.position + (-gravityDir * 100), gravityDir, out hit, 1000, layerMask))
        {
                
            float furthestFootDist = GetFurthestFootDist();

            //get height based on magnitude or default height
            //float desiredHeight = Mathf.Min(animalHeight * .9f, animalHeight - (rb.velocity.magnitude / 30)); //strides get bigger at faster speeds so animate lower body too

            float clampedMag = Mathf.Clamp(rb.velocity.magnitude/2, 1, Mathf.Min(2,animalHeight* animalHCorrectorAmount));
            float desiredHeight = (animalHeight * animalHCorrectorAmount)-((furthestFootDist/clampedMag)/8); //height based on stride
            //               print("desiredHeight"+transform.name+desiredHeight);
            desiredHeight = Mathf.Clamp(desiredHeight, animalHeight * .5f, animalHeight *animalHCorrectorAmount);
            //desiredHeight = desiredHeight * (1 * brain.bounceMult);


            //get a distance away from target than can be used to reduce force// *8 to decrease range it affects
            var position = point.transform.position;
            float distForce = Vector3.Distance(hit.point + (transform.up * desiredHeight), position) * 5; //find dist between current and desired point

//                    print("distFroce"+distForce);

            float gravForce = Mathf.Min(gravityStrength * distForce, gravityStrength); //if close to point then add less force

            Vector3 dir = (hit.point + (-gravityDir * (desiredHeight)) - position).normalized;

            rb.AddForceAtPosition(dir * (gravForce * Time.deltaTime), position,
                ForceMode.Acceleration);

            CheckOnGround();
            
        }
    }

    public float GetFurthestFootDist()
    {
        //print(footPositioners);
        float fDist =0;

        for (int i = 0; i < 2; i++)
        {
            fDist = fDist+Mathf.Abs(footPositioners[i].axisDifferences.z+(footPositioners[i].axisDifferences.x/2)); //add distances

        }

        return fDist*brain.bounceMult;
    }

    public void CheckOnGround()
    {
        //If animal is swimming or dead then set feet to not stick to surface. check is on correct to avoid multiple accesses when not needed
        if (animalHeight < animalStartingHeight/10 && footPositioners[0].legDefaultStretching == false)
        {
            foreach (var footPositioner in footPositioners)
            {
                footPositioner.legDefaultStretching = true;
                col.enabled = false;
            }
        }
        else if(animalHeight > animalStartingHeight/10 && footPositioners[0].legDefaultStretching == true)
        {
            foreach (var footPositioner in footPositioners)
            {
                footPositioner.legDefaultStretching = false;
                col.enabled = true;
            }
        }
    }
}