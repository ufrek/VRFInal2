using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(AudioSource))]
//handles player locomotion around the scene
public class PController : MonoBehaviour
{
	[SerializeField]
	XRNode inputSource;
	XRRig rig;
	Vector2 inputAxis;

	public float walkSpeed = 4;
	public float detach_walkSpeed = 50;
	public float jumpForce = 220;
	public Vector3 moveDir;
	public Vector3 prevMov;
	public static bool isGrounded;
	public Vector3 moveAmount;
	public LayerMask groundedMask;
	//public Camera cam;
	public static bool isJumping = false;
	Vector3 prevAmount;
	public static bool isMoving;
	public AudioClip carbonSound;
	public AudioClip hydrogenSound;
	public AudioClip oxygenSound;
	public AudioClip bondSound;
	public float soundVolume = 1;

	AudioSource localSound;
	Vector3 smoothMoveVelocity;
	Transform cameraTransform;
	Transform playerTransform;
	Rigidbody playerRigidBody;
	GameObject previousPlanet;

	GameObject[] grabbedObjects = new GameObject[2];

	bool rayDelay = false;

	void Awake()
	{
		cameraTransform = Camera.main.transform;
		playerRigidBody = this.GetComponent<Rigidbody>();
		playerTransform = this.GetComponent<Transform>();
		localSound = this.GetComponent<AudioSource>();
		

	}

	void Start()
	{
		rig = GetComponent<XRRig>();

		//finds every object in the radius of it's transform to determine planet we are standing on
		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 6, groundedMask);
		float tempDistance = 200; //could mess with this if working with different size planets
		foreach (Collider col in hitColliders)
		{
			if (col.gameObject.tag.Equals("Element") && Vector3.Distance(this.transform.position, col.transform.position) < tempDistance)
			{
				tempDistance = Vector3.Distance(this.transform.position, col.transform.position);
				GravMgr.currentPlanet = col.gameObject.GetComponentInChildren<Gravity2>().gameObject;

				/*if (GravMgr.currentPlanet.tag == "CubeBridge")                                                                  //prevents you from flying off the cube at initialization
				{
					GravMgr.currentPlanet.GetComponent<Gravity2>().isBridgeTransitioning = true;
					StartCoroutine(GravMgr.currentPlanet.GetComponent<Glow>().BridgeTransition(1));
				}*/
				print(GravMgr.currentPlanet.name + "curPl");
				break;
			}
		}


		if (GravMgr.currentPlanet.GetComponent<Gravity2>().shape == Gravity2.planetShape.cubeBridge)
			GravMgr.currentPlanet.GetComponent<Gravity2>().gravCube.GetComponent<GravCube>().enabled = true;
		print(GravMgr.currentPlanet.name + "is current planet");
	}

	void Update()
	{
		InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
		device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

		prevAmount = moveAmount;
		//Get the input
		//float inputX = Input.GetAxisRaw("Horizontal");
		//float inputZ = Input.GetAxisRaw("Vertical");



		//Get movement direction
		Vector3 forwardDir = Camera.main.transform.forward.normalized;
		forwardDir = forwardDir * inputAxis.y;// + Camera.main.transform.right.normalized * inputX;
		Vector3 leftDir = Vector3.Cross(Camera.main.transform.up, forwardDir).normalized;   //left axis is cross product of camera direction and forward
		leftDir = leftDir * inputAxis.x;

		if (isGrounded && (forwardDir != Vector3.zero || leftDir != Vector3.zero))
			prevMov = forwardDir + leftDir;
		/*if (TriggerInteraction.detached_flag == true && TriggerInteraction.moving == false)
		{
			//Navigation code here
			isGrounded = true;

			moveAmount = forwardDir * detach_walkSpeed;
		}*/
		else if (TriggerInteraction.detached_flag == false)
		{ }

		Vector3 targetMoveAmount;
		if (inputAxis.y > 0)
		{
			targetMoveAmount = (forwardDir + leftDir) * walkSpeed;
		}
		else
		{
			targetMoveAmount = (forwardDir - leftDir) * walkSpeed;
		}
		if (isGrounded)                                                                                             //player keeps moving in mid-air based on previous input rather than current input
			moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .1f);
		else
			moveAmount = prevAmount;

		if (moveAmount.magnitude < .3f)
			isMoving = false;
		else
			isMoving = true;


		//if(Input.GetButtonDown("Fire1"))                                                            //map this to contrller later
		//{
		//    Gravity2 nearestNode = FindNearestNode();
		//    if (isWalkOn)
		//    {
		//        isWalkOn = false;
		//        nearestNode.isInGravityPull = true;
		//    }
		//    else
		//    {
		//        isWalkOn = true;
		//        nearestNode.isInGravityPull = false;
		//    }

		//}



		//jump nmechanics
		/*if (Input.GetButtonDown("Jump"))                                                    //map this to controller later
		{
			if (isGrounded)
			{
				isJumping = true;
				playerRigidBody.AddForce(transform.up * jumpForce);
				//I can stop what ever music is playing when the user jumps
				if (localSound.isPlaying)
					localSound.Stop();
			}
		}*/

		//grounded check
		Ray ray = new Ray(transform.position, -transform.up);                   //may need to change this in my setup to child capsule
		RaycastHit hit;

		if (!rayDelay && Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
		{
			//print("grounded");
			isJumping = false;
			isGrounded = true;

		}
		else
		{
			isGrounded = false;
		}
		// the following is used for the looping sound when your on a planet.
		if (previousPlanet == null && GravMgr.currentPlanet != null)
		{           //instantiate the previous planet variable
			previousPlanet = GravMgr.currentPlanet;
			PlayWalkingLoop();
		}
		else if (!GameObject.Equals(previousPlanet, GravMgr.currentPlanet))
		{ //check if a new planet has been selected
			PlayWalkingLoop();
			previousPlanet = GravMgr.currentPlanet;
		}

	}

	//disables groundCheck to allow for initial jump action and provides planetary gravity when jumping rather than smoothing gravity
	public void SuspendGroundCheck()
	{
		StartCoroutine(SuspendRayCheck());
	}

	IEnumerator SuspendRayCheck()
	{
		rayDelay = true;
		isGrounded = false;
		yield return new WaitForSeconds(.1f);
		rayDelay = false;
	}
	void FixedUpdate()
	{
		Vector3 newPos = Vector3.Slerp(this.transform.position, (playerRigidBody.position + moveAmount), Time.fixedDeltaTime);
		playerRigidBody.position = newPos;
		// playerRigidBody.MovePosition(playerRigidBody.position + moveAmount * Time.fixedDeltaTime); //felt a bit more jerky
	}

	public Vector2 GetLeftAxis()
	{
		return new Vector2(inputAxis.x, inputAxis.y);
	}

	void PlayWalkingLoop()
	{
		string planetName = GravMgr.currentPlanet.name;

		switch (planetName.Substring(planetName.Length - 1))
		{
			case "C":
				//insert loop for carbon sound
				localSound.PlayOneShot(carbonSound, soundVolume);
				break;
			case "H":
				//insert loop for hydrogen sounds
				localSound.PlayOneShot(hydrogenSound, soundVolume);
				break;
			case "O":
				//insert logic
				localSound.PlayOneShot(oxygenSound, soundVolume);
				break;
		}
		//add the sounds for the bridge

	}
}
