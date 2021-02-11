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

    public bool arriveEnabled = false;
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

    public Vector3 CalculateForce()
    {
        Vector3 f = Vector3.zero;
        if (seekEnabled )
        {
            if (targetTransform != null)
            {
                f += Seek(targetTransform.position);
            }
            
        }

        if (arriveEnabled)
        {
            if (targetTransform != null)
            {
                f += Arrive(targetTransform.position);
            }
            
        }
        
        
        //add grav
        /*
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, gravityDir, out hit,desiredHeight))
        {
            //HeightPositioning(hit);
            
            Vector3 upForce;
            
            distToGround = hit.distance;
            if (distToGround > desiredHeight+0.02)
                upForce = -(gravityDir * (gravityStrength));
                f = f + upForce;
            else
            {
                verticalF = Mathf.Abs(1 / ((hit.point.y - transform.position.y)));
                upForce = gravityDir * (verticalF * upMultiplier * animalHeight * Time.deltaTime);

                f = f + upForce;
            }
            
            //f = f+(gravityDir * (gravityStrength * Time.deltaTime));
            
            float upForce = 0;
            upForce = Mathf.Abs(1 / ((hit.point.y - transform.position.y)));
            //add upforce to fight gravity scaled to height of animal
            rb.AddForceAtPosition(gravityDir * (upForce * upMultiplier * animalHeight * Time.deltaTime),
                transform.position, ForceMode.Acceleration);
            
            

        }
        else
        {
            f = f+(gravityDir * (gravityStrength * Time.deltaTime));
        }
        */
        
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, gravityDir, out hit))
        {
            distToGround = hit.distance;
            float distToDesired = Mathf.Abs(distToGround - desiredHeight);
            
            //Lerp height, force was taking too long to do so i may come back to it
            HeightPositioning(hit);

            //Force gravity
            /*
            if (distToGround > desiredHeight + 0.1f)
            {
                //print("down");
                float downForce = 0;
                downForce = distToDesired*gravityStrength;//will be normal gravity unless its less than 1 then it will make gravity weaker
                downForce = Mathf.Clamp(downForce,0, gravityStrength);
                
                
                vertForce = (gravityDir * (downForce));
            }
            
            else if(distToGround < desiredHeight - 0.1f)
            {
                float upForce = 0;
                upForce = (gravityStrength / distToDesired);//* animalHeight
                upForce = Mathf.Clamp(upForce, 0,gravityStrength);
                
                
                print("upforce"+upForce);

                vertForce = -gravityDir * (upForce * upMultiplier);//* animalHeight
            }



            f = f+vertForce;
            */
        }


        /*
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, gravityDir, out hit))
        {
            headHeightPosObj.transform.position = hit.point + (-gravityDir * desiredHeight);
            Vector3 desiredHeadDir = (desiredHeadPos - transform.position).normalized;
            
            Vector3 newPosition;
            newPosition.y = Mathf.Lerp(this.transform.localPosition.z, desiredHeadPos.z, Time.deltaTime * 1);
            headHeightPosObj.transform.position = Vector3.Lerp((headHeightPosObj.transform.position.x,headHeightPosObj.transform.position.y,headHeightPosObj.transform.position.z),headHeightPosObj.transform.position,5*Time.deltaTime);
            
            
            transform.localPosition = newPosition;
            
            if(Vector3.Distance(transform.position,desiredHeadPos)>0.5f)
                f = f + (desiredHeadDir * (gravityStrength));
            else
            {
                f = f + (desiredHeadDir);
            }
            
        }
        */




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
 
        
        force = CalculateForce();
        acceleration = force / mass;
        if(targetTransform!=null)
            rb.velocity = rb.velocity + acceleration * Time.deltaTime;
        
        /*
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, -gravityDir, out hit,2))
        {
            float upForce = 0;
            upForce = Mathf.Abs(1 / ((hit.point.y - transform.position.y)));
            //add upforce to fight gravity scaled to height of animal
            rb.AddForceAtPosition(gravityDir * (upForce * upMultiplier * animalHeight * Time.deltaTime),
                transform.position, ForceMode.Acceleration);
            //rb.AddForceAtPosition((gravityDir * upBaseAmount) + (gravityDir * (upForce * upMultiplier*animalHeight)),transform.position,ForceMode.Acceleration);

        }
        */
        
        transform.position = transform.position + velocity * Time.deltaTime;



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
