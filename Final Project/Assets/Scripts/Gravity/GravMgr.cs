using UnityEngine;
using System.Collections;
/// <summary>
/// 
/// </summary>

public class GravMgr : MonoBehaviour 
{
	public Camera cam;
	public GameObject lastObjectHit;
	bool isOff = false;
    public static GameObject currentPlanet;                              //initialized in Gravity2 at startup
    public LayerMask ground;
  
    public  float lookDistance = 10;
    public static bool isGazeDelay = false;
    bool doOnce = true;
    public static Vector3 attractionPoint;                           

	// Use this for initialization
	void Start () 
	{
        
		//InvokeRepeating ("RayCaster", .1f,  1f / 60);
        Glow.lookDist = lookDistance;
	}
	
	// Update is called once per frame
	void RayCaster () //points at whatever you look at
	{
		RaycastHit hit;
		Ray ray = Camera.main.ViewportPointToRay( new Vector3(0.5f, 0.5f, 0));

        if (!isGazeDelay && Physics.Raycast(ray, out hit, lookDistance, ground))                                //use SphereCast instead
        {
            //print("hit" + hit.transform.gameObject.name);                                                       //prints what the raycaster is hitting
            GameObject objectHit = hit.transform.gameObject;

            if (Input.GetButtonDown("Jump")) 
            {
//IMPORTANT, SETS A NEW ATTRACTION POINT 
                attractionPoint = hit.point;            
            }
          
 //IMPORTANT FOR TRANSITION OF PLANET           
            ObjectIDAndSelect(objectHit);
            

        }
        else
        {
            if (isGazeDelay && doOnce)
            {
                doOnce = false;
                StartCoroutine(GazeCooldown());    
 
            }
                
            
            ObjectIDAndSelect(null);
        }
	}

	void ObjectIDAndSelect(GameObject go)
	{
        
		if (lastObjectHit == null)                  //when we first see an object, set it up for selection
        {
           // print("space");
			lastObjectHit = go;
			isOff = false;
		} 
		else if (go == lastObjectHit) 
		{
            if (go != currentPlanet && 
                (go.tag == "Element" || go.tag == "Bridge" || go.tag == "CubeBridge") )
            {
               // print("Charging");
                go.GetComponentInChildren<Glow>().isOver = true;    //makes planet glow in selection
            }
                 
			
            isOff = false;

		} 
		else if(lastObjectHit != go)
		{
            //print("missed");
            if (lastObjectHit != null &&  (lastObjectHit.tag == "Element" || lastObjectHit.tag == "Bridge" || lastObjectHit.tag == "CubeBridge"))
                lastObjectHit.GetComponentInChildren<Glow>().isOver = false;
			
            isOff = true;
            ObjectCheck(go);
		}

		
		lastObjectHit = go;




	}

    IEnumerator GazeCooldown()
    {
        yield return new WaitForSeconds(Glow.gazeDelayTime);
        isGazeDelay = false;
        doOnce = true;
    }

    void ObjectCheck(GameObject go)
    {


        if (go != null && 
            isOff && 
            lastObjectHit.GetComponent<Glow>() != null &&
            go.GetComponent<Glow>() == null)
        {
            lastObjectHit.GetComponent<Glow>().isOver = false;
        }
        isOff = false;

    }



    //might need this
	void Timer()
	{
		StartCoroutine ("RoundTimer");
	}

	IEnumerable RoundTime(int roundlength)
	{
		do 
		{
			//getcompoinent<timer>().text = roundlength;
			roundlength -= 1;
			yield return new WaitForSeconds(1);
		}
		while(roundlength > 0);
	}
}
