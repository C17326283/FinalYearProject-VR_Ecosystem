using System.Collections;
using System.Collections.Generic;
using DitzelGames.FastIK;
using UnityEngine;

public class InitializeAnimal : MonoBehaviour
{
    public AnimalProfile animalData;
    [HideInInspector]
    public GameObject animalObj;
    [HideInInspector]
    public NewTaskManager taskManager;
    [HideInInspector]
    public BoxCollider collider;
    [HideInInspector]
    public BodyRaycastPositioner bodyPositioner;
    [HideInInspector]
    public Rigidbody rb;

    public GameObject head;
    public List<GameObject> feet;//for manageing all the leg objects
    public List<FootRaycastPositioner> feetPositioners;//for linking legs together

    public GameObject core;

    public GameObject movementOrigin;

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
        
        //set the stats to max
        animalData.health = animalData.maxHealth;
        animalData.hunger = animalData.maxStat;
        animalData.thirst = animalData.maxStat;
        
        //make the animal object
        animalObj = Instantiate(animalData.model);
        animalObj.transform.parent = this.transform;
        animalObj.transform.position = this.transform.position;
        animalObj.tag = animalData.Tag;
        
        feet = new List<GameObject> ();
        feetPositioners = new List<FootRaycastPositioner> ();
        
        //make array of all child objects and check bones for correct ones
        Transform[] allChildObjects = GetComponentsInChildren<Transform>();
        foreach (Transform childBone in allChildObjects)
        {
//            Debug.Log("child "+childBone.name+childBone.tag);
            if(childBone.CompareTag("Head"))//find the head and end of the legs of the animals
            {
                head = childBone.gameObject;
                
                //Done here because the head is needed to make spine and position legs
                SpineNew SpineScript = animalObj.AddComponent<SpineNew>();
                SpineScript.head = head;
                SpineScript.InitializeSpine();
                movementOrigin = head.transform.parent.gameObject;//set in spinescript to control head so need this to have the rigidbody to allow spine animation;
            }
            else if(childBone.CompareTag("Leg"))
            {
//                print(childBone.transform.name);
//                print(feet);
                feet.Add(childBone.transform.gameObject);
            }
        }

        foreach (var foot in feet)
        {
            //Needs to have found a head first to be successfull
            SetUpFootPositioner(foot.transform);
        }

        //add components to animal and position it to center of mass
        collider = movementOrigin.AddComponent<BoxCollider>();
        Vector3 meshBounds = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size;

        //move the collider back to the body even though we need it attached to the head
        collider.center = (animalObj.transform.position -movementOrigin.transform.position)+Vector3.up;//a bit higher so legs can have sprign without collider hitting ground
        //edit the bounds to be smaller and reasign
        Vector3 newMeshBounds = meshBounds / 2;
        
        newMeshBounds.y = newMeshBounds.y / 3;//Half the height so it can have a body floating above ground and legs work liek springs
        
        collider.size = newMeshBounds;
        
        /*
        bodyPositioner = animalObj.AddComponent<BodyRaycastPositioner>();
        if (core != null)
            bodyPositioner.core = core;
            
        //Make the positioning objects
        bodyPositioner.positionerMeasuringObj = new GameObject("front positioner");
        bodyPositioner.positionerMeasuringObj.transform.parent = animalObj.transform;
        bodyPositioner.positionerMeasuringObj.transform.position = animalObj.transform.position + transform.forward;
        bodyPositioner.backRotFixingObj = new GameObject("back positioner");
        bodyPositioner.backRotFixingObj.transform.parent = animalObj.transform;
        bodyPositioner.backRotFixingObj.transform.position = animalObj.transform.position - transform.forward;
        */
        rb = movementOrigin.AddComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.mass = 10;
        rb.drag = 1;
        rb.angularDrag = 1;


        
        AnBoidMove movementScript = movementOrigin.AddComponent<AnBoidMove>();
        movementScript.core = core;
        movementScript.animalHeight = head.transform.position.y-feet[0].transform.position.y;
        movementScript.desiredHeight =movementScript.animalHeight;
        
        /*
        BoidTaskManager taskManager = movementOrigin.AddComponent<BoidTaskManager>();
        taskManager.animalProfile = animalData;
        taskManager.boidMovementScript = movementScript;
        taskManager.Initialize();
        */


        /*
        taskManager = animalObj.AddComponent<NewTaskManager>();
        taskManager.animalProfile = animalData;
        taskManager.CreateObjs();
        */

    }

    // Update is called once per frame
    void Update()
    {
        animalData.hunger = animalData.hunger - animalData.hungerDecrement * Time.deltaTime;// div 100 to keep it within normal numbers for testing
        animalData.thirst = animalData.thirst - animalData.thirstDecrement * Time.deltaTime;
        animalData.reproductiveUrge = animalData.reproductiveUrge + animalData.reproductiveIncrement * Time.deltaTime;
        animalData.age = animalData.age + animalData.ageIncrement * Time.deltaTime;
       
    }

    public void Die()
    {
        Debug.Log("Died");
        Destroy(gameObject);//destroy after 20secs
        Destroy(taskManager.wanderPosObj);//destroy after 20secs
    }

    public void Born()
    {
        //Need a better mating system with father and mother and get a base stat based on the 2 of them
          animalData.health = 100;
          animalData.hunger = 100;
          animalData.thirst = 100;
          animalData.reproductiveUrge = 0;
          animalData.age = 0;
          MutateStats();
          taskManager.GetNewTask();
    }

    public void MutateStats()
    {
        float change = animalData.maxMutatePercent / 100;//get float as percent
        float percentDif = 0;

        percentDif = animalData.healthStarveDecrement * change;
        animalData.healthStarveDecrement = Mathf.Clamp(Random.Range(animalData.healthStarveDecrement-percentDif, animalData.healthStarveDecrement+percentDif), 0, 30);
        percentDif = animalData.hungerDecrement * change;
        animalData.hungerDecrement = Mathf.Clamp(Random.Range(animalData.hungerDecrement-percentDif, animalData.hungerDecrement+percentDif), 0, 30);
        percentDif = animalData.thirstDecrement * change;
        animalData.thirstDecrement = Mathf.Clamp(Random.Range(animalData.thirstDecrement-percentDif, animalData.thirstDecrement+percentDif), 0, 30);
        percentDif = animalData.reproductiveIncrement * change;
        animalData.reproductiveIncrement = Mathf.Clamp(Random.Range(animalData.reproductiveIncrement-percentDif, animalData.reproductiveIncrement+percentDif), 0, 30);
        percentDif = animalData.memoryLossRate * change;
        animalData.memoryLossRate = Mathf.Clamp(Random.Range(animalData.memoryLossRate-percentDif, animalData.memoryLossRate+percentDif), 0, 100000);
        percentDif = animalData.sensoryRange * change;
        animalData.sensoryRange = Mathf.Clamp(Random.Range(animalData.sensoryRange-percentDif, animalData.sensoryRange+percentDif), 0, 100);
        percentDif = animalData.healthStarveDecrement * change;
        animalData.healthStarveDecrement = Mathf.Clamp(Random.Range(animalData.maxHealth-percentDif, animalData.maxHealth+percentDif), 0, 30);
        percentDif = animalData.moveSpeed * change;
        animalData.moveSpeed = Mathf.Clamp(Random.Range(animalData.moveSpeed-percentDif, animalData.moveSpeed+percentDif), 0, 30000);
        percentDif = animalData.rotSpeed * change;
        animalData.rotSpeed = Mathf.Clamp(Random.Range(animalData.rotSpeed-percentDif, animalData.rotSpeed+percentDif), 0, 1000000);
        percentDif =animalData.wanderRadius * change;
        animalData.wanderRadius = Mathf.Clamp(Random.Range(animalData.wanderRadius-percentDif, animalData.wanderRadius+percentDif), 0, 30000);
        percentDif = animalData.forwardWanderBias * change;
        animalData.forwardWanderBias = Mathf.Clamp(Random.Range(animalData.forwardWanderBias-percentDif, animalData.forwardWanderBias+percentDif), 0, 30);
    }

    void SetUpFootPositioner(Transform foot)//todo connect to bone not base obj
    {
        //Make the foot positioner stuff for inverse kinematics
        FastIKFabric ikScript = foot.gameObject.AddComponent<FastIKFabric>();
        
        GameObject footPositioner = new GameObject("FootPositioner_"+foot.name);
        footPositioner.transform.parent = movementOrigin.transform.GetChild(0);//set them to the child so the container isnt stretched
        footPositioner.transform.position = foot.transform.position+(footPositioner.transform.forward);//+(-animalObj.transform.forward*0.5f)
        FootRaycastPositioner footScript = footPositioner.AddComponent<FootRaycastPositioner>();
        feetPositioners.Add(footScript);

        footScript.endBoneObj = foot.gameObject;
        footScript.forwardFacingObj = movementOrigin.gameObject;
        
        GameObject ikPole = new GameObject("ikPole_"+foot.name);
        ikPole.transform.parent = movementOrigin.transform.GetChild(0);
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
