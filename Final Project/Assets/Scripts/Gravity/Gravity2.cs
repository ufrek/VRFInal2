using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Still glitches on edges of Cube Bridges. 
//Controls the gravity of the planet and the orientation of objects in the gravitational pull
//Ignore the Bridge, Cube Bridge, and Grav Cube stuff...it's meant for different planetary shapes
public class Gravity2 : MonoBehaviour
{
    
    public float gravity = -100.0f;
    [SerializeField]
    float jumpGravity = -9.8f;

    public float cutoffDistance = 10.0f;
    public bool isInGravityPull = false;
    [SerializeField]
    public List<GameObject> attractableObjects;
    public GameObject gravCube;
    public Rigidbody planet;
    public Rigidbody player;
    bool isFound;                   //visible for debugging purposes,
    
    public float checkRadius = 5f;
    public float checkDistance = 50;
    public LayerMask bridgeGround;
    public Vector3 attractionPoint;     //if it finds the surface, we use a different vector for attraction, if not, we use the default center vector until we hit something with a spherecast
    public Vector3 normalPoint;
    public float rotDamp = 5;
    public planetShape shape;
    Vector3 playerMove;
    public Quaternion rot;
    public bool isBridgeTransitioning = false;
    public bool isCircleTransitioning = false;

	public static     bool isSurfaceLost = false;
    public float surfaceRefreshRate = 1f;
    bool rayDelay = true;
    Vector3 prevAttraction = Vector3.zero;
    Vector3 prevNormal = Vector3.zero;
    public float jitterTolerance = .3f;
    Vector3 prevPosition;

    Quaternion prevRot = Quaternion.identity;
    float rotJitterTolerance = .03f;

    public bool isVelocityInNeedOfSmoothing = false;

    public static bool isTurning = false;

    Vector3 prevUp;
   public enum planetShape                    //might want to add more later if we want some different things
    {
        sphere,
        bridge,
        cubeBridge,
        node                            //might just make it so you can float around, not sure yet
    };

    void Awake()    //figures out which shape's gravity to use
    {
       
        planet = this.gameObject.GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();

        if (planet.gameObject.tag == "Element")
            shape = planetShape.sphere;
        else if (planet.gameObject.tag == "Bridge")
        {
            shape = planetShape.bridge;
        }
           
        else if (planet.gameObject.tag == "Node")
            shape = planetShape.node;
        else if (planet.gameObject.tag == "CubeBridge") 
        {
            shape = planetShape.cubeBridge;
            gravCube = this.GetComponentInChildren<GravCube>().gameObject;
            gravCube.GetComponent<GravCube>().enabled = false;
        }
           
        else
            print("Planet Shape is not defined. Please Set Shape in Inspector for this Object.");

    }


    

    public IEnumerator TriggerGravCube() //used on cube bridge only
    {
        while (!PController.isGrounded)
        {
            yield return new WaitForSeconds(.1f);
            gravCube.GetComponent<GravCube>().enabled = false; //hides the gravity sources from view
        }
       // print("GravCube Engaged");
        gravCube.GetComponent<GravCube>().enabled = true;
    }

    public void  Start()                                                       
    {
        StartCoroutine(DeactivateInfluence()); 
        //shuts off gravity field around all planets, lets player controller find current planet
    }

    IEnumerator DeactivateInfluence()
    {
        yield return new WaitForSeconds(2);
        this.GetComponentInParent<GravityInfluence>().enabled = false;

        if (this.gameObject.tag == "Bridge")
        {
            this.GetComponentInParent<CapsuleCollider>().enabled = false;
            this.GetComponent<CapsuleCollider>().enabled = true;
        }
        switch (this.gameObject.tag)                                                          
        {
            case "CubeBridge":
                this.GetComponentInParent<CapsuleCollider>().enabled = false;

                break;
            case "Bridge":
             
                break;
            case "Element":
                this.GetComponentInParent<SphereCollider>().enabled = false;
                this.GetComponent<SphereCollider>().enabled = true;
                break;
        }
    }


    //gravity function for planet
    //makes the object stand upright on the planet regardless of orientation on planet
    public void Attract(Rigidbody body)  
    {
        
            Vector3 gravityUp = (body.position - this.transform.position).normalized;
            Vector3 localUp;
           // float grav = UniversalGravitation(planet, player);                //NOT USED: enable if you are trying out the alternative physics methods
            
            switch (shape)
            {

      //USED IN THIS PROJECT
                case planetShape.sphere:
                    Ray ray = new Ray(player.transform.position, -(player.transform.up));
                    RaycastHit hit;
                    Physics.Raycast(ray, out hit, checkDistance, bridgeGround);
                if (hit.collider != null && !hit.collider.gameObject.tag.Equals("Element"))
                {
                    print("working");
                    // gravityUp = Vector3.Slerp(gravityUp, (body.position-hit.collider.gameObject.transform.position).normalized, Time.fixedDeltaTime);
                    //gravityUp = (body.position - this.transform.position).normalized;
                    Vector3 newGrav = (hit.collider.ClosestPoint(body.position) - this.transform.position).normalized;
                    gravityUp = Vector3.Slerp(gravityUp, newGrav, Time.fixedDeltaTime);

                }
                else
                {
                    gravityUp = Vector3.Slerp(gravityUp, (body.position - this.transform.position).normalized, Time.fixedDeltaTime);
                    //gravityUp = (body.position - this.transform.position).normalized;

                }
                   
                    
                localUp = body.transform.up;


             
                    //body.AddForce(gravityUp * jumpGravity); //kinda wonky feeling
                    body.AddForce(gravityUp *  gravity);
                

                    
                    //realign body

                    if ( isCircleTransitioning)        //makes player stand upright on the planet                                                                
                    {
                        
                        rot = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
                        body.rotation = Quaternion.Slerp(body.rotation, rot, Time.fixedDeltaTime + 0);
                        
                    }
                    else
                    {
                        rot = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
                        body.rotation = Quaternion.Slerp(body.rotation, rot, Time.fixedDeltaTime + 0.3f);
                       // body.rotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;                   
                       
                    }
                    
               
                    break;
                case planetShape.cubeBridge:  //not used
                        CheckSurface(body);
                    gravityUp = normalPoint; 
                    localUp = body.transform.up;

                    //applies gravity
                    body.AddForce(gravityUp * gravity);
                    
                    //smoothe out rotation rotation
                    // rot = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
                    //body.rotation = Quaternion.Slerp(body.rotation, rot,  Time.fixedDeltaTime + 0);
                    if (!PController.isGrounded)
                    {
                        rotDamp = .021f;
                    }
                    else
                    {
                        rotDamp = .05f;
                    }

                    if (isCircleTransitioning)                                                                                     //does this when changing planets, has less damping, so is more smoothe
                    {
                        rot = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
                        body.rotation = Quaternion.Slerp(body.rotation, rot, Time.fixedDeltaTime + 0);
                    }
                    else
                    {
                        rot = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
                        body.rotation = Quaternion.Slerp(body.rotation, rot, Time.fixedDeltaTime + rotDamp);
                        // body.rotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;                    

                    }
                    break;
                case planetShape.bridge:
                        CheckSurface(body);
                    gravityUp = normalPoint; 
                    localUp = body.transform.up;

                    //applies gravity
                
                        body.AddForce(gravityUp * gravity);

                        //smoothe out rotation rotation
                        if (isCircleTransitioning)
                        {
                            rot = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
                            body.rotation = Quaternion.Slerp(body.rotation, rot, Time.fixedDeltaTime + 0);
                        }
                        else
                        {
                                rot = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
                                body.rotation = Quaternion.Slerp(body.rotation, rot, Time.fixedDeltaTime + .3f);
                            
                     
                      
                        } 
                    
                    
               

                    prevAttraction = attractionPoint;
                    prevNormal = normalPoint;
                    prevPosition = player.transform.position;
                    break;


                //case planetShape.node:                                                              //NOT USED
                //    gravityUp = (body.position - this.transform.position).normalized;
                //    localUp = body.transform.up;

                //    //applies gravity
                //    body.AddForce(gravityUp * gravity);
                //    //realign body
                //    body.rotation = Quaternion.FromToRotation(-localUp, gravityUp) * body.rotation;                             //***need to change the orientation to something that fits for each node......


                //    break;
            } 

 
    }
    public void FixedUpdate()
    {   
   
        if (isInGravityPull && !isVelocityInNeedOfSmoothing)
            Attract(player);
        foreach (GameObject go in attractableObjects)
        {
            Attractable a = go.GetComponent<Attractable>();
            if (a != null && a.getAttractable() == true)
            {
                Attract(go.GetComponent<Rigidbody>());
            }
            
        }

    }

    public Quaternion checkTurning(Quaternion rot) //adds player input turning to updated orientation
    {
        if (isTurning)
        {
            Quaternion newRot = rot * player.GetComponent<TurnContinuous>().getTurnAmt();
            return newRot;
        }
        else
            return rot;

    }

   
    //shoots a ray downward and checks if the ground is found, if not, let's the player float for a bit before checking again
    //ensures smooth floating motion
    //essentially, the point you are attracted to on a sphere is directly underneath the player
    //if, for some reason it loses track of the ground, it coasts around the last found point until the ground is found again
    public void CheckSurface(Rigidbody body)     
    {
        Ray ray = new Ray(player.transform.position , -(player.transform.up) );
        RaycastHit hit;

        if (shape == planetShape.cubeBridge)   //not going to implement cube bridges in this project
        {
            if (GravCube.isAnchored)                                                                                            //smoother, but needs some kind of check fo rtray casting here
            {
                if (rayDelay)
                {
                    StartCoroutine(RayDelay(surfaceRefreshRate));
                    //print("cube");
                    attractionPoint = gravCube.gameObject.transform.position;
                    normalPoint = (body.position - gravCube.transform.position).normalized;
                }

            }

            if (!rayDelay && !isSurfaceLost && Physics.Raycast(ray, out hit, checkDistance, bridgeGround))  //NOT USED:      (Physics.SphereCast(ray, checkRadius, out hit, checkDistance, bridgeGround))                 //spherecast doesn't want to work
            {
                Vector3 jitterDist = hit.point - prevAttraction;

                if (!isBridgeTransitioning && jitterDist.magnitude > jitterTolerance && jitterDist.magnitude < 1 )                   //smoothes out any slight rotations when walking around
                {
                    player.transform.position = prevPosition;
                    print("hold");
                    attractionPoint = prevAttraction;
                    normalPoint = prevNormal;

                }
                else
                {

                    attractionPoint = hit.point;
                    normalPoint = hit.normal;
                }


            }
            else
            {
                if (!isSurfaceLost) //raycast may occassionally lose sight of the ground, just coast for a bit and try again
                {
                    isSurfaceLost = true;
                    StartCoroutine(JumpSmoother(surfaceRefreshRate)); //makes jumps less jittery
                    rayDelay = true;
                }

                if (GravCube.isAnchored && gravCube != null)
                {
                    //print("cube");
                    attractionPoint = gravCube.gameObject.transform.position;
                    normalPoint = (body.position - this.transform.position).normalized;
                }

                else
                {
                   // print("fail");
                    attractionPoint = this.gameObject.transform.position;
                    normalPoint = (body.position - this.transform.position).normalized;
                }
            }
        }
        else    //gravity checker for cylinder/sphere
        {


            if (rayDelay)
            {
                StartCoroutine(RayDelay(surfaceRefreshRate));
                // print("cube");
                attractionPoint = this.transform.position;
                normalPoint = (body.position - this.transform.position).normalized;
            }



            if (!rayDelay && !isSurfaceLost && Physics.Raycast(ray, out hit, checkDistance, bridgeGround)) //NOT USED: (Physics.SphereCast(ray, checkRadius, out hit, checkDistance, bridgeGround))                 //spherecast doesn't want to work
            {
                Vector3 jitterDist = hit.point - prevAttraction;
               
                if (!isBridgeTransitioning && jitterDist.magnitude > jitterTolerance  && (PController.isGrounded && !PController.isMoving))          //cancels jitter on cube, works as is
                {
                    //player.transform.position = prevPosition;
                    //print("hold");
                    //attractionPoint = prevAttraction;
                    //normalPoint = prevNormal;

                }
                else 
                {

                    attractionPoint = hit.point;
                    normalPoint = hit.normal;
                }
        

            }
            else
            {
                // print("fail");
                attractionPoint = this.gameObject.transform.position;
                normalPoint = (body.position - this.transform.position).normalized;
            }
        }
    }

    //both of these are used to delay the downward raycasting and to help smooth floating if thedownward raycast can't find the ground.

    IEnumerator RayDelay(float refresh)
    {
        yield return new WaitForSeconds(refresh);
        rayDelay = false;

    }
        IEnumerator JumpSmoother(float refresh)
        {
            yield return new WaitForSeconds(refresh);
            isSurfaceLost = false;

        }

    public void AddAttractable(GameObject go)
    {
            attractableObjects.Add(go);
    }

    public void RemoveAttractable(GameObject go)
    {
        if (attractableObjects.Contains(go))
        {
            attractableObjects.Remove(go);
        }
    }

    public bool CheckAttractableList(GameObject go)
    {
        if (attractableObjects.Contains(go))
        {
            return true;
        }
        else
            return false;
    }

    public static void TransitionPlanetGravity(GameObject planetA, GameObject planetB, GameObject obj)
    {
       
        planetA.GetComponent<Gravity2>().RemoveAttractable(obj);

        if (planetB.GetComponent<Gravity2>().CheckAttractableList(obj) == false)
        {
            planetB.GetComponent<Gravity2>().AddAttractable(obj);
            
            if (obj.tag.Equals("Ball"))
            {
                GameObject sPlanet = obj.GetComponent<BallGrab>().getStartingPlanet();
                if (planetB.transform.position != sPlanet.transform.position)
                {
                   
                    obj.GetComponent<BallGrab>().SetTransitionedPlanet(planetB);
                }

                else
                    obj.GetComponent<BallGrab>().SetTransitionedPlanet(sPlanet);
            }
        }
      
        //coll.GetComponent<BallGrab>().SetStartingPlanet(planet.gameObject); //if you want to make the transition permanent

        //Check obj ID and do necessary functions 
      
        
    }

}


//------------Non-Functional Alternative Physics Methods for Reference----------------------------------


//private float UniversalGravitation(Rigidbody a, Rigidbody b)
//{
//    float distance = Vector3.Distance(a.transform.position, b.transform.position);
//    //if (distance >= cutoffDistance)                                                     //can make a queadratic fall off for gravity here
//    //    return 0.0f;

//    if (!this.isInGravityPull)
//        return 0.0f;

//    return gravity * (a.mass * b.mass) / Mathf.Pow(distance, 1);
//}




//private Vector3 CubeGravityDirection(Rigidbody planet, Rigidbody other)                     //not gonna use
//{
//    float bestDot = -1.0f;
//    Vector3 direction = Vector3.zero;

//    for (int index = 0; index < m_GravityDirections.Length; index++)
//    {
//        Vector3 gravityDirection = planet.transform.TransformVector(m_GravityDirections[index]).normalized;
//        Vector3 directionToOther = (other.transform.position - planet.transform.position).normalized;

//        float dot = Vector3.Dot(gravityDirection, directionToOther);

//        if (dot > bestDot)
//        {
//            direction = gravityDirection;
//            bestDot = dot;
//        }
//    }

//    return direction;
//}

//private Vector3 CubeGravityForce(Rigidbody planet, Rigidbody other)                         //not gonna use
//{
//    float gravity = UniversalGravitation(planet, other);
//    Vector3 direction = CubeGravityDirection(planet, other);

//    return gravity * direction * -1.0f;
//}