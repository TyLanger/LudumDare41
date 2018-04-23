using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
	// Controls the driving of the car

	public float maxMoveSpeed = 10;
	public float currentMoveSpeed = 0;
	float acceleration = 0.1f;
	float deceleration = 0.05f;
	float turnSpeed = 0.2f;

	public Transform lookPoint;
	//float maxTurnAngle = 45;
	//float drag = 0.1f;

	// 0 is forward
	// + is to the right
	// - is to the left
	public float currentTurning = 0;
	public float maxTurning = 2;

	public AnimationCurve turnCurve;
	public CameraFollow cameraFollow;
	//Rigidbody rbody;

	//public GameObject frontLeftTire;
	//public GameObject frontRightTire;

	float timeToResetSteering = 1.5f;
	float timeLastSteered = 0;

	float timeToResetSpeed = 3.0f;
	float timeLastAccel = 0;

	Vector3 oldForward;

	float startTime = 0;
	float endTime = 0;
	float penaltyTime = 0;
	float hitPenalty = 1;
	int timesHit = 0;
	int enemiesCrushed = 0;

	bool playerControl = true;
	float accelInput = 0;
	float brakeInput = 0;
	float horInput = 0;

	public Menu menu;

	AudioSource audioSource;
	public AudioClip[] clips;
	float timeClipStarted = 0;

	public AudioSource revAudio;
	AudioSource lowRevAudio;
	public AudioClip accelClip;
	public AudioClip lowAccelClip;
	public float minVolume = 0.4f;
	public float maxVolume = 0.9f;
	public float minPitch = 0.75f;
	public float maxPitch = 1f;

	int gear = 0;
	float timeBetweenGears = 1;
	float timeOfNextShift = 0;

	// Use this for initialization
	void Start () {
		//rbody = GetComponent<Rigidbody> ();
		oldForward = transform.forward;
		//cameraFollow = FindObjectOfType<CameraFollow> ();
		audioSource = GetComponent<AudioSource>();
		/*
		lowRevAudio = gameObject.AddComponent<AudioSource> ();
		lowRevAudio.clip = lowAccelClip;
		lowRevAudio.loop = true;
		lowRevAudio.volume = 0;
		lowRevAudio.pitch = 1.2f;
		if (!lowRevAudio.isPlaying) {
			lowRevAudio.Play ();
		}
		*/
	}

	public void InitPlayer(Menu _menu, CameraFollow cam)
	{
		// mixing style....
		menu = _menu;
		cameraFollow = cam;

		cameraFollow.player = lookPoint;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (playerControl) {
			accelInput = Input.GetAxisRaw ("Acceleration");
			brakeInput = Input.GetAxisRaw ("Brake");
			horInput = Input.GetAxisRaw ("Horizontal");
		} 
		/* Reload the level at the end
		else {
			if (Input.GetButtonDown ("Fire1")) {
				// reset
				SceneManager.LoadSceneAsync(0);
			}
		}*/

		if (transform.position.y < -1 && playerControl) {
			// player fell off map
			playerControl = false;
			OutsideMap ();
		}

		/*
		if (Input.GetButton ("Jump")) {
			// add some trauma to camera
			cameraFollow.AddScreenShake();
		}*/

		// turning
		if (horInput == 0) {
			// not turning
			currentTurning = Mathf.Lerp (currentTurning, 0, Mathf.Clamp01((Time.time - timeLastSteered) / timeToResetSteering));
		} else {
			timeLastSteered = Time.time;
		}


		currentTurning += horInput * turnSpeed * (maxTurning - Mathf.Abs(currentTurning));
		currentTurning = Mathf.Clamp (currentTurning, -maxTurning, maxTurning);

		//frontLeftTire.transform.localRotation = Quaternion.AngleAxis (maxTurnAngle * currentTurning, Vector3.up);
		//frontRightTire.transform.localRotation = Quaternion.AngleAxis (maxTurnAngle * currentTurning, Vector3.up);

		//frontRightTire.transform.localRotation = Quaternion.FromToRotation (transform.forward, new Vector3 (Mathf.Cos(Mathf.Deg2Rad * maxTurnAngle * currentTurning), 0, Mathf.Sin(Mathf.Deg2Rad * maxTurnAngle * currentTurning)));
		//frontRightTire.transform.localRotation = Quaternion.FromToRotation (new Vector3 (1, 0, 0), new Vector3 (0, 0, 0));



		if (accelInput == 0) {
			// not accelerating
			// shouldn't be lerp
			// should be based on how high it is
			// cut it in half every frame?
			currentMoveSpeed = Mathf.Lerp (currentMoveSpeed, 0, Mathf.Clamp01((Time.time - timeLastAccel) / timeToResetSpeed));
		} else {
			timeLastAccel = Time.time;
		}

		// increase or decrease current moveSpeed
		// slow down
		// -1 * 0.1 * 15-15 = 0
		//currentMoveSpeed += Input.GetAxisRaw ("Vertical") * acceleration * (maxMoveSpeed - Mathf.Abs(currentMoveSpeed));
		currentMoveSpeed += accelInput * acceleration * (maxMoveSpeed - Mathf.Abs(currentMoveSpeed));
		currentMoveSpeed -= brakeInput * deceleration * (0.5f*maxMoveSpeed - Mathf.Abs(currentMoveSpeed));

		currentMoveSpeed = Mathf.Clamp (currentMoveSpeed, -maxMoveSpeed*0.5f, maxMoveSpeed);
		
		// Accel sound
		if (currentMoveSpeed > 0 && accelInput > 0) {
			// moving forward and pressing the gas
			//audioSource.clip = accelClip;
			// if speed is close to the max, stop updating volumes

			if (!revAudio.isPlaying) {
				revAudio.Play ();
			} else {
				//Debug.Log ("Change Volume");
				if (currentMoveSpeed < (maxMoveSpeed - 1.0f)) {
					revAudio.volume = Mathf.Lerp (minVolume, maxVolume, (currentMoveSpeed / maxMoveSpeed));
					revAudio.pitch = Mathf.Lerp (minPitch, maxPitch, (currentMoveSpeed / maxMoveSpeed));
				} else {
					//revAudio.volume = Mathf.Lerp (maxVolume, 0, (1.0f - (maxMoveSpeed - currentMoveSpeed)));
					//lowRevAudio.volume = Mathf.Lerp (0, 0.7f, (1.0f - (maxMoveSpeed - currentMoveSpeed)));
					if (Time.time > timeOfNextShift) {
						revAudio.Stop ();
						// shift gears (in sound only)
						//minPitch = Mathf.Clamp (minPitch - 0.05f, 0.3f, 1);
						//maxPitch = Mathf.Clamp (maxPitch - 0.05f, 0.55f, 1);
						//revAudio.pitch = minPitch;
						gear++;
						//Debug.Log ("Shift");
						timeOfNextShift = Time.time + timeBetweenGears * gear;
						revAudio.volume = minVolume;
						revAudio.pitch = minPitch;
						revAudio.Play ();
					}
					revAudio.volume = Mathf.Lerp (minVolume, maxVolume, ((timeBetweenGears*gear) - (timeOfNextShift - Time.time)) / (timeBetweenGears*gear));
					revAudio.pitch = Mathf.Lerp (minPitch, maxPitch, ((timeBetweenGears*gear) - (timeOfNextShift - Time.time)) / (timeBetweenGears*gear));
				}

			}

		} else {
			if (revAudio.isPlaying) {
				gear = 0;
				timeOfNextShift = 0;
				revAudio.Stop ();
			}
		}

		// move forwards
		//transform.position = Vector3.MoveTowards (transform.position, transform.position + oldForward, currentMoveSpeed * Time.fixedDeltaTime);

		// This makes the car quite slippery
		transform.position = Vector3.MoveTowards (transform.position, transform.position + (transform.forward + oldForward * Mathf.Abs(currentMoveSpeed) *0.5f), currentMoveSpeed * Time.fixedDeltaTime);
		oldForward = (transform.forward + oldForward * Mathf.Abs(currentMoveSpeed) * 0.5f).normalized;

		// square currentMoveSpeed to make it very slippery
		//transform.position = Vector3.MoveTowards (transform.position, transform.position + (transform.forward + oldForward * currentMoveSpeed*currentMoveSpeed *0.5f), currentMoveSpeed * Time.fixedDeltaTime);
		//oldForward = (transform.forward + oldForward * currentMoveSpeed*currentMoveSpeed * 0.5f).normalized;

		// rotate
		// only when moving
		if (Mathf.Abs(currentMoveSpeed) > 0.4f) {
			// multiply by the sign of moveSpeed so reverse reverses the turning
			transform.Rotate (Vector3.up, currentTurning * Mathf.Sign(currentMoveSpeed));
			//rbody.AddTorque (transform.up * currentTurning * 0.2f);
		}


	}

	public void StartRace()
	{
		// the start line calls this when the player crosses the start line
		startTime = Time.time;
		// start a coroutine to have a timer counting
		menu.RaceStarted();
	}

	void OutsideMap()
	{
		menu.RaceEnded (0, 0, 0);
	}

	public void EndRace()
	{
		playerControl = false;
		accelInput = 0;
		brakeInput = 0;
		horInput = 0;
		//oldForward = Vector3.up * -0.5f;
		// the end line calls this when the player touches the end line
		endTime = Time.time;

		Debug.Log ("Time: " + (endTime - startTime));
		Debug.Log ("Penalty time: " + penaltyTime);
		Debug.Log ("But you crushed " + enemiesCrushed + " enemies!");
		Debug.Log ("Total time: " + ((endTime-startTime)+penaltyTime-enemiesCrushed));

		menu.RaceEnded ((endTime - startTime), timesHit, enemiesCrushed);
	}

	public void HitByProjectile()
	{
		// lose hp?
		// time penalty?
		penaltyTime += hitPenalty;
		cameraFollow.AddScreenShake(0.5f);
		timesHit++;
		menu.PlayerHit (timesHit);
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine (transform.position, transform.position + oldForward);

		//Gizmos.color = Color.blue;
		//Gizmos.DrawLine (frontLeftTire.transform.position, frontLeftTire.transform.position + frontLeftTire.transform.forward);


	}

	void OnTriggerEnter(Collider col)
	{

		if (col.tag == "Enemy") {
			// kill enemy
			if (!audioSource.isPlaying) {
				audioSource.clip = clips [Random.Range (0, clips.Length)];
				audioSource.Play ();
				timeClipStarted = Time.time;
			} else if((Time.time - timeClipStarted) > 0.1f){
				audioSource.clip = clips [Random.Range (0, clips.Length)];
				audioSource.Play ();
				timeClipStarted = Time.time;
			}
			col.gameObject.GetComponent<Mage>().CrashInto((col.transform.position - transform.position).normalized, currentMoveSpeed);
			// add screen shake
			cameraFollow.AddScreenShake(0.3f);
			enemiesCrushed++;
			menu.EnemyCrushed (enemiesCrushed);
		}
	}

	/*
	void OnCollisionEnter(Collision col)
	{
		if (col.transform.tag == "Wall") {
			// add a force based on the perpendicular component of the player forward vector
			// in the direction away from the wall
			// that may work for glancing blows, but does it work for head on collisions?
			Debug.Log("Hit a wall");
			//rbody.AddForce ((col.transform.position - transform.position) * 15);
		}
	}
	*/
}
