using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

//Code From https://www.youtube.com/watch?v=5NRTT8Tbmoc
//Not Used, but good source to cite ideas of locomotion
public class ContinuousMovement : MonoBehaviour
{   
    [SerializeField]
    XRNode inputSource;
    [SerializeField]
    float speed = 1;

    XRRig rig;
    Vector2 inputAxis;
    CharacterController character;
    [SerializeField]
    float additionalHeight = .2f;

    //Reimplement Gravity using PLanet Implementation
    [SerializeField]
    float gravity = -9.81f;
    [SerializeField]
    LayerMask groundLayer;
    float fallingSpeed;

    //added stuff
    public float walkSpeed = 4;
    public float detach_walkSpeed = 50;
 
    public Vector3 moveDir;
    public Vector3 prevMov;
    public static bool isGrounded;
    public Vector3 moveAmount;
    public LayerMask groundedMask;
    //public Camera cam;
    public static bool isJumping = false;
    Vector3 prevAmount;
    public static bool isMoving;
    Vector3 smoothMoveVelocity;
    Transform cameraTransform;
    Transform playerTransform;
    Rigidbody playerRigidBody;
    GameObject previousPlanet;

    void Awake()
    {

        cameraTransform = Camera.main.transform;
        playerRigidBody = this.GetComponent<Rigidbody>();
        playerTransform = this.GetComponent<Transform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();


        //finds every object in the radius of it's transform to determine planet we are standing on
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 6, groundedMask);
        float tempDistance = 200; //could mess with this if working with different size planets
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
                print(GravMgr.currentPlanet.name + "curPl");
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        //moves character
        prevAmount = moveAmount;
        Vector3 forwardDir = Camera.main.transform.forward.normalized;
        forwardDir = forwardDir * inputAxis.y;

        if (isGrounded && forwardDir != Vector3.zero)
            prevMov = forwardDir;

        Vector3 targetMoveAmount = forwardDir * walkSpeed;

        if (isGrounded)                                                                                             //player keeps moving in mid-air based on previous input rather than current input
            moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .1f);
        else
            moveAmount = prevAmount;

        if (moveAmount.magnitude < .3f)
            isMoving = false;
        else
            isMoving = true;

        //grounded check
        Ray ray = new Ray(transform.position, -transform.up);                   //may need to change this in my setup to child capsule
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
        {
            //print("grounded");
            isJumping = false;
            isGrounded = true;

        }
        else
        {
            isGrounded = false;
        }

    }

    void FixedUpdate()
    {
        CapsuleFollowHeadset();

        //actually moves the character
        //playerRigidBody.MovePosition(playerRigidBody.position + moveAmount * Time.fixedDeltaTime);


        //unused character motion
        Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw *  new Vector3(inputAxis.x, 0, inputAxis.y);
        character.Move(direction * Time.fixedDeltaTime * speed);

        //using planet gravity instead
        //gravity
        /* bool isGrounded = CheckIfGrounded();
         fallingSpeed += gravity * Time.deltaTime;
         character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);*/
    }

    void CapsuleFollowHeadset()
    {
        character.height = rig.cameraInRigSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2 + character.skinWidth, capsuleCenter.z);
    }

    bool CheckIfGrounded()
    {
        //tells us if on Ground
        //shoot from center of character
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + .01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, 
            out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
}
