using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
/// <summary>
///  selection sound from: http://www.freesound.org/people/fins/sounds/191589/
/// </summary>

public class Glow : MonoBehaviour
{
    public bool isOver = false;
    public bool isSelected = false;
   // public Renderer[] renderers;
    public float chargeGrain = .01f;
    public float minCharge = 0;
    public float maxCharge = .008f;
    public float charge = 0;
    public float chargeTime = .1f;
    public float currentGrain = 0;
    public int dischargeSpeed = 8;
    public bool isFullyCharged = false;
    public GameObject player;
   // public AudioClip sCharging;
    //public AudioClip sFailCharge;
    public AudioClip sSelectionSound; // lock-on sound
    public AudioClip transitionSound;
    public AudioSource localSound;
    public float vol = .9f;

    public bool __________________;

  
   protected bool doOnce = true; //used to call the charge corouting only once

    public Color selectionColor = new Color(0, 255, 55, 103);
    public Material m_PlaneetSelected;                                  //SET IN INSPECTOR
    public Material m_PlanetNormal;
    public float switchDelayTime = 1f;
    public static float gazeDelayTime = 3;

    public bool ________________;
    public float minDecelSmoothTime = .25f;
    public float maxDecelSmoothTime = .75f;
    public float minSmoothVelocityDelay = .5f;
    public float maxSmoothVeloityDelay = 2;
    public float minVelocity = 3;
    public float maxVelocity = 10;


    public static float lookDist;
    // Use this for initialization
    public void Awake()
    {
        localSound = this.GetComponent<AudioSource>();
        


    }

    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Selection();

        if (isSelected) //the object is selected and waiting to be fired
        {
            if (Input.GetButtonDown("Jump"))                                                                        //currently testing
            {
                GameObject currentPlanet = GravMgr.currentPlanet;
                if (currentPlanet.tag == "CubeBridge")
                {
                    GravMgr.currentPlanet.GetComponentInChildren<GravCube>().enabled = false;
                    BoxCollider[] colliders = GravMgr.currentPlanet.GetComponentsInChildren<BoxCollider>();
                    foreach (BoxCollider b in colliders)
                    {
                        if(b.tag != "CubeBridge")
                            b.enabled = false;
                    }
                }
                else if (currentPlanet.tag == "Bridge")
                {
                    GravMgr.currentPlanet.GetComponentInChildren<GravCube>().enabled = false;
                }
               
                GravMgr.isGazeDelay = true;
               // currentPlanet.GetComponentInParent<GravityInfluence>().enabled = false;
              
         

                FailSelection();
                //currentPlanet.GetComponent<Renderer>().material = m_PlanetNormal;
                localSound.PlayOneShot(transitionSound, 1);
                //Invoke("GravSwitchDelay", switchDelayTime);
                GravSwitch();
 
                
             
                
            }
        }

    }

    void GravSwitch()
    {
        GameObject prevPlanet = GravMgr.currentPlanet;                                                              //needed to  prevent element gravity sticking
        

        GravMgr.currentPlanet.GetComponentInChildren<Gravity2>().isInGravityPull = false;                           
            GravMgr.currentPlanet.layer = LayerMask.NameToLayer("Default");

            GravMgr.currentPlanet = this.gameObject;                                                                    //turns off one planet's gravity and truns the other on
		if (GravMgr.currentPlanet.tag == "Element") {
			GravMgr.currentPlanet.GetComponent<Gravity2> ().isCircleTransitioning = true;
		}

		print ("GravSwitch " + GravMgr.currentPlanet.name);

        GravMgr.currentPlanet.layer = LayerMask.NameToLayer("Ground");
       // GravMgr.currentPlanet.GetComponentInParent<GravityInfluence>().enabled = true;
        this.gameObject.GetComponentInChildren<Gravity2>().isInGravityPull = true;


        if (GravMgr.currentPlanet.tag == "CubeBridge")
        {
            //GravMgr.currentPlanet.GetComponentInChildren<GravCube>().enabled = false;
            BoxCollider[] colliders = GravMgr.currentPlanet.GetComponentsInChildren<BoxCollider>();
            StartCoroutine(EngageGravityCube(colliders));

        }
               
        GravMgr.currentPlanet.GetComponent<Gravity2>().isCircleTransitioning = true;                                    //makes you flip smoothly
        
		StartCoroutine(TransitionGravity());
               


       
    

        StartCoroutine(ResetGravLayer(prevPlanet));

        Vector3 attractionPoint = GravMgr.attractionPoint;                                                              //for some reason, this works a lot better outside of the coroutine, as in it prints  values more reliably
        float planetDistance = Vector3.Distance(player.transform.position, attractionPoint);                            //determines the correct values to be used when smoothing motion between planets
        float iFactor = planetDistance / lookDist;
        iFactor = Mathf.Clamp01(iFactor);

       float  decelSmoothTime = Mathf.Lerp(minDecelSmoothTime, maxDecelSmoothTime, iFactor);
        float smoothVelocityDelay = Mathf.Lerp(minSmoothVelocityDelay, maxSmoothVeloityDelay, iFactor);
        float vel = Mathf.Lerp(maxVelocity, minVelocity, iFactor);
       
        print("decelTime = " + decelSmoothTime);
        print("smoothVelDelay = " + smoothVelocityDelay);
        print("MinVelocity= " + vel);  

        StartCoroutine(TransitionSpeed(attractionPoint, decelSmoothTime,  planetDistance, smoothVelocityDelay, iFactor, vel));
   
    }

    public IEnumerator TransitionSpeed(Vector3 attractionPoint, float decelSmoothTime, float planetDistance, float smoothVelocityDelay, float iFactor, float vel)
    {
        
       
        print("Distance: " + planetDistance);

     
      
        yield return new WaitForSeconds(smoothVelocityDelay);
       
        GravMgr.currentPlanet.GetComponent<Gravity2>().isVelocityInNeedOfSmoothing = true;
        
    
        float currentDecelTime = 0;
        Vector3 speed = player.GetComponent<Rigidbody>().velocity;
        Vector3 moveDir = player.GetComponent<Rigidbody>().velocity.normalized;
        Vector3 playerPos = player.transform.position;
        Vector3 attractionDir = (attractionPoint - playerPos).normalized;
        while (true)                                                                                    //I'm damping between 2 direction vectors to gradually change the position of the gravitation to 
        {

            Vector3 tempVel = player.GetComponent<Rigidbody>().velocity;

            float interpolatingFactor = currentDecelTime / decelSmoothTime;
            float newDirX = Mathf.SmoothDamp(moveDir.x, attractionDir.x, ref tempVel.x, decelSmoothTime);
            float newDirY = Mathf.SmoothDamp(moveDir.y, attractionDir.y, ref tempVel.y, decelSmoothTime);
            float newDirZ = Mathf.SmoothDamp(moveDir.z, attractionDir.z, ref tempVel.z, decelSmoothTime);


            Vector3 newDir = new Vector3(newDirX, newDirY, newDirZ);

            Vector3 move = Vector3.Slerp(speed, newDir*vel, interpolatingFactor);
         

            player.GetComponent<Rigidbody>().velocity = move;
            currentDecelTime += Time.deltaTime;

            if (currentDecelTime > decelSmoothTime)
            {
                GravMgr.currentPlanet.GetComponent<Gravity2>().isVelocityInNeedOfSmoothing = false;
                break;
            }
            

            //if (PlayerController.isGrounded)//(dist < speedSmoothThreshold)
            //{
            //    print("smoothing gravity");
            //    //player.GetComponent<Rigidbody>().velocity = Vector3.zero;


            //   //// GravMgr.currentPlanet.GetComponent<Gravity2>().isVelocityInNeedOfSmoothing = true;
            //   // float decelTime = .5f;
            //   // float currentDecelTime = 0;
            //   // Vector3 speed = player.GetComponent<Rigidbody>().velocity;
               
            //   // while (true)
            //   // {
            //   //     float interpolatingFactor = currentDecelTime/decelTime; 
            //   //     Vector3 move = Vector3.Slerp(speed, Vector3.zero, interpolatingFactor);

            //   //     transform.position -= move;
            //   //     currentDecelTime += Time.deltaTime;

            //   //     if (dist < 1)
            //   //     {
            //   //         //GravMgr.currentPlanet.GetComponent<Gravity2>().isVelocityInNeedOfSmoothing = false; 
            //   //         break;
            //   //     }
                        

            //   //     yield return new WaitForSeconds(1/60);
            //   // }
               
            //    break;
            //}
            yield return new WaitForSeconds(1/60);
        }
    }

    public IEnumerator BridgeTransition(float delay)                                              //despite it saying 0 references, it is in fact used in PlayerController Initiallization to find the beginning planet
    {

        yield return new WaitForSeconds(delay);
        while (true)
        {
            if (PController.isGrounded)
            {
               // print("reset");
                GravMgr.currentPlanet.GetComponent<Gravity2>().isBridgeTransitioning = false;
                break;
            }

            yield return new WaitForSeconds(1/60);
        }
    }  

    IEnumerator TransitionGravity()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            if (PController.isGrounded)
            {
               // print("run");

                GravMgr.currentPlanet.GetComponent<Gravity2>().isCircleTransitioning = false;
                break;
            }
            yield return new WaitForSeconds(1/60);
        }
    }

    IEnumerator ResetGravLayer(GameObject prevPlanet)
    {
         yield return new WaitForSeconds(.5f);
         while (true)
         {
             if (PController.isGrounded)
             {
                 prevPlanet.layer = LayerMask.NameToLayer("Ground");
                 break;
             }
             yield return new WaitForSeconds(.1f);                                                                      //this is not refreshed at 60 times per second, it doesn't need to be since rayDelay offsets this
         }
    }
    IEnumerator EngageGravityCube(BoxCollider[] colliders)
    {
        yield return new WaitForSeconds(.1f);
        while (true)
        {
            if (PController.isGrounded)
            {

                foreach (BoxCollider b in colliders)
                {
                    if (b.gameObject.tag == "GravCube")
                        b.enabled = false;
                    else
                        b.enabled = true;
                }

                GravMgr.currentPlanet.GetComponentInChildren<GravCube>().enabled = true;
                print(GravMgr.currentPlanet.name);
                break;
            }
            yield return new WaitForSeconds(1/60);
        }
      
    }

    public void  Selection()
    {
        if (isOver && doOnce)
        {
            isSelected = true;                                                                                  //prevents latency in jump controls, can remove if you want to charge up for a jump
           if(GravMgr.currentPlanet != this.gameObject)
                 ChargeSelection();
            doOnce = false;
        }
        else if (!isOver && !doOnce)
        {
            //resets so chargeSelection can be called again
            FailSelection();
         
            doOnce = true;
            
        }

        if (isFullyCharged)
        {
            ItemSelected(); // makes a sound and changes color of outline
            isFullyCharged = false;
        }

    }

  



         

 

    public void ItemSelected()
    {
            this.GetComponent<Renderer>().material.SetColor("_OutlineColor", selectionColor);
        
        localSound.PlayOneShot(sSelectionSound, 1);
        isSelected = true;
    }


    public void ChargeSelection()
    {
        //localSound.PlayOneShot(sCharging, 1);
        StartCoroutine(ChargeLaser());
    }

    IEnumerator ChargeLaser()
    {
        //constantly checks if is over, so only need to call once
 
            //do
            //{
            //    if (isOver)
            //    {
            //        this.GetComponent<Renderer>().material.SetFloat("_Outline", charge);
            //        currentGrain += chargeGrain;
            //        charge = Mathf.Lerp(minCharge, maxCharge, currentGrain);
            //        yield return new WaitForSeconds(chargeTime);
            //    }
            //    else
            //        break;
            //}
            //while (charge < maxCharge);
        yield return new WaitForSeconds(chargeTime);
        charge = maxCharge;
            if (charge >= maxCharge)
            {
                isFullyCharged = true;
               	this.GetComponent<Renderer>().material = m_PlaneetSelected;
            }
           // print(this.gameObject.name + " is Selected");                             //print which object we selected

           
       
    }

    public void FailSelection()
    {
           // localSound.Stop();                                                        //might want to put in fail sounds later
           // localSound.PlayOneShot(sFailCharge, 1);
            isSelected = false;
           

        StartCoroutine(DischargeLaser());
    }

    IEnumerator DischargeLaser()
    {
            //do
            //{
            //    if (!isSelected)
            //    {
            //        this.GetComponent<Renderer>().material.SetFloat("_Outline", charge);
            //        currentGrain -= chargeGrain * dischargeSpeed;
            //        charge = Mathf.Lerp(minCharge, maxCharge, currentGrain);
            //        yield return new WaitForSeconds(chargeTime);
            //    }
            //    else
            //        break;
            //}
            //while (charge > minCharge);

            yield return new WaitForSeconds(chargeTime);
            charge = minCharge;
            currentGrain = minCharge;
            this.GetComponent<Renderer>().material = m_PlanetNormal;
   }
}
