using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a trigger border around a planet used to change the gravitational pull from one planet to another in the throw puzzle
public class GravityTransition : MonoBehaviour
{
    Gravity2 planet;

    void Start()
    {
        planet = this.transform.parent.GetComponent<Gravity2>();
    }
    
    // Start is called before the first frame update
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Ball")  //transitions gravitational attraction to this planet if ball crosses trigger
        {
            //print("transition hit");
            Gravity2.TransitionPlanetGravity(coll.gameObject.GetComponent<BallGrab>().getStartingPlanet(), planet.gameObject, coll.gameObject);
        }
    }

 

    // Update is called once per frame
    void Update()
    {
        
    }
}
