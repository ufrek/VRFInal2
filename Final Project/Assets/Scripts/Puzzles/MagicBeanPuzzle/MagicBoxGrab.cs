using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//picks up and puts down magic boxes. they jump when attached...unsure of why...
public class MagicBoxGrab : ExtendedGrab
{
    Vector3 originalSize;
    Vector3 originalPosition;
    [SerializeField]
    Vector3 growSize;
    Color originalColor;
    GameObject player;
    bool isGrabbed = false;
    bool isSnapped = false;

    [SerializeField]
    MagicPotTrigger snapZone = null;


    // Start is called before the first frame update
    void Start()
    {
        originalPosition = this.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        originalSize = this.transform.localScale;
        originalColor = this.GetComponent<Renderer>().material.color;


    }

    public Vector3 getOriginalSize()
    {
        return originalSize;
    }

    public override void OnGrab()
    {
        if (snapZone.GetSnapped() == true && this.gameObject == snapZone.getSnappedObject())                  //makes sure to re-enable gravity and physics
        {
            snapZone.UnsnapObject();
        }

        print("grab");
        isGrabbed = true;
        //this.transform.parent = player.gameObject.transform;
        //this.transform.localScale = originalSize;  //puts back to normal size seed
        GrabMgr.AddGrabbed(this.gameObject);
        Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), player.GetComponent<CapsuleCollider>(), true);
        this.GetComponent<Attractable>().setAttractable(false);
    }
    public override void OnRelease()
    {

        print("release");
        isGrabbed = false;
        GrabMgr.RemoveGrabbed(this.gameObject);
        this.transform.localScale = originalSize;               //maybe change this
        //this.transform.parent = player.gameObject.transform;

        this.GetComponent<Attractable>().setAttractable(true);
        Invoke("ResetCollider", 1f);    //prevents being stuck in player collider....most of the time
    }

    public void GrowObject()
    {
        this.transform.localScale = growSize;
    }

    public Color GetColor()
    {
        return originalColor;
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

    public void ResetPosition()
    {
        this.transform.position = originalPosition;
    }
    void ResetCollider()
    {
        Physics.IgnoreCollision(this.GetComponent<BoxCollider>(), player.GetComponent<CapsuleCollider>(), false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
