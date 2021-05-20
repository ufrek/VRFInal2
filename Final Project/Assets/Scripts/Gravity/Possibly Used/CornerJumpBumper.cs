using UnityEngine;
using System.Collections;

public class CornerJumpBumper : MonoBehaviour
{
    GameObject player;
    public float generalBumpForce = 200;
    public BumpDirection forceDirection;
    public float directionalForce = 400;
    static bool doOnce = true;
    public float waitTime = .5f;
    // Use this for initialization
    Vector3 SupplementalDirection;          //this is added to help with smoothing

    public enum BumpDirection
    {
        up,
        down,
        left,
        right,
        forward,
        back
    };
 
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }


    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            if (doOnce && !FirstPersonController.isJumping)
            {
                FirstPersonController.isJumping = false;
                //print("cpornerbump");
                Rigidbody body = player.GetComponent<Rigidbody>();
                Vector3 direction = player.GetComponent<FirstPersonController>().moveDir;
                if (direction == Vector3.zero)
                    direction = FirstPersonController.prevMov;

                float tempForce = generalBumpForce;
                float tempDirForce = directionalForce;
                if (FirstPersonController.prevAmount.magnitude == 0f)
                {

                    tempForce = 00;
                    tempDirForce = 0;

                }
                else  if (FirstPersonController.prevAmount.magnitude < 1)
                {
                    waitTime = 1;
                    print("push Back");
                    tempForce = -1;
                    tempDirForce = -1;
                }
                else if (FirstPersonController.prevAmount.magnitude < 2f)
                {
                    waitTime = 1;
                    print("weaker");
                    tempForce = generalBumpForce * 2 + 400;
                    tempDirForce = directionalForce * 5 + 600;
                
                }
                else if (FirstPersonController.prevAmount.magnitude < 4.5f)
                {
                    waitTime = 2f;
                    print("weak");
                    tempForce = generalBumpForce * 2 + 200;
                    tempDirForce = directionalForce * 3 + 200;

                    
                }
               

                   
                                                                                  //additional force direction to help correct the player
                switch (forceDirection)
                {
                    case BumpDirection.up:
                        SupplementalDirection = this.transform.up;
                        break;
                    case BumpDirection.down:
                        SupplementalDirection = -this.transform.up;
                        break;
                    case BumpDirection.left:
                        SupplementalDirection = -this.transform.right;
                        break;
                    case BumpDirection.right:
                        SupplementalDirection = this.transform.right; 
                        break; 
                    case BumpDirection.forward:
                        SupplementalDirection = this.transform.forward;
                        break;
                    case BumpDirection.back:
                        SupplementalDirection = -this.transform.forward;
                        break;
                }
                body.AddForce((direction * tempForce) +
                               (SupplementalDirection * tempDirForce));
                doOnce = false;
                StartCoroutine(BumpCoolDown(waitTime));
            } 
        }
    }

    IEnumerator BumpCoolDown(float coolDown)
    {
        yield return new WaitForSeconds(coolDown);
        doOnce = true;
    }
}
