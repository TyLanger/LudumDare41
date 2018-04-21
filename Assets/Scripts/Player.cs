using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	// Controls the driving of the car

	public float maxMoveSpeed = 10;
	public float currentMoveSpeed = 0;
	float acceleration = 0.5f;
	float turnSpeed = 0.2f;

	//float maxTurnAngle = 45;
	//float drag = 0.1f;

	// 0 is forward
	// + is to the right
	// - is to the left
	public float currentTurning = 0;
	public float maxTurning = 2;

	public AnimationCurve turnCurve;

	Rigidbody rbody;

	//public GameObject frontLeftTire;
	//public GameObject frontRightTire;

	float timeToResetSteering = 1.5f;
	float timeLastSteered = 0;

	float timeToResetSpeed = 1.0f;
	float timeLastAccel = 0;

	Vector3 oldForward;

	// Use this for initialization
	void Start () {
		rbody = GetComponent<Rigidbody> ();
		oldForward = transform.forward;
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		// turning
		if (Input.GetAxisRaw ("Horizontal") == 0) {
			// not turning
			currentTurning = Mathf.Lerp (currentTurning, 0, Mathf.Clamp01((Time.time - timeLastSteered) / timeToResetSteering));
		} else {
			timeLastSteered = Time.time;
		}


		currentTurning += Input.GetAxisRaw ("Horizontal") * turnSpeed * (maxTurning - Mathf.Abs(currentTurning));
		currentTurning = Mathf.Clamp (currentTurning, -maxTurning, maxTurning);

		//frontLeftTire.transform.localRotation = Quaternion.AngleAxis (maxTurnAngle * currentTurning, Vector3.up);
		//frontRightTire.transform.localRotation = Quaternion.AngleAxis (maxTurnAngle * currentTurning, Vector3.up);

		//frontRightTire.transform.localRotation = Quaternion.FromToRotation (transform.forward, new Vector3 (Mathf.Cos(Mathf.Deg2Rad * maxTurnAngle * currentTurning), 0, Mathf.Sin(Mathf.Deg2Rad * maxTurnAngle * currentTurning)));
		//frontRightTire.transform.localRotation = Quaternion.FromToRotation (new Vector3 (1, 0, 0), new Vector3 (0, 0, 0));



		if (Input.GetAxisRaw ("Vertical") == 0) {
			// not accelerating
			// shouldn't be lerp
			// should be based on how high it is
			// cut it in half every frame?
			currentMoveSpeed = Mathf.Lerp (currentMoveSpeed, 0, Mathf.Clamp01((Time.time - timeLastAccel) / timeToResetSpeed));
		} else {
			timeLastAccel = Time.time;
		}
		// increase or decrease current moveSpeed

		currentMoveSpeed += Input.GetAxisRaw ("Vertical") * acceleration * (maxMoveSpeed - Mathf.Abs(currentMoveSpeed));
		currentMoveSpeed = Mathf.Clamp (currentMoveSpeed, -maxMoveSpeed, maxMoveSpeed);

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
			transform.Rotate (Vector3.up, currentTurning);
			//rbody.AddTorque (transform.up * currentTurning * 0.2f);
		}


	}

	/*
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position + Vector3.up, Vector3.up + transform.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));

		//Gizmos.color = Color.blue;
		//Gizmos.DrawLine (frontLeftTire.transform.position, frontLeftTire.transform.position + frontLeftTire.transform.forward);


		Vector3 from = new Vector3 (1, 0, 0);
		Vector3 to = new Vector3 (1, 0, 0);
		Gizmos.color = Color.green;
		Gizmos.DrawLine (transform.position, transform.position + Quaternion.FromToRotation (from, to).eulerAngles);
	}*/

	void OnTriggerEnter(Collider col)
	{

		if (col.tag == "Enemy") {
			// kill enemy
			col.gameObject.GetComponent<Mage>().CrashInto((col.transform.position - transform.position).normalized, currentMoveSpeed);
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.transform.tag == "Wall") {
			// add a force based on the perpendicular component of the player forward vector
			// in the direction away from the wall
			// that may work for glancing blows, but does it work for head on collisions?
			rbody.AddForce (col.transform.position - transform.position);
		}
	}
}
