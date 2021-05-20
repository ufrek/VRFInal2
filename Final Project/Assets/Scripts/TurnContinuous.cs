using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


//handles turning and jumps on the right controller
public class TurnContinuous : MonoBehaviour
{

    [SerializeField]
    XRNode inputSource;
    [SerializeField]
    float turnSpeed = 1;
    [SerializeField]
    float deadZone = .2f;
    [SerializeField]
    float snapAmt = 45;
    [SerializeField]
    float snapCoolDownTime = .2f;
    [SerializeField]
    float damping = .1f;
    public float jumpForce = 440;

    private bool isTrigger;
    private bool isTriggerHeld = false;
    bool snapReady = true;
    bool jumpPressed;
    bool jumpHeld = false;
    

    XRRig rig;
    Vector2 inputAxis;
    CharacterController character;
    Vector3 prevPos;

    // Start is called before the first frame update
    void Start()
    {
        prevPos = this.transform.position;
        character = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();
    }

    // Update is called once per frame
    void Update()   //checks controller state and implements jumping mechanics
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);       //not sure if this will help or not

        //buttonPressEvent
        device.TryGetFeatureValue(CommonUsages.triggerButton, out isTrigger);
        device.TryGetFeatureValue(CommonUsages.primaryButton, out jumpPressed);
        if (isTrigger)
        {
        
            if (isTriggerHeld)
            {
               // ButtonHeldEvent.Invoke();
            }
            else
            {
                isTriggerHeld = true;
                //ButtonDownEvent.Invoke();
            }

        }
        else
        {
            //ButtonUpEvent.Invoke();
            isTriggerHeld = false;
        }

        if (jumpPressed)// && ContinuousMovement.isGrounded)
        {
                if (PController.isGrounded && !jumpHeld)
                {
                    jumpHeld = true;
                    print("jump");
                    PController.isJumping = true;
                // this.GetComponent<PController>().SuspendGroundCheck();
                //Vector3 jumpDir = (this.transform.position - prevPos).normalized;             //doesn't work....

                //this block is for properly scaling jump in all directions, the physics calculations made this difficult to figure out
                float force = jumpForce;
                Vector2 inputDirection = this.GetComponent<PController>().GetLeftAxis();
               /* if (inputDirection.x > 0)
                {
                    force = jumpForce / 2;
                }
                else if (inputDirection.x < 0)
                {
                    force = jumpForce / 2;
                }
                else
                    force = jumpForce; */

                if (inputDirection.y < 0)
                {
                    force = jumpForce;
                }
                if (inputDirection.y > 0)
                    force = jumpForce * 1.5f;

                this.GetComponent<Rigidbody>().AddForce((transform.up).normalized * force);
                    
                    
                    //this block dictates where to put extra momentum for some extra direction in the jump
                    Vector3 jumpDir = new Vector3(0,0,0);
                    if (inputDirection.x > 0)
                    {
                        if (inputDirection.y > 0)
                        {
                            jumpDir = (this.transform.forward + this.transform.right).normalized;
                        }
                        else if (inputDirection.y < 0)
                        {
                            jumpDir = (this.transform.forward - this.transform.right).normalized;
                        }
                        else
                            jumpDir = this.transform.forward.normalized;
                    }
                    else if (inputDirection.x < 0)
                    {

                        if (inputDirection.y > 0)
                        {
                            jumpDir = (-this.transform.forward + this.transform.right).normalized;
                        }
                        else if (inputDirection.y < 0)
                        {
                            jumpDir = (-this.transform.forward - this.transform.right).normalized;
                        }
                        else
                            jumpDir = -this.transform.forward.normalized;
                    }
                    else
                    {
                        if (inputDirection.y > 0)
                        {
                            jumpDir = (this.transform.right).normalized;
                        }
                        else if(inputDirection.y < 0)
                        {
                            jumpDir = (-this.transform.right).normalized;
                        }
                    }

                    this.GetComponent<Rigidbody>().AddForce(jumpDir * jumpForce / 10);             //adds a bit of directional momentum
                }

            
        }
        else
            jumpHeld = false;
    }

   
    void FixedUpdate()      //all turning mechanics are here
    {
        if (Mathf.Abs(inputAxis.x) < deadZone)
        {
            inputAxis.x = 0;
        }

        Quaternion currentRot = this.transform.rotation;
        List<GameObject> grabbedObjects = GrabMgr.getGrabbed();

        Quaternion newRot = new Quaternion();
        if (isTrigger && inputAxis.x != 0)
        {
             //allows for correct rotation in regards to orientation on planet
            if (snapReady)
            {

                if (inputAxis.x > 0)
                {

                    //newRot = currentRot * Quaternion.Euler(1, snapAmt, 1);
                    transform.Rotate(0, 45, 0);

                    foreach (GameObject go in grabbedObjects)
                    {
                        go.transform.Rotate(0, 45, 0);
                    }
                }
                else
                {
                    transform.Rotate(0, -45, 0);
                    foreach (GameObject go in grabbedObjects)
                    {
                        go.transform.Rotate(0, -45, 0);
                    }
                    //newRot = currentRot * Quaternion.Euler(1, -snapAmt, 1);
                }


                snapReady = false;
                StartCoroutine(SnapCoolDown());
            }

            prevPos = this.transform.position;

        }
        else
        {


            
               
        }

          

        //upate rotation
       


    }
    void LateUpdate()   //manual slow turn implementation
    {

        if (!isTrigger && inputAxis.x != 0)
        {
           
            Quaternion newRot = this.transform.rotation * Quaternion.Euler(0, (inputAxis.x * Time.deltaTime * turnSpeed), 0);
            //this.transform.rotation = newRot;
            this.transform.Rotate(Quaternion.Euler(0, (inputAxis.x * Time.deltaTime * turnSpeed), 0).eulerAngles);
        }
    }
   
    public Quaternion getTurnAmt()
    {
        return Quaternion.Euler(0, (inputAxis.x * Time.deltaTime * turnSpeed), 0);
    }


    IEnumerator SnapCoolDown()
    {
        yield return new WaitForSeconds(snapCoolDownTime);
        snapReady = true;
    }
}
