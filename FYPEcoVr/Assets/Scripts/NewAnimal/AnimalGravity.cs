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
    public bool InitializeOnStart = false;

    public List<AnimalFeetPositioner> footPositioners;

    public float animalHCorrectorAmount = 0.88f;
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
    }

    public void AddGravity() //todo fix this mess
    {
        RaycastHit hit;
        //Only add if theres environment below
        if (Physics.Raycast(headHeightPosObj.transform.position + (-gravityDir * 100), gravityDir, out hit, 1000, layerMask))
        {
            float furthestFootDist = GetFurthestFootDist();
            float clampedMag = Mathf.Clamp(rb.velocity.magnitude, 1, Mathf.Min(2,animalHeight* animalHCorrectorAmount));
            float desiredHeight = (animalHeight * animalHCorrectorAmount)-((furthestFootDist/clampedMag)/6); //height based on stride
            desiredHeight = Mathf.Clamp(desiredHeight, animalHeight * .5f, animalHeight *animalHCorrectorAmount);
            
            //get a distance away from target than can be used to reduce force// *8 to decrease range it affects
            var position = headHeightPosObj.transform.position;
            float distForce = Vector3.Distance(hit.point + (transform.up * desiredHeight), position) * 5;
            //find dist between current and desired point
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

        for (int i = 0; i < footPositioners.Count; i++)
        {
            fDist = fDist+Mathf.Abs(footPositioners[i].axisDifferences.z+(footPositioners[i].axisDifferences.x/2)); //add distances

        }

        return fDist/(footPositioners.Count/2);
    }

    public void CheckOnGround()
    {
        //If animal is swimming or dead then set feet to not stick to surface. check is on correct to avoid multiple accesses when not needed
        if (animalHeight < animalStartingHeight/4 && footPositioners[0].legDefaultStretching == false)
        {
            foreach (var footPositioner in footPositioners)
            {
                footPositioner.legDefaultStretching = true;
                footPositioner.needToMove = true;
                col.enabled = false;
            }
        }
        else if(animalHeight > animalStartingHeight/4 && footPositioners[0].legDefaultStretching == true)
        {
            foreach (var footPositioner in footPositioners)
            {
                footPositioner.legDefaultStretching = false;
                col.enabled = true;
            }
        }
    }
}