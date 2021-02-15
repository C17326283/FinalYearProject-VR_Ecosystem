using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;

public class AnimalInitializer : MonoBehaviour
{
    public AnimalProfile animalData;
    [HideInInspector]
    public GameObject animalObj;
    [HideInInspector]
    public BoxCollider collider;
    [HideInInspector]
    public BodyRaycastPositioner bodyPositioner;
    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public GameObject head;
    [HideInInspector]
    public List<GameObject> feet;//for manageing all the leg objects
    [HideInInspector]
    public List<FootRaycastPositioner> feetPositioners;//for linking legs together
    [HideInInspector]
    public GameObject core;
    [HideInInspector]
    public GameObject movementOriginObj;
    
    [HideInInspector]
    public AnimalBrain brain;//manages memory and senses
    [HideInInspector]
    public AnimalBehaviours behaviours;//The behaviours uses by behaviour tree
    
    
    
    

    public bool initialiseOnStart = false;
    // Start is called before the first frame update
    void Start()
    {
        if(initialiseOnStart) 
            InitialiseAnimal();

    }

    public void InitialiseAnimal()
    {
        core = GameObject.Find("Core");
        
        //make the animal object
        animalObj = Instantiate(animalData.model);
        animalObj.transform.parent = this.transform;
        animalObj.transform.position = this.transform.position;
        animalObj.tag = animalData.Tag;
        
        feet = new List<GameObject> ();//for setting up foot positioners
        feetPositioners = new List<FootRaycastPositioner> ();
        
        //make array of all child objects and check bones for correct ones
        Transform[] allChildObjects = GetComponentsInChildren<Transform>();
        foreach (Transform childBone in allChildObjects)
        {
            if(childBone.CompareTag("Head"))//find the head and end of the legs of the animals
            {
                head = childBone.gameObject;
                
                //Done here because the head is needed to make spine and position legs
                SpineNew SpineScript = animalObj.AddComponent<SpineNew>();
                SpineScript.head = head;
                SpineScript.InitializeSpine();
                movementOriginObj = head.transform.parent.gameObject;//set in spinescript to control head so need this to have the rigidbody to allow spine animation;
            }
            else if(childBone.CompareTag("Leg"))
            {
                feet.Add(childBone.transform.gameObject);
            }
        }

        foreach (var foot in feet)
        {
            //Needs to have found a head first to be successfull
            SetUpFootPositioner(foot.transform);
        }

        //add components to animal and position it to center of mass
        collider = movementOriginObj.AddComponent<BoxCollider>();
        Vector3 meshBounds = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size;
        //move the collider back to the body even though we need it attached to the head
        collider.center = (animalObj.transform.position -movementOriginObj.transform.position)+Vector3.up;//a bit higher so legs can have sprign without collider hitting ground
        //edit the bounds to be smaller and reasign
        Vector3 newMeshBounds = meshBounds / 2;
        newMeshBounds.y = newMeshBounds.y / 3;//Half the height so it can have a body floating above ground and legs work liek springs
        collider.size = newMeshBounds;
        
        rb = movementOriginObj.AddComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.mass = 10;
        rb.drag = 1;
        rb.angularDrag = 1;
        
        AnimalBrain brain = this.gameObject.AddComponent<AnimalBrain>();
        AnimalBehaviours behaviours = this.gameObject.AddComponent<AnimalBehaviours>();
        



    }


    void SetUpFootPositioner(Transform foot)//todo connect to bone not base obj
    {
        //Make the foot positioner stuff for inverse kinematics
        FastIKFabric ikScript = foot.gameObject.AddComponent<FastIKFabric>();
        
        GameObject footPositioner = new GameObject("FootPositioner_"+foot.name);
        footPositioner.transform.parent = movementOriginObj.transform.GetChild(0);//set them to the child so the container isnt stretched
        footPositioner.transform.position = foot.transform.position+(footPositioner.transform.forward);//+(-animalObj.transform.forward*0.5f)
        FootRaycastPositioner footScript = footPositioner.AddComponent<FootRaycastPositioner>();
        feetPositioners.Add(footScript);

        footScript.endBoneObj = foot.gameObject;
        footScript.forwardFacingObj = movementOriginObj.gameObject;
        
        GameObject ikPole = new GameObject("ikPole_"+foot.name);
        ikPole.transform.parent = movementOriginObj.transform.GetChild(0);
        ikPole.transform.position = footPositioner.transform.position+(-footPositioner.transform.forward * 10)+(footPositioner.transform.up * 2);
        ikScript.Pole = ikPole.transform;
        //footScript.footIKPositionObj = childBone.gameObject;
        
        //Match the legs so not walking with both feet off the ground
        if (feetPositioners.Count % 2 == 0) //Then an even leg number, we can assume it will get the front 2 legs first, this leg is added before function runs
        {
            footScript.otherFootRaycastPositioner = feetPositioners[feetPositioners.Count - 2];
            feetPositioners[feetPositioners.Count - 2].otherFootRaycastPositioner = footScript;
            footScript.hasOffset = true;
        }
    }
}
