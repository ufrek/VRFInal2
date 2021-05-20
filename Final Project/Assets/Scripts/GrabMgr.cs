using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

//Maintains references to what objects are grabbed and which helps ID which hand is doing the grab action
public class GrabMgr : MonoBehaviour
{
    GameObject player;
    //Controller vars
    [SerializeField]
    XRNode leftHandSource;
    [SerializeField]
    XRNode rightHandSource;
    XRRig rig;
    bool leftGrabPressed = false;
    bool leftHeld = false;
    bool rightGrabPressed = false;
    bool rightHeld = false;
    static int lastGrabbed = -1;


    static List<GameObject> grabbedObjects;
    // Start is called before the first frame update
    void Start()
    {
        grabbedObjects = new List<GameObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        rig = player.GetComponent<XRRig>();
    }

    public static void AddGrabbed(GameObject go)
    {
        grabbedObjects.Add(go);
        print("Added: " + go.name);
    }

    public static void RemoveGrabbed(GameObject go)
    {
        if (grabbedObjects.Contains(go))
        {
            grabbedObjects.Remove(go);
            print("Removed: " + go.name);
        }
        else
            print("remove grabbed couldn't find object");
    
    }

    public static List<GameObject> getGrabbed()
    {
        return grabbedObjects;
    }
    // Update is called once per frame
    void Update()
    {
        InputDevice leftDevice = InputDevices.GetDeviceAtXRNode(leftHandSource);
        InputDevice rightDevice = InputDevices.GetDeviceAtXRNode(rightHandSource);

        leftDevice.TryGetFeatureValue(CommonUsages.gripButton, out leftGrabPressed);
        if (leftGrabPressed)
        {

            if (leftHeld)
            {
                // ButtonHeldEvent.Invoke();
            }
            else
            {
                leftHeld = true;
                lastGrabbed = 0;
                //ButtonDownEvent.Invoke();
            }

        }
        else
        {
            //ButtonUpEvent.Invoke();
            leftHeld = false;
        }

        rightDevice.TryGetFeatureValue(CommonUsages.gripButton, out rightGrabPressed);
        if (rightGrabPressed)
        {

            if (rightHeld)
            {
                // ButtonHeldEvent.Invoke();
            }
            else
            {
                rightHeld = true;
                lastGrabbed = 1;
                //ButtonDownEvent.Invoke();
            }

        }
        else
        {
            //ButtonUpEvent.Invoke();
            rightHeld = false;
        }


    }

    public static  int getLastGrabbed()
    {
        return lastGrabbed;
    }
}
