using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

//Code for when ball is grabbed
public class BallGrab : ExtendedGrab
{
  
    int activeHand;        //0 for left hand, 1 for right hand, -1 for no hand
    
    GameObject player;
    bool isGrabbed = false;

    //Controller vars
    [SerializeField]
    XRNode leftHandSource;
    [SerializeField]
    XRNode rightHandSource;
    XRRig rig;
    Vector3 throwAngularVelocity;
    Vector3 throwPosition;
    [SerializeField]
    float throwForce = 500;
    [SerializeField]
    GameObject startingPlanet;
    [SerializeField]
    GameObject rightHand;

    GameObject transitionedPlanet = null;

    public GameObject getStartingPlanet()
    {
        return startingPlanet;
    }
    public void SetStartingPlanet(GameObject go)
    {
        startingPlanet = go;
    }

    public GameObject getTransitionedPlanet()
    {
        return transitionedPlanet;
    }

    public void SetTransitionedPlanet(GameObject go)
    {
        transitionedPlanet = go;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rig = player.GetComponent<XRRig>();
        transitionedPlanet = startingPlanet;    //basic setup so it won't go null
    }

    //disables gravity and ignores collision with the player to prevent you from being shot into space
    public override void OnGrab()
    {
        print("grab");
        isGrabbed = true;
        GrabMgr.AddGrabbed(this.gameObject);
        this.GetComponent<BallRespawner>().SetHolding(true);
        Physics.IgnoreCollision(this.GetComponent<SphereCollider>(), player.GetComponent<CapsuleCollider>(), true);
        this.GetComponent<Attractable>().setAttractable(false);
        CheckActiveHand();
    }

    public override void OnRelease()
    {
        print("release");
        isGrabbed = false;
        GrabMgr.RemoveGrabbed(this.gameObject);

        this.GetComponent<Attractable>().setAttractable(true);
        this.GetComponent<BallRespawner>().SetHolding(false);
        AddThrowForce();
        // player.transform.TransformVector(OVRInput.getlo)
        activeHand = -1;
        Invoke("ResetCollider", 1f);    //prevents being stuck in player collider on release....most of the time
    }

    void ResetCollider()
    {
        Physics.IgnoreCollision(this.GetComponent<SphereCollider>(), player.GetComponent<CapsuleCollider>(), false);
    }

    void CheckActiveHand()  //checks which hand is holding the ball
    {
        activeHand = GrabMgr.getLastGrabbed();
    }

    void AddThrowForce()    //adds a little extra zip to thrown objects so they go farther to accomodate for gravitational forces, probably overkill
    {
        CheckActiveHand();

        switch (activeHand)
        {
            case 0: //left hand
                InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftHandSource);
                leftDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out throwAngularVelocity);
                Vector3 tossDirL = throwForce * -throwAngularVelocity;
                this.GetComponent<Rigidbody>().AddForce(tossDirL);   //hopefully throws the object farther
                break;
            case 1:
                InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(rightHandSource);
                rightDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out throwAngularVelocity);
                rightDevice.TryGetFeatureValue(CommonUsages.devicePosition, out throwPosition);
                Vector3 tossDirR = throwForce  * rightHand.transform.forward * throwAngularVelocity.magnitude;
                this.GetComponent<Rigidbody>().AddForce(tossDirR);   //hopefully throws the object farther
                break;
            default:
                print("error in hand ID for throwing. can't add force");
                break;
        }
    }

 
    // Update is called once per frame
    void Update()
    {
        
    }
}
