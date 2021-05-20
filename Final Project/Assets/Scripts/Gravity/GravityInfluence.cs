using UnityEngine;
using System.Collections;

//when the game starts, this pulls the player to the nearest current planet
public class GravityInfluence : MonoBehaviour {
   public Gravity2 localGrav;

	// Use this for initialization
	void Start () 
    {
        localGrav = this.GetComponentInChildren<Gravity2>();
        StartCoroutine(DisableCollider());  //the gravitational transition is still a bit weird, disable trigger after a while
	}

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(1);
		if(this.gameObject.name.StartsWith("Bridge") || this.gameObject.name.StartsWith("CubeBridge")) //long cyliunder gravity setup
        {
            this.GetComponent<CapsuleCollider>().enabled = false;
            //this.GetComponentInChildren<CapsuleCollider>().enabled = true;
        }
		else if (this.gameObject.name.StartsWith("Element")) //sphere planet gravity setup
        {
            this.GetComponent<SphereCollider>().enabled = false;
        }
      
    }
	
	// Update is called once per frame
	void Update () 
    {
      
	
	}

    void OnTriggerEnter(Collider coll)                                           //might need an OnTriggerStay() as well, not sure
    {
        if (coll.gameObject.tag == "Player")
        {
            localGrav.isInGravityPull = true;
 
        }

   
           

    }

    //void OnTriggerStay()
    //{
    //    localGrav.isInGravityPull = true;
    //}

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
            localGrav.isInGravityPull = false;
    }
}
