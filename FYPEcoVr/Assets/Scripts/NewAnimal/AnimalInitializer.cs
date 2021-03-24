using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using Panda;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR.Interaction.Toolkit;

public class AnimalInitializer : MonoBehaviour
{
    [Header("Requires Input")]
    public AnimalProfile animalDNA;
    public TextAsset[] btTexts;
    
    [Header("Debug display")]
    public GameObject animalObj;
    public CapsuleCollider collider;
    public Rigidbody rb;

    public GameObject head;
    public GameObject spineCore;
    public List<GameObject> legs;
    public List<GameObject> feet;//for manageing all the leg objects
    public List<GameObject> otherLimbs;
    
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
    public SpineNew spineMain;
    public float damping = 15;
    
    public AnimalAudioManager audioManager;

    public bool initialiseOnStart = false;
    public AudioMixer audioMixer;

    public AnimalForce animalForce;

    // Start is called before the first frame update
    void Awake()
    {
        if(initialiseOnStart) 
            InitialiseAnimal();
    }

    public void InitialiseAnimal()
    {
        feet = new List<GameObject> ();//for setting up foot positioners
        legs = new List<GameObject> ();//for setting up foot positioners
        feetPositioners = new List<AnimalFeetPositioner> ();
        otherLimbs = new List<GameObject> ();
        core = GameObject.Find("Core");
        
        
        //make the animal object
        animalObj = Instantiate(animalDNA.model);
        animalObj.transform.parent = this.transform;
        animalObj.transform.position = this.transform.position;
        //animalObj.tag = animalDNA.name;
        
        
        //Todo find more efficient way
        SetMovementOrigin();
        brain = movementOriginObj.gameObject.AddComponent<AnimalBrain>();
        GetLimbs();
        SetSkeleton();
        SetCollider();
        
        //OffsetSkeleton();
        
        SetRb();
        SetAudio();
        SetAI();
        SetPositioners();
        //brain.animalHeight = animalHeight;
        //brain.animalLength = animalLength;
        SetSenses();
        SetLimbs();
        movementOriginObj.AddComponent<XRSimpleInteractable>();//for triggering stat display

        StartCoroutine(SetDisabler());
        //temp
    }
    
    public void SetMovementOrigin()
    {
        movementOriginObj = new GameObject(animalDNA.name);
        movementOriginObj.transform.position = this.transform.position;
        movementOriginObj.transform.parent = this.transform;
        movementOriginObj.transform.tag = "AnimalContainer";//So animal can be sensed by others
    }
    
    
    public void SetSkeleton()
    {
        //Add spine controller for main spine object
        spineMain = this.gameObject.AddComponent<SpineNew>();
        spineMain.start = spineCore;
        spineMain.InitializeSpine();
        spineMain.damping = damping;
        
        //parent the spines holder to the movementOrigin
        spineCore.transform.parent.parent = movementOriginObj.transform;//set the base spine container to child of move obj
        
        //The animalObj will only hold the mesh after reassigning bones so parent that to movement origin
        animalObj.transform.parent = movementOriginObj.transform;
        animalObj.transform.name = "MeshParent";
        
        GameObject headHolder = new GameObject("HeadHolder");
        headHolder.transform.position = head.transform.position;
        headHolder.transform.forward = movementOriginObj.transform.forward;
        head.transform.parent = headHolder.transform;
        head = headHolder;//Reassing to headholder so we can move the holder
        spineMain.MatchLimbToSpine(head.transform);

        
        foreach (var limb in otherLimbs)
        {
            SpineNew limbSp = this.gameObject.AddComponent<SpineNew>();
            limbSp.start = limb;
            limbSp.InitializeSpine();
            limbSp.damping = damping;

            spineMain.MatchLimbToSpine(limb.transform.parent,headHolder.transform);//use parent to get containter
        }

        foreach (var leg in legs)
        {
            spineMain.MatchLimbToSpine(leg.transform);
        }
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
            }
            else if(childBone.CompareTag("SpineCore"))
            {
                spineCore = childBone.gameObject;
            }
            else if(childBone.CompareTag("Leg"))
            {
                legs.Add(childBone.transform.gameObject);
            }
            else if(childBone.CompareTag("Foot"))
            {
                feet.Add(childBone.transform.gameObject);
            }
            else if(childBone.CompareTag("OtherLimb"))
            {
                otherLimbs.Add(childBone.transform.gameObject);
            }
        }
    }


    public void SetCollider()
    {
        //add components to animal and position it to center of mass
        collider = movementOriginObj.AddComponent<CapsuleCollider>();
        collider.direction = 2;//x is 0, y is 1, z is 2
        //collider.center = transform.InverseTransformPoint(transform.position);
        //Vector3 meshBounds = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size;
        //move the collider back to the body even though we need it attached to the head
        //todo tails to allow for this to correctly position
        /*
        Vector3 centerpos =
            (spineMain.spineContainers[spineMain.spineContainers.Count - 1].transform.position -
             spineMain.spineContainers[0].transform.position) / 2;
        
        collider.center = centerpos;
        */

        //edit the bounds to be smaller and reasign
        //Vector3 newMeshBounds = meshBounds / 2;
        //newMeshBounds.y = newMeshBounds.y / 3;//Half the height so it can have a body floating above ground and legs work liek springs
        //collider.size = newMeshBounds;

        float anH = Mathf.Abs(movementOriginObj.transform.InverseTransformPoint(head.transform.position).y - movementOriginObj.transform.InverseTransformPoint(feet[0].transform.position).y)*1.1f;
        float anW = Mathf.Abs(movementOriginObj.transform.InverseTransformPoint(legs[1].transform.position).x -movementOriginObj.transform.InverseTransformPoint(legs[0].transform.position).x)*1.1f;
        //float anL = (transform.InverseTransformPoint(head.transform.position).z - transform.InverseTransformPoint(spineMain.spineContainers[spineMain.spineContainers.Count - 1].transform.position).z)*4;
        float anL = Mathf.Abs(movementOriginObj.transform.InverseTransformPoint(head.transform.position).z - movementOriginObj.transform.InverseTransformPoint(spineMain.spineContainers[spineMain.spineContainers.Count - 1].transform.position).z)*1.5f;

        brain.animalHeight = anH;
        brain.animalLength = anL;
        
//        Debug.Log(movementOriginObj.transform.name+",anL:"+anL+",anW"+anW);
        collider.height = anL;//collider.height = newMeshBounds.z;
        collider.radius = anW;

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
        brain.animalBaseDNA = animalDNA;
        
        if (GetComponent<AnimalBehaviours>() == null) //incase i have it attached for testing
            behaviours = movementOriginObj.gameObject.AddComponent<AnimalBehaviours>(); //todo get a way to add this at runtime
        else
            behaviours = GetComponent<AnimalBehaviours>();
        behaviours.brain = brain;
        //Pool object are got from a find in the scene on awake
        behaviours.audioManager = audioManager;
        behaviours.headObject = head.transform;
        behaviours.rb = rb;
        HeadLook headTargeter = head.AddComponent<HeadLook>();
        headTargeter.behaviourTargeting = behaviours;

        if (GetComponent<BehaviourTree>() == null) //incase i have it attached for testing
            behaviourTreeManager = movementOriginObj.gameObject.AddComponent<BehaviourTree>(); //todo get a way to add this at runtime
        else
            behaviourTreeManager = GetComponent<BehaviourTree>();

        behaviourTreeManager.scripts = btTexts;
        behaviourTreeManager.tickOn = BehaviourTree.UpdateOrder.Update;
        //        behaviourTreeManager.Apply();//This is the thing to fix
        
        animalForce = movementOriginObj.AddComponent<AnimalForce>();
        animalForce.rb = rb;
        animalForce.brain = brain;
        behaviours.animalForce = animalForce;
    }

    

    void SetPositioners()
    {
        //animalHeight = spineCore.transform.position.y-feet[0].transform.position.y;
        //animalLength = spineCore.transform.position.z-feet[feet.Count-1].transform.position.z;
        
        groundOrienter = movementOriginObj.AddComponent<AnimalGroundVelocityOrienter>();
        groundOrienter.brain = brain;
        groundOrienter.Initialize();

        animalGravity = movementOriginObj.AddComponent<AnimalGravity>();
        animalGravity.brain = brain;
        //animalGravity.animalHeight = animalHeight;//this would be the height of the animal
        //animalGravity.animalLength = animalLength;
        animalGravity.headHeightPosObj = spineCore;//todo change head height var name
        animalGravity.Initialize();
    }


    void SetUpFootPositioner(Transform foot)//todo connect to bone not base obj
    {
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
        footScript.brain = brain;
        footScript.rb = rb;
        footScript.audioManager = audioManager;
        
        //Make the foot positioner stuff for inverse kinematics
        FastIKFabric ikScript = foot.gameObject.AddComponent<FastIKFabric>();
        ikScript.Target = footScript.footIKTargetObj.transform;
        ikScript.Init();//Changed it to called init instead of it happening
                        //on awake to prevent it creating target again

        GameObject ikPole = new GameObject("ikPole_"+foot.name);
        ikPole.transform.parent = footPositioner.transform;
        if (feetPositioners.Count > 2) //If not the front 2 legs
        {
            ikPole.transform.position = footPositioner.transform.position+(footPositioner.transform.forward * 10)+(footPositioner.transform.up *
                (brain.animalHeight/3));
        }
        else
        {
            ikPole.transform.position = footPositioner.transform.position+(-footPositioner.transform.forward * 10)+(footPositioner.transform.up *  (brain.animalHeight/2));
            
            //Set gravity foot positioners so can have foot dist based height
            animalGravity.footPositioners.Add(footScript);
        }
        ikScript.Pole = ikPole.transform;

        //Match the legs so not walking with both feet off the ground
        if (feetPositioners.Count % 2 == 0) //Then an even leg number, we can assume it will get the front 2 legs first, this leg is added before function runs
        {
            footScript.otherFootRaycastPositioner = feetPositioners[feetPositioners.Count - 2];
            feetPositioners[feetPositioners.Count - 2].otherFootRaycastPositioner = footScript;
            //footScript.hasOffset = true;
            
        }
    }
    
    void SetLimbs()
    {
        foreach (var foot in feet)
        {
            //Needs to have found a head first to be successfull
            SetUpFootPositioner(foot.transform);
        }
        //This needs to be done after setup because the feet are used to get the height
        foreach (var footPositioner in feetPositioners)
        {
            footPositioner.brain = brain;
            footPositioner.rb = rb;
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
        int checks = 0;
        while (checks < 30)
        {
            yield return new WaitForSeconds(1);//Wait until player spawns

            if (GameObject.FindWithTag("Player") != null)
            {
                AnimalDistanceDisabler distDisabler = transform.parent.gameObject.AddComponent<AnimalDistanceDisabler>();
                distDisabler.player = GameObject.FindWithTag("Player").transform;
                distDisabler.animal = movementOriginObj.transform;
                distDisabler.animalHolder = transform.gameObject;
                distDisabler.bt = behaviourTreeManager;
                distDisabler.enabled = true;
                brain.distDisabler = distDisabler;//for turning off on death
                checks = 100;//exit
                transform.gameObject.SetActive(false);//disable to begin with for loading
            }
        }
    }

    public void SetAudio()
    {
        audioManager = movementOriginObj.AddComponent<AnimalAudioManager>();
        audioManager.footstep = animalDNA.footstep;
        audioManager.attack = animalDNA.attack;
        audioManager.ambient = animalDNA.ambient;
        
        audioManager.Initialize();
    }
}
