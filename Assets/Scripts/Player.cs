using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	// Controls the driving of the car

	float maxMoveSpeed = 10;
	public float currentMoveSpeed = 0;
	float acceleration = 1;
	float turnSpeed = 0.2f;

	//float maxTurnAngle = 45;
	//float drag = 0.1f;

	// 0 is forward
	// + is to the right
	// - is to the left
	public float currentTurning = 0;
	public float maxTurning = 2;

	public AnimationCurve turnCurve;

	//Rigidbody rbody;

	//public GameObject frontLeftTire;
	//public GameObject frontRightTire;

	float timeToResetSteering = 1.5f;
	float timeLastSteered = 0;

	float timeToResetSpeed = 1.0f;
	float timeLastAccel = 0;

	// Use this for initialization
	void Start () {
		//rbody = GetComponent<Rigidbody> ();
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

		// can't rotate like this
		// then you can rotate in place while not moving
		//transform.Rotate(new Vector3(0, turnSpeed * Input.GetAxisRaw("Horizontal"), 0));

		//oldPos = transform.position;
		//transform.position = Vector3.MoveTowards (transform.position, transform.position + frontLeftTire.transform.forward, currentMoveSpeed * Time.fixedDeltaTime);
		// move forwards
		transform.position = Vector3.MoveTowards (transform.position, transform.position + transform.forward, currentMoveSpeed * Time.fixedDeltaTime);

		// physics
		//rbody.AddForce(transform.forward * currentMoveSpeed);
		//rbody.velocity = transform.forward * Vector3.Dot (rbody.velocity, transform.forward);

		//transform.rotation = Quaternion.FromToRotation (Vector3.forward, transform.position - oldPos);
		//transform.rotation = Quaternion.FromToRotation (transform.forward, new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, currentMoveSpeed));
		//transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.forward, frontLeftTire.transform.forward), 1);
		//transform.rotation = Quaternion.AngleAxis (maxTurnAngle * currentTurning * currentMoveSpeed, Vector3.up);
		//transform.rotation = Quaternion.FromToRotation(transform.forward, transform.position - oldPos);
		//transform.rotation = Quaternion.FromToRotation(transform.forward, ((transform.position + frontLeftTire.transform.forward) - rearAxle.position)*currentMoveSpeed);

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
			col.gameObject.GetComponent<Mage>().CrashInto(col.transform.position - transform.position);
		}
	}
}
