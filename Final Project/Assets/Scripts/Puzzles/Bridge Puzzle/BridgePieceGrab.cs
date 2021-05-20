using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Allows for attach and detach of bridge pieces, resizes them based on if you're holding them or not
public class BridgePieceGrab : MonoBehaviour
{
    Vector3 originalSize;
    Color originalColor;
    GameObject player;
    bool isGrabbed = false;
    bool isSnapped = false;

    [SerializeField]
    BridgeSnapTrigger snapZone = null;
    [SerializeField]
    BoxCollider testTrigger;    //for the next subsequent attacment zone. lets you attach the next piece of the bridge

 
   
    

    // Start is called before the first frame update
    void Start()
    {
       // testTrigger.enabled = false;
        player = GameObject.FindGameObjectWithTag("Player");
        originalSize = this.transform.localScale;
        originalColor = this.GetComponent<Renderer>().material.color;
      
    }
    public Color GetColor()
    {
        return originalColor;
    }
    // Update is called once per frame
    
    void Update()
    {
        
    }

    public Vector3 getOriginalSize()
    {
        return originalSize;
    }
    
    public void onGrab()
    {
        if (snapZone.GetSnapped() == true && this.gameObject == snapZone.getSnappedObject())                  //makes sure to re-enable gravity and physics
        {
            snapZone.UnsnapObject();
        }

        print("grab");
        isGrabbed = true;  
        //this.transform.parent = player.gameObject.transform;
        this.transform.localScale = originalSize * .5f;
        GrabMgr.AddGrabbed(this.gameObject);
        Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), player.GetComponent<CapsuleCollider>(), true);
        this.GetComponent<Attractable>().setAttractable(false);
    }

    public void SetTestTrigger(bool a)
    {
        testTrigger.enabled = a;
    }

    public void OnRelease()
    {
       
        print("release");
        isGrabbed = false;
        GrabMgr.RemoveGrabbed(this.gameObject);
        this.transform.localScale = originalSize;
        //this.transform.parent = player.gameObject.transform;

        this.GetComponent<Attractable>().setAttractable(true);
        Invoke("ResetCollider", 1f);    //prevents being stuck in player collider....most of the time
    }

    public bool GetGrabbed()
    {
        return isGrabbed;
    }

    public void ResetSize()
    {
        print("Reset");
        this.transform.localScale = originalSize;
    }
    void ResetCollider()
    {
        Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), player.GetComponent<CapsuleCollider>(), false);
    }
}
