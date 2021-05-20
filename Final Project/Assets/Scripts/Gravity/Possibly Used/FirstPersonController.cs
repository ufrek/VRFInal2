using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour 
{
    public float mouseSensitivityX = 1;
    public float mouseSensitivityY = 1;
    public float walkSpeed = 4;
    public float jumpForce = 220;
    public float cornerOffset = .5f;                      //distance threshold to check for glitches on corners
    public LayerMask groundedMask;
    public Vector3 moveDir;
    public static Vector3 prevMov;
   public static bool isGrounded;
   public static bool isJumping = false;
    public Vector3 moveAmount;
	public static Vector3 prevAmount;
	public static bool isMoving;

    Vector3 smoothMoveVelocity;
    float verticalLookRotation;
    Transform cameraT;
    Rigidbody rigidBody;
   

    
    GameObject[] nodes;
    public static bool isWalkOn = true;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraT = Camera.main.transform;
        rigidBody = this.GetComponent<Rigidbody>();
        nodes = GameObject.FindGameObjectsWithTag("Node");
    }
    void Start()                                                                                                //Initializes starting point
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 15, groundedMask);
        float tempDistance = 200;
        foreach (Collider col in hitColliders)
        {
            if (Vector3.Distance(this.transform.position, col.transform.position) < tempDistance)
            {
                tempDistance = Vector3.Distance(this.transform.position, col.transform.position);
                GravMgr.currentPlanet = col.gameObject.GetComponentInChildren<Gravity2>().gameObject;
                
                if (GravMgr.currentPlanet.tag == "CubeBridge")                                                                  //prevents you from flying off the cube at initialization
                {
                    GravMgr.currentPlanet.GetComponent<Gravity2>().isBridgeTransitioning = true;                        
                    StartCoroutine(GravMgr.currentPlanet.GetComponent<Glow>().BridgeTransition(1));
                }
                //print(GravMgr.currentPlanet.name + "curPl");
                break;
            }
        }


        if (GravMgr.currentPlanet.GetComponent<Gravity2>().shape == Gravity2.planetShape.cubeBridge)
            GravMgr.currentPlanet.GetComponent<Gravity2>().gravCube.GetComponent<GravCube>().enabled = true;
        print(GravMgr.currentPlanet.name + "is current planet");
        
    }

	// Update is called once per frame
	void Update ()
    {
        prevAmount = moveAmount;                                                                 //modify all this to work with an xbox controller
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -50, 70);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        //calculate movement
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        //nav method 1
         moveDir = new Vector3(inputX, 0, inputZ ).normalized;
         if (isGrounded && moveDir != Vector3.zero)
             prevMov = moveDir;
        //if (moveDir != Vector3.zero && isGrounded)
        //    prevMove = moveDir;
        Vector3 targetMoveAmount = moveDir * walkSpeed;



        if (isGrounded)                                                                                             //player keeps moving in mid-air based on previous input rather than current input
            moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .1f);
        else
            moveAmount = prevAmount;

        if (moveAmount.magnitude < .3f)
            isMoving = false;
        else
            isMoving = true;
      

        //if(Input.GetButtonDown("Fire1"))                                                            //map this to contrller later
        //{
        //    Gravity2 nearestNode = FindNearestNode();
        //    if (isWalkOn)
        //    {
        //        isWalkOn = false;
        //        nearestNode.isInGravityPull = true;
        //    }
        //    else
        //    {
        //        isWalkOn = true;
        //        nearestNode.isInGravityPull = false;
        //    }
            
        //}


        if (isWalkOn)
        {

            //jump nmechanics
            if (Input.GetButtonDown("Jump"))                                                    //map this to controller later
            {
                if (isGrounded)
                {
                    isJumping = true;
                    rigidBody.AddForce(transform.up * jumpForce);
                }
            }

            //grounded check
            Ray ray = new Ray(transform.position, -transform.up);                   //may need to change this in my setup to child capsule
            RaycastHit hit;

            if  (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
            {
                print("grounded");
                isJumping = false;
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            } 
        }
       
	}

    void FixedUpdate()
    {
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rigidBody.MovePosition(rigidBody.position + localMove);
    }



    Gravity2 FindNearestNode()                                                                  //Not USED : Porpose is for use of invisible nodes to float to
    {
       float distance = 9999999;
       GameObject shortest = null;
        foreach (GameObject go in nodes)
        {
            float tempDist = Vector3.Distance(this.transform.position, go.transform.position);
            if (tempDist < distance)
            {
                shortest = go;
                distance = tempDist;
            }
        }
        if (shortest != null)
            return shortest.GetComponent<Gravity2>();
        else
            print("Closest node can't be found");
            return null;
    }
}
