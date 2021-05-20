﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//is a snapZone for the bridge pieces, attaches at the transform point
public class BridgeSnapTrigger : MonoBehaviour
{
    [SerializeField]
    BoxCollider bridgeBase;
    [SerializeField]
    BoxCollider floor;  //not used
    GameObject[] gos;

   
   
    [SerializeField]
    GameObject objToAttach;
    bool isValid = false;
    bool isSnapped = false;
    GameObject[] curObject = new GameObject[1];
    GameObject[] snappedObject = new GameObject[1];
    [SerializeField]
    Transform attachTransform;

    GameObject snappedItem;

    [SerializeField]
    int pieceOrder;                     //set in inspector

    void Start()    //set default color of objects
    {
        
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (!isSnapped)   //if nothing is in snapZone, checks ID for snapping 
        {
           

            if (other.gameObject == objToAttach
                && other.gameObject.layer == 8 
                && other.gameObject.GetComponent<BridgePieceGrab>().GetGrabbed() == true)   //checks for black bridge piece and if player is holding it
            {


                curObject[0] = other.gameObject;
                //curObject[1] = other.transform.parent.gameObject;       //the actual object we talk to

                isValid = true;
                GameObject[] objs = other.gameObject.GetComponent<MatGrabber>().getObjsToChange();
                gos = objs;
                foreach (GameObject go in objs)
                {
                    
                    go.GetComponent<Renderer>().material.color = Color.green;   //sets valid snap color
                }
                //originalColor = objs[0].gameObject.GetComponent<Renderer>().material.color;

                // this.GetComponent<XRSocketInteractor>().socketActive = true;
            }
            else if (other.gameObject.layer == 10)              //floor check, do nothing
            { }
            else
            {
                if (other.gameObject.GetComponent<MatGrabber>() != null && !isValid)
                {
                    GameObject[] objs = other.gameObject.GetComponent<MatGrabber>().getObjsToChange();
                    foreach (GameObject go in objs)
                    {
                        go.GetComponent<Renderer>().material.color = Color.red;    //sets invalid snap color
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isSnapped)   //if not snapped, reset colots of objects to 
        {

            if (other.gameObject.layer != 10)
            {
                if (other.gameObject.GetComponent<MatGrabber>() != null)
                {
                    GameObject[] objs = other.gameObject.GetComponent<MatGrabber>().getObjsToChange();
                    foreach (GameObject go in objs)
                    {
                        go.GetComponent<Renderer>().material.color = other.GetComponent<BridgePieceGrab>().GetColor();
                    }
                }

            }
            if (other.gameObject.name.StartsWith("BBridge") || other.gameObject.name.StartsWith("YBridge") 
                || other.gameObject.name.StartsWith("PBridge"))  //if was valid snap ID, delete reference to snapped item 
            {
               // this.GetComponent<XRSocketInteractor>().socketActive = false;
                for (int i = 0; i < curObject.Length; i++)
                {
                    curObject[i] = null;
                }
            }
            //originalColor = Color.clear;
            gos = null;
            isValid = false;
        }
    }

    public void Update()
    {
        if (isValid && curObject[0].GetComponent<BridgePieceGrab>().GetGrabbed() == false)
        {
            curObject[0].transform.position = attachTransform.position;
            curObject[0].transform.rotation = attachTransform.rotation;
            snappedItem = curObject[0];
            isValid = false;
            SnapObject();
        }

   
    }

    public bool GetSnapped()
    {
        return isSnapped;
    }
    public GameObject getSnappedObject()
    {
        return snappedItem;
    }
    public void SnapObject()
    {
        print("Snap");
        isSnapped = true;
        //playSound here
        if (gos != null)
        {
            GameObject[] objs = gos;
            foreach (GameObject go in objs)
            {
                if (GrabMgr.getGrabbed().Contains(go))
                {
                    GrabMgr.RemoveGrabbed(go);
                }

                go.GetComponent<BridgePieceGrab>().SetTestTrigger(true);
                //go.GetComponent<BridgePieceGrab>().OnRelease();
                go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY
                        | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY
                        | RigidbodyConstraints.FreezePositionZ;

                go.GetComponent<Attractable>().setAttractable(false);
                // go.GetComponent<BridgePieceGrab>().ResetSize();

                go.GetComponent<Renderer>().material.color = go.GetComponent<BridgePieceGrab>().GetColor() ;
               // this.GetComponentInChildren<AttachOrientationCorrection>().AdjustOrientation();
                go.layer = 10; ///makes object ground for gravity calculations




            }
        }
        if (curObject[0] != null)
        {
            for (int i = 0; i < curObject.Length; i++)
            {



                //curObject[i].GetComponent<Rigidbody>().isKinematic = true;

                Physics.IgnoreCollision(bridgeBase, curObject[i].GetComponent<BoxCollider>(), true);
                //Physics.IgnoreCollision(curObject[i].GetComponent<BoxCollider>(), this.GetComponentInChildren<BoxCollider>(), true);
            }

            snappedObject = curObject;
        }

        // this.GetComponentInChildren<BoxCollider>().enabled = true; //sets fake trigger so chair can stand on legs

        // AssemblyProgressCounter.S.IncrementSnap();
    }

    public void UnsnapObject()
    {
        print("Unsnap");

        isSnapped = false;
        snappedItem = null;
        if (snappedObject[0] != null)
        {
            for (int i = 0; i < snappedObject.Length; i++)
            {

                snappedObject[i].GetComponent<BridgePieceGrab>().SetTestTrigger(false);
                snappedObject[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                //snappedObject[i].transform.localScale = snappedObject[i].GetComponent<BridgePieceGrab>().getOriginalSize();
                //snappedObject[i].GetComponent<Attractable>().setAttractable(false);
                //snappedObject[i].GetComponent<Renderer>().material.color = originalColor;


                snappedObject[i].gameObject.layer = 8; //back to grabbable layer

                curObject[i].GetComponent<Attractable>().setAttractable(true);
                Physics.IgnoreCollision(bridgeBase, snappedObject[i].GetComponent<BoxCollider>(), false);
                snappedObject[i] = null;
            }
        }
        //this.GetComponentInChildren<BoxCollider>().enabled = false; //disables fake collider when unsnapped
        //AssemblyProgressCounter.S.DecrementSnap();
    }
}
