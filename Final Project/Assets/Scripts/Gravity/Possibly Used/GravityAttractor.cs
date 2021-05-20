using UnityEngine;
using System.Collections;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = -9.8f;

    public void Attract(Rigidbody  body)
    {
        Vector3 gravityUp = (body.position - this.transform.position).normalized;
        Vector3 localUp = body.transform.up;

        //applies gravity
        body.AddForce(gravityUp * gravity);
        //realign body
        body.rotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
    }




}
//***********************BASIC TEST SCRIPT****************
/*  public float gravity = -10;
  Vector3 bodyUp;
   public void Attract(Transform body)           //called by everybody
   {

       Vector3 targetDirection = (body.position - this.transform.position.normalized);        //direction between body and the center of the planet
       bodyUp = body.transform.up;

       //body.rotation  =  Quaternion.FromToRotation(bodyUp, targetDirection) * body.rotation; 
         Quaternion initialRot  =  Quaternion.FromToRotation(bodyUp, targetDirection) * body.rotation;                //rotation from one vector to another vector
       Vector3 fixedRot = initialRot.eulerAngles - new Vector3(15.5f, 0.1f, -1.5f);
       body.rotation = Quaternion.Euler(fixedRot);
     
        
        
        
       body.GetComponent<Rigidbody>().AddForce(targetDirection * gravity);
   }
*/