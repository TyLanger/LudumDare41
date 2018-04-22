using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float moveSpeed = 1;

	public float timeAlive = 7;

	// Use this for initialization
	void Start () {
		Invoke ("DestroyProjectile", timeAlive);
	}

	// Update is called once per frame
	void Update () {
		transform.position = Vector3.MoveTowards (transform.position, transform.position + transform.forward, moveSpeed * Time.deltaTime);
	}

	void DestroyProjectile()
	{
		Destroy (this.gameObject);
	}

	void OnTriggerEnter(Collider col)
	{
		// they also hit each other when spawned
		if (col.tag == "Enemy" || col.tag == "Projectile" || col.tag == "Aura") {
			// don't collide with the enemies
			//return;
		} else {
			// hit player, do damage
			// hit anything (except caster), die
			if (col.tag == "Player") {
				//Debug.Log ("Player hit");
				col.GetComponent<Player> ().HitByProjectile ();
			}
			DestroyProjectile ();
		}
	}
}
