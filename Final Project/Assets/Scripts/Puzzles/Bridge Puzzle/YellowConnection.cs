using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


//Not used
public class YellowConnection : MonoBehaviour
{
    [SerializeField]
    BoxCollider bridgeBase;
    [SerializeField]
    BoxCollider floor;
    GameObject[] gos;
    Color originalColor = Color.yellow;
    bool isValid = false;
    bool isSnapped = false;
    GameObject[] curObject = new GameObject[2];
    GameObject[] snappedObject = new GameObject[2];

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (!isSnapped)   //if nothing is in snapZone, checks ID for snapping 
        {

            if (other.gameObject.name.StartsWith("YBridge") && other.gameObject.layer == 8)   //checks for black bridge piece
            {


                curObject[0] = other.gameObject;
                curObject[1] = other.transform.parent.gameObject;       //the actual object we talk to

                isValid = true;
                GameObject[] objs = other.gameObject.GetComponent<MatGrabber>().getObjsToChange();
                gos = objs;
                foreach (GameObject go in objs)
                {
                    go.GetComponent<Renderer>().material.color = Color.green;   //sets valid snap color
                }
                //originalColor = objs[0].gameObject.GetComponent<Renderer>().material.color;

                this.GetComponent<XRSocketInteractor>().socketActive = true;
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



}
