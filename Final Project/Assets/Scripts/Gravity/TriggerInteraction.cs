//Goal:Detach to a target point on trigger, allow free walkaround and Re-attach to the earlier position on trigger.

//NOT USED. SUPER BUGGY
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class TriggerInteraction : MonoBehaviour {

	public GameObject target; //Object you want to move
	public AudioClip trigger; //Audio clip to play on trigger
	//public Vector3 vantagePoint; //Set the initial vantage point relevant to the scene
	AudioSource audio;
	GameObject detached_planet; //Detached planet
	private Vector3 savedPosition; //Detached position
	private Vector3 startPosition; //Where we start
	private Vector3 endPosition; //Where to end
	public float distance = 100f; //Distance = Calculate CurrentPos to TargetPos
	private float lerpTime = 1.62f; //Time taken from start to end
	private float currentLerpTime = 0; //This will update the lerp time
	static public bool detached_flag = false; //Check if currently detached
	static public bool moving = false; //Check if currently moving

	//public float mouseSensitivityX = 1;
    //public float mouseSensitivityY = 1;
    //public Vector3 moveDir;
    public static Vector3 prevMov;
    public static bool isGrounded;
    Vector3 smoothMoveVelocity;
    float verticalLookRotation;
    Transform cameraT;
    Rigidbody rigidBody;
	public static Vector3 prevAmount;  
   // public Vector3 moveAmount;

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;		// Disable screen dimming:
	}
	
	// Update is called once per frame
	void Update () {
		//not used, changes planets
		/*if (Input.GetButtonDown ("Fire1") && moving == false) {
			
			//audio = GetComponent<AudioSource> ();
			//audio.clip = trigger;
			//audio.Play();
			startPosition = target.transform.position; //transform.position gives the current position of the object
			currentLerpTime = 0;
			GetComponent<AudioSource>().PlayOneShot(trigger, 1); //make this some kind of sound
			Debug.Log(startPosition);
			if (detached_flag == false) {
				savedPosition = startPosition;
				endPosition = target.transform.position + Vector3.up * distance;
				//endPosition = vantagePoint;
				detached_flag = true;

				GameObject currentPlanet = GravMgr.currentPlanet;
				detached_planet = currentPlanet;
				if (currentPlanet.tag == "CubeBridge") {
					GravMgr.currentPlanet.GetComponentInChildren<GravCube> ().enabled = false;
					BoxCollider[] colliders = GravMgr.currentPlanet.GetComponentsInChildren<BoxCollider> ();
					foreach (BoxCollider b in colliders) {
						if (b.tag != "CubeBridge")
							b.enabled = false;
					}					
				} else {
					GravMgr.currentPlanet.GetComponentInChildren<Gravity2> ().isInGravityPull = false;
				}
			} 
			else if (detached_flag == true) {
				endPosition = savedPosition;
				detached_flag = false;

				GameObject currentPlanet = detached_planet;
				if (GravMgr.currentPlanet.tag == "CubeBridge") {
					BoxCollider[] colliders = GravMgr.currentPlanet.GetComponentsInChildren<BoxCollider> ();
					StartCoroutine (EngageGravityCube (colliders));
				} else {
					GravMgr.currentPlanet.GetComponentInChildren<Gravity2> ().isInGravityPull = true;
				}
			}
			moving = true;
		}
		if (moving == true) {
			//Debug.Log ("Started Moving");
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime >= lerpTime) {
				//currentLerpTime = lerpTime;
				Debug.Log("Stopped Moving");
				Debug.Log (endPosition);
				moving = false;
			}
			float weight = currentLerpTime / lerpTime;
			target.transform.position = Vector3.Lerp (startPosition, endPosition, weight);  
		}*/
	}

	IEnumerator EngageGravityCube(BoxCollider[] colliders) //this basically only turns the colliders back on again when the player lands on the cube bridge
	{
		yield return new WaitForSeconds(.1f);
		while (true)
		{
			if (FirstPersonController.isGrounded)
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
			yield return new WaitForSeconds(.1f);
		}
	}
}

