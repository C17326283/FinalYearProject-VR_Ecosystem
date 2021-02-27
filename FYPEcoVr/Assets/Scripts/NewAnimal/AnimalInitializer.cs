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
    public AnimalGravity animalGravity;
    public AnimalGroundVelocityOrienter groundOrienter;
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
        otherLimbs = new List<GameObject> ();
        core = GameObject.Find("Core");
        
        //make the animal object
        animalObj = Instantiate(animalDNA.model);
        animalObj.transform.parent = this.transform;
        animalObj.transform.position = this.transform.position;
        //animalObj.tag = animalDNA.name;
        
        //Todo find more efficient way
        GetLimbs();
        SetCollider();
        SetRb();
        SetAI();
        SetPositioners();
        brain.animalHeight = animalHeight;
        SetSenses();
        SetLimbs();

        StartCoroutine(SetDisabler());
        //temp
    }

    public void GetLimbs()
    {
        //make array of all child objects and check bones for correct ones
        Transform[] allChildObjects = GetComponentsInChildren<Transform>();
        foreach (Transform childBone in allChildObjects)
        {
            if(childBone.CompareTag("Head"))//find the head and end of the legs of the animals
            {
                head = childBone.gameObject;
                
                //Done here because the head is needed to make spine and position legs
                SetUpSpine();
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
    }

    public void SetUpSpine()
    {
        SpineScript = this.gameObject.AddComponent<SpineNew>();
        SpineScript.head = head;
        SpineScript.InitializeSpine();//initiallise once all the feet have been addedf
        SpineScript.MatchLimbsToSpine();
        movementOriginObj = head.transform.parent.gameObject;//set in spinescript to control head so need this to have the rigidbody to allow spine animation;
        movementOriginObj.transform.name = animalDNA.name;
        animalObj.transform.parent = movementOriginObj.transform;//Parent mesh to movement to hoepfully fix that stretched mesh
        animalObj.transform.name = "MeshParent";
    }

    public void SetCollider()
    {
        //add components to animal and position it to center of mass
        collider = movementOriginObj.AddComponent<BoxCollider>();
        Vector3 meshBounds = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size;
        //move the collider back to the body even though we need it attached to the head
        //todo tails to allow for this to correctly position
        Vector3 centerpos =
            (SpineScript.spineContainers[SpineScript.spineContainers.Count - 1].transform.position -
             SpineScript.spineContainers[0].transform.position) / 2;
        
        collider.center = centerpos;

        //edit the bounds to be smaller and reasign
        Vector3 newMeshBounds = meshBounds / 2;
        newMeshBounds.y = newMeshBounds.y / 3;//Half the height so it can have a body floating above ground and legs work liek springs
        collider.size = newMeshBounds;
    }

    void SetRb()
    {
        rb = movementOriginObj.AddComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.mass = 10;
        rb.drag = 1;
        rb.angularDrag = 1;
    }

    void SetAI()
    {
        brain = movementOriginObj.gameObject.AddComponent<AnimalBrain>();
        brain.animalBaseDNA = animalDNA;
        
        if (GetComponent<AnimalBehaviours>() == null) //incase i have it attached for testing
            behaviours = movementOriginObj.gameObject.AddComponent<AnimalBehaviours>(); //todo get a way to add this at runtime
        else
            behaviours = GetComponent<AnimalBehaviours>();
        behaviours.brain = brain;
        behaviours.rb = rb;

        if (GetComponent<BehaviourTree>() == null) //incase i have it attached for testing
            behaviourTreeManager = movementOriginObj.gameObject.AddComponent<BehaviourTree>(); //todo get a way to add this at runtime
        else
            behaviourTreeManager = GetComponent<BehaviourTree>();
        
        print("btTexts"+btTexts);
        


        behaviourTreeManager.scripts = btTexts;
        //behaviourTreeManager.Compile();
        behaviourTreeManager.tickOn = BehaviourTree.UpdateOrder.FixedUpdate;
        //behaviourTreeManager.autoReset = true;
        
        //        behaviourTreeManager.Apply();//This is the thing to fix it not starting untill the objetc was clicked


        //behaviourTreeManager.Compile();


    }

    void SetLimbs()
    {
        foreach (var foot in feet)
        {
            //Needs to have found a head first to be successfull
            SetUpFootPositioner(foot.transform);
        }
        foreach (var limb in otherLimbs)
        {
            SpineNew LimbScript = gameObject.AddComponent<SpineNew>();//Needs to be in global position no childed locally
            LimbScript.isLimbSetup = true;
            LimbScript.head = limb;
            LimbScript.InitializeSpine();//initiallise once all the feet have been added
            LimbScript.damping = SpineScript.damping / 3;
        }
        
        //This needs to be done after setup because the feet are used to get the height
        foreach (var footPositioner in feetPositioners)
        {
            footPositioner.animalHeight = animalHeight;
            footPositioner.animalLength = animalLength;
            footPositioner.rb = rb;
        }
    }

    void SetPositioners()
    {
        animalHeight = head.transform.position.y-feet[0].transform.position.y;
        animalLength = head.transform.position.z-feet[feet.Count-1].transform.position.z;
        
        groundOrienter = movementOriginObj.AddComponent<AnimalGroundVelocityOrienter>();
        groundOrienter.brain = brain;
        groundOrienter.Initialize();

        animalGravity = movementOriginObj.AddComponent<AnimalGravity>();
        animalGravity.animalHeight = animalHeight;//this would be the height of the animal
        animalGravity.animalLength = animalLength;
        animalGravity.headHeightPosObj = head;
        animalGravity.Initialize();
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
        footScript.footIKTargetObj.transform.position = foot.transform.position;
        footScript.endBoneObj = foot.gameObject;
        footScript.forwardFacingObj = movementOriginObj.gameObject;
        footScript.animalHeight = animalHeight;
        footScript.rb = rb;

        GameObject ikPole = new GameObject("ikPole_"+foot.name);
        ikPole.transform.parent = footPositioner.transform;
        if (feetPositioners.Count > 2) //If not the front 2 legs
        {
            ikPole.transform.position = footPositioner.transform.position+(footPositioner.transform.forward * 10)+(footPositioner.transform.up *
                (animalHeight/2));
        }
        else
        {
            ikPole.transform.position = footPositioner.transform.position+(-footPositioner.transform.forward * 10)+(footPositioner.transform.up *  (animalHeight/2));
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
    
    public void SetSenses()
    {
        sensorySphere = new GameObject("SenseSphere");
        AnimalSenses senses = sensorySphere.AddComponent<AnimalSenses>();
        senses.brain = brain;
        SphereCollider col = sensorySphere.AddComponent<SphereCollider>();
        (col as SphereCollider).radius  = 10 * 2;
        sensorySphere.GetComponent<Collider>().isTrigger = true;
        sensorySphere.transform.parent = movementOriginObj.gameObject.transform;
        sensorySphere.transform.position = movementOriginObj.transform.position;
        Rigidbody SenseRb = sensorySphere.AddComponent<Rigidbody>();//Needs kinematic to register collisions
        SenseRb.useGravity = false;
        SenseRb.isKinematic = true;
    }
    

    IEnumerator SetDisabler()
    {
        bool found = false;
        while (found == false)
        {
            yield return new WaitForSeconds(1);

            if (GameObject.Find("Player") != null)
            {
                AnimalDistanceDisabler distDisabler = transform.parent.gameObject.AddComponent<AnimalDistanceDisabler>();
                distDisabler.player = GameObject.Find("Player").transform;
                distDisabler.enabled = false;
                distDisabler.animal = movementOriginObj.transform;
                distDisabler.animalHolder = transform.gameObject;
                distDisabler.bt = behaviourTreeManager;
                distDisabler.enabled = true;
                found = true;
                transform.gameObject.SetActive(false);//disable to begin with for loading
            }
        }
        
    }
    
    //IEnumerator testCustTick
    
}
