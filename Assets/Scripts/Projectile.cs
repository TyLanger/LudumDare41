using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float moveSpeed = 1;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void FixedUpdate () {
		transform.position = Vector3.MoveTowards (transform.position, transform.position + transform.forward, moveSpeed * Time.fixedDeltaTime);
	}

	void OnTriggerEnter(Collider col)
	{
		// hit player, do damage
		// hit anything (except caster), die
	}
}
