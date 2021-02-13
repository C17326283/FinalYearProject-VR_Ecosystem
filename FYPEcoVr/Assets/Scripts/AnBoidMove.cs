using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnBoidMove : MonoBehaviour
{
    
    public Vector3 velocity;
    public float speed;
    public Vector3 acceleration;
    public Vector3 force;
    public float maxSpeed = 5;
    public float maxForce = 10;

    public float mass = 1;

    public bool seekEnabled = true;
    public Transform targetTransform;
    public Vector3 movePosTransform;//for keeping inlien to terrain

    public bool arriveEnabled = true;
    public float slowingDistance = 10;
    
    public GameObject core;
    public float gravityStrength = 100;
    public bool hasPlanetGravity = true;
    public bool hasUpForce = true;
    public float upMultiplier = 1f;
    public Vector3 gravityDir;
    public float animalHeight;
    public Rigidbody rb;
    public float distToGround;
    public float desiredHeight;
    public GameObject headHeightPosObj;
    public float lerpSpeed = 3;
    
    public Vector3 vertForce;
    private int layerMask;


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + acceleration);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + force * 10);

        if (arriveEnabled)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetTransform.position, slowingDistance);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        if(core == null)
            core = GameObject.Find("Core");
        rb = GetComponent<Rigidbody>();
        desiredHeight = animalHeight * .8f;
        //temp for testing
        targetTransform = GameObject.Find("Player").transform;
        layerMask = 1 << 8;//bit shift to get mask

    }

    public Vector3 Seek(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        Vector3 desired = toTarget.normalized * maxSpeed;

        return (desired - velocity);
    } 

    public Vector3 Arrive(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        float dist = toTarget.magnitude;
        float ramped = (dist / slowingDistance) * maxSpeed;
        float clamped = Mathf.Min(ramped, maxSpeed);
        Vector3 desired = (toTarget / dist) * clamped;

        return desired - velocity;
    }

    //For setting the target to the a pos above terrain instead of aiming through ground
    public Vector3 TerrainPosCorrecting(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, toTarget, out hit, layerMask))
        {
            return hit.point + (-gravityDir * desiredHeight);
        }
        else
        {
            return target;
        }
    }

    public Vector3 CalculateForce()
    {
        movePosTransform = TerrainPosCorrecting(targetTransform.position);


        Vector3 f = Vector3.zero;
        if (seekEnabled )
        {
            if (targetTransform != null)
            {
                f += Seek(movePosTransform);
            }
        }

        if (arriveEnabled)
        {
            if (targetTransform != null)
            {
                f += Arrive(movePosTransform);
            }
        }
        
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, -transform.up, out hit,layerMask))
        {
            distToGround = hit.distance;
            float distToDesired = Mathf.Abs(distToGround - desiredHeight);
            
            //Lerp height, force was taking too long to do so i may come back to it
            HeightPositioning(hit);

        }
        else//nothing below
        {
//            print("below ground");
            transform.position = transform.position+(Vector3.up*5);
        }
        return f;
    }
    

    
    public void HeightPositioning(RaycastHit hit)
    {
        
        Vector3 transformPosition = transform.position;
        Vector3 lerpedPos = hit.point + (-gravityDir * desiredHeight);
        
        if (hit.distance != desiredHeight)//is too low so go higher
        {
            transform.position = Vector3.Lerp(transform.position, lerpedPos, lerpSpeed*Time.deltaTime);
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gravityDir = (core.transform.position-transform.position).normalized;//todo flip dir
 
        if(Vector3.Distance(targetTransform.position,transform.position)>2)
            force = CalculateForce();
        acceleration = force / mass;
        if(targetTransform!=null)
            rb.velocity = rb.velocity + acceleration * Time.deltaTime;
        
        //transform.position = transform.position + velocity * Time.deltaTime;



        Quaternion rotation;
        speed = rb.velocity.magnitude;
        if (targetTransform != null)
        {
            rotation = Quaternion.LookRotation(rb.velocity, -gravityDir);
            transform.rotation = rotation;
        }
        else
        {
            transform.up = -gravityDir;
        }
    }
}
