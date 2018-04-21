using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float moveSpeed = 1;

	public float timeAlive = 10;

	// Use this for initialization
	void Start () {
		Invoke ("DestroyProjectile", timeAlive);
	}

	// Update is called once per frame
	void FixedUpdate () {
		transform.position = Vector3.MoveTowards (transform.position, transform.position + transform.forward, moveSpeed * Time.fixedDeltaTime);
	}

	void DestroyProjectile()
	{
		Destroy (this.gameObject);
	}

	void OnTriggerEnter(Collider col)
	{
		// they also hit each other when spawned
		if (col.tag == "Enemy" || col.tag == "Projectile") {
			// don't collide with the enemies
			//return;
		} else {
			// hit player, do damage
			// hit anything (except caster), die
			if (col.tag == "Player") {
				Debug.Log ("Player hit");
			}
			DestroyProjectile ();
		}
	}
}
