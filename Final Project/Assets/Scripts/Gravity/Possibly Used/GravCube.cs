using UnityEngine;
using System.Collections;

public class GravCube : MonoBehaviour 
{
    GameObject player;
   public static bool isAnchored = false;
   public float gravCubeOffset = 3;                                                 //modify this to alter the amount of correction when jumping around corners
   Vector3 lastPos;
	// Use this for initialization
	void Start () 
    {
        player = GameObject.FindGameObjectWithTag("Player");
	}
    void FixedUpdate()
    {
        if (!isAnchored && FirstPersonController.isGrounded)
        {
            lastPos = this.transform.position;
            this.GetComponent<Rigidbody>().MovePosition(player.transform.position - (gravCubeOffset * player.transform.up));
        }
        else
        {
            CheckJump();

            this.GetComponent<Rigidbody>().MovePosition(lastPos);
        }



        //this.transform.position = player.transform.position - (2 * player.transform.up);
    }

    void CheckJump()
    {
            isAnchored = true;
        



     
            if (FirstPersonController.isGrounded)
                isAnchored = false;
    }
	// Update is called once per frame
    void OnCollisionEnter(Collision other)
    {
      
       // if (other.gameObject.tag == "Cube Bridge")
            print("hit");
         isAnchored = true;
        StartCoroutine(Release());
    }

    IEnumerator Release()
    {
        yield return new WaitForSeconds(1.5f);
        isAnchored = false;
    }

}
