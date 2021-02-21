using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using Panda;
using UnityEngine;

public class AnimalInitializer : MonoBehaviour
{
    public AnimalProfile animalDNA;
    public TextAsset[] btTexts;
    
    [HideInInspector]
    public GameObject animalObj;
    [HideInInspector]
    public BoxCollider collider;
    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public GameObject head;
    [HideInInspector]
    public List<GameObject> feet;//for manageing all the leg objects
    [HideInInspector]
    public List<GameObject> otherLimbs;
    [HideInInspector]
    public List<AnimalFeetPositioner> feetPositioners;//for linking legs together
    [HideInInspector]
    public GameObject core;
    [HideInInspector]
    public GameObject movementOriginObj;
    
    [HideInInspector]
    public AnimalBrain brain;//manages memory and senses
    
    public AnimalBehaviours behaviours;//The behaviours uses by behaviour tree
    
    [HideInInspector]
    public BehaviourTree behaviourTreeManager;
    
    public AnimalBodyPositioner bodyPositioner;

    public GameObject sensorySphere;

    public SpineNew SpineScript;

    public float animalHeight=2;
    public float animalLength=2;
    
    
    

    public bool initialiseOnStart = false;
    // Start is called before the first frame update
    void Awake()
    {
        if(initialiseOnStart) 
            InitialiseAnimal();

    }

    public void InitialiseAnimal()
    {
        feet = new List<GameObject> ();//for setting up foot positioners
        feetPositioners = new List<AnimalFeetPositioner> ();
        core = GameObject.Find("Core");
        
        //make the animal object
        animalObj = Instantiate(animalDNA.model);
        animalObj.transform.parent = this.transform;
        animalObj.transform.position = this.transform.position;
        animalObj.tag = animalDNA.Tag;
        
        
        
        //make array of all child objects and check bones for correct ones
        Transform[] allChildObjects = GetComponentsInChildren<Transform>();
        foreach (Transform childBone in allChildObjects)
        {
            if(childBone.CompareTag("Head"))//find the head and end of the legs of the animals
            {
                head = childBone.gameObject;
                
                //Done here because the head is needed to make spine and position legs
                SpineScript = animalObj.AddComponent<SpineNew>();
                SpineScript.head = head;
                SpineScript.InitializeSpine();//initiallise once all the feet have been added
                SpineScript.MatchLimbsToSpine();
                movementOriginObj = head.transform.parent.gameObject;//set in spinescript to control head so need this to have the rigidbody to allow spine animation;
            }
            else if(childBone.CompareTag("Leg"))
            {
                feet.Add(childBone.transform.gameObject);
            }
            else if(childBone.CompareTag("OtherLimb"))
            {
                otherLimbs.Add(childBone.transform.gameObject);
            }
        }
        
        
        

        foreach (var foot in feet)
        {
            //Needs to have found a head first to be successfull
            SetUpFootPositioner(foot.transform);
        }
        foreach (var limb in otherLimbs)
        {
            SpineNew LimbScript = animalObj.AddComponent<SpineNew>();
            LimbScript.isLimbSetup = true;
            LimbScript.head = limb;
            LimbScript.InitializeSpine();//initiallise once all the feet have been added
            LimbScript.damping = SpineScript.damping / 2;
        }
        
        //add components to animal and position it to center of mass
        collider = movementOriginObj.AddComponent<BoxCollider>();
        Vector3 meshBounds = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size;
        //move the collider back to the body even though we need it attached to the head
        //todo tails to allow for this to correctly position
        //collider.center = (animalObj.transform.position -movementOriginObj.transform.position)+Vector3.up;//a bit higher so legs can have sprign without collider hitting ground
        //collider.center = (SpineScript.spineContainers[0].transform.position+SpineScript.spineContainers[SpineScript.spineContainers.Count-1].transform.position)/2;//a bit higher so legs can have sprign without collider hitting ground
        collider.center = (SpineScript.spineContainers[SpineScript.spineContainers.Count-1].transform.localPosition-SpineScript.spineContainers[0].transform.localPosition)/2;

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

        
        
        brain = this.gameObject.AddComponent<AnimalBrain>();
        brain.animalBaseDNA = animalDNA;
        if (GetComponent<AnimalBehaviours>() == null) //incase i have it attached for testing
            behaviours =
                this.gameObject.AddComponent<AnimalBehaviours>(); //todo get a way to add this at runtime
        else
            behaviours = GetComponent<AnimalBehaviours>();
        behaviours.brain = brain;
        behaviours.rb = rb;

        if (GetComponent<BehaviourTree>() == null) //incase i have it attached for testing
            behaviourTreeManager =
                this.gameObject.AddComponent<BehaviourTree>(); //todo get a way to add this at runtime
        else
            behaviourTreeManager = GetComponent<BehaviourTree>();
        behaviourTreeManager.scripts = btTexts;
        behaviourTreeManager.Compile();
        
        bodyPositioner = movementOriginObj.AddComponent<AnimalBodyPositioner>();
        bodyPositioner.brain = brain;
        animalHeight = head.transform.position.y-feet[0].transform.position.y;
        bodyPositioner.animalHeight = animalHeight;//this would be the height of the animal
        animalLength = head.transform.position.z-feet[feet.Count-1].transform.position.z;
        bodyPositioner.animalLength = animalLength;
        bodyPositioner.headHeightPosObj = head;

        addSenses();
        
        //This needs to be done after setup because the feet are used to get the height
        foreach (var footPositioner in feetPositioners)
        {
            footPositioner.animalHeight = animalHeight;
            footPositioner.animalLength = animalLength;
            //footPositioner.lerpSpeed = Mathf.Max(10,brain.moveSpeed/10);
            footPositioner.rb = rb;
//            print(footPositioner.animalHeight);
        }

    }


    void SetUpFootPositioner(Transform foot)//todo connect to bone not base obj
    {
        //Make the foot positioner stuff for inverse kinematics
        FastIKFabric ikScript = foot.gameObject.AddComponent<FastIKFabric>();
        
        GameObject footPositioner = new GameObject("FootPositioner_"+foot.name);
        footPositioner.transform.parent = GetRecursiveParentTag(foot);
        footPositioner.transform.position = foot.transform.position;//-(footPositioner.transform.forward)
        AnimalFeetPositioner footScript = footPositioner.AddComponent<AnimalFeetPositioner>();
        feetPositioners.Add(footScript);
        footScript.footIKTargetObj = new GameObject("FootTargetObj");
        footScript.footIKTargetObj.transform.parent = this.transform; //Set it to highest level parent as they need to move independently
        footScript.endBoneObj = foot.gameObject;
        footScript.forwardFacingObj = movementOriginObj.gameObject;
        footScript.animalHeight = animalHeight;
        footScript.rb = rb;


        GameObject ikPole = new GameObject("ikPole_"+foot.name);
        ikPole.transform.parent = footPositioner.transform;
        if (feetPositioners.Count > 2) //If not the front 2 legs
        {
            ikPole.transform.position = footPositioner.transform.position+(footPositioner.transform.forward * 10)+(footPositioner.transform.up * 2);
        }
        else
        {
            ikPole.transform.position = footPositioner.transform.position+(-footPositioner.transform.forward * 10)+(footPositioner.transform.up * 2);
        }
        ikScript.Pole = ikPole.transform;
        
        
        
        //Match the legs so not walking with both feet off the ground
        if (feetPositioners.Count % 2 == 0) //Then an even leg number, we can assume it will get the front 2 legs first, this leg is added before function runs
        {
            footScript.otherFootRaycastPositioner = feetPositioners[feetPositioners.Count - 2];
            feetPositioners[feetPositioners.Count - 2].otherFootRaycastPositioner = footScript;
            footScript.hasOffset = true;
        }
    }
    

    //Checks if parent has tag to allow proper parenting of spines
    public Transform GetRecursiveParentTag(Transform foot)
    {
        if (foot.CompareTag("SpineContainer"))
        {
            return foot;
        }
        else
        {
            return GetRecursiveParentTag(foot.parent);
        }
        
    }
    

    public void addSenses()
    {
        if (sensorySphere == null)
        {
            sensorySphere = new GameObject("SenseSphere");
            AnimalSenses senses = sensorySphere.AddComponent<AnimalSenses>();
            senses.brain = brain;
            print(senses.brain);
            SphereCollider col = sensorySphere.AddComponent<SphereCollider>();
            (col as SphereCollider).radius  = 10 * 2;
            sensorySphere.GetComponent<Collider>().isTrigger = true;
            sensorySphere.transform.parent = movementOriginObj.gameObject.transform;
            sensorySphere.transform.position = movementOriginObj.transform.position;
            Rigidbody rb = sensorySphere.AddComponent<Rigidbody>();//Needs kinematic to register collisions
            rb.useGravity = false;
            rb.isKinematic = true;
            
        }
    }
}
