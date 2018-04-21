using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour {

	public enum AttackType {PulseRing, Spiral }

	public AttackType attackType;

	public float timeBetweenAttacks;
	float timeOfNextAttack = 0;

	int current = 0;

	public GameObject projectile;

	bool alive = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (alive) {
			if (Time.time > timeOfNextAttack) {
				timeOfNextAttack = Time.time + timeBetweenAttacks;

				switch (attackType) {

				case AttackType.PulseRing:
				// fire right
					FireProjectile (0);
					FireProjectile (1);
					FireProjectile (2);
					FireProjectile (3);
					FireProjectile (4);
					FireProjectile (5);
					FireProjectile (6);
					FireProjectile (7);

					break;

				case AttackType.Spiral:
					FireProjectile (current);
					current = (current + 1) % 8;
					break;
				}

			}
		}
	}

	void FireProjectile(int direction)
	{
		switch (direction) {
		case 0:
			Instantiate (projectile, transform.position, Quaternion.Euler (new Vector3 (0, 0, 0)));
			break;
		case 1:
			Instantiate (projectile, transform.position, Quaternion.Euler (new Vector3 (0, 45, 0)));
			break;
		case 2:
			Instantiate (projectile, transform.position, Quaternion.Euler (new Vector3 (0, 90, 0)));
			break;
		case 3:
			Instantiate (projectile, transform.position, Quaternion.Euler (new Vector3 (0, 135, 0)));
			break;

		case 4:
			Instantiate (projectile, transform.position, Quaternion.Euler (new Vector3 (0, 180, 0)));
			break;

		case 5:
			Instantiate (projectile, transform.position, Quaternion.Euler (new Vector3 (0, 225, 0)));
			break;

		case 6:
			Instantiate (projectile, transform.position, Quaternion.Euler (new Vector3 (0, 270, 0)));
			break;

		case 7:
			Instantiate (projectile, transform.position, Quaternion.Euler (new Vector3 (0, 315, 0)));
			break;

		}
	}

	void Death()
	{
		// despawn yourself
		// called after it has been crashed into
		Destroy(this.gameObject);
	}

	public void CrashInto(Vector3 forceDirection)
	{
		// die and go off flying in forceDirection
		alive = false;
		// rework timing later
		Invoke ("Death", 5);
	}
}
