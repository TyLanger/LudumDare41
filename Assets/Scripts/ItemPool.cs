using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour {

	int numToSpawn = 35;
	public int totalSpawned = 0;

	public GameObject projectile;
	Queue<GameObject> itemPool;

	// Use this for initialization
	void Start () {
		itemPool = new Queue<GameObject> (75);
		GameObject copy;
		for (int i = 0; i < numToSpawn; i++) {
			copy = Instantiate (projectile, transform.position, transform.rotation);
			copy.GetComponent<Projectile> ().Initialize (this);
			itemPool.Enqueue (copy);
			totalSpawned++;
		}
	}

	public void SpawnProjectile(Vector3 position, Quaternion rotation)
	{
		GameObject item;
		if (itemPool.Count > 0) {
			item = itemPool.Dequeue ();
		} else {
			totalSpawned++;
			item = Instantiate (projectile, transform.position, transform.rotation);
			item.GetComponent<Projectile> ().Initialize (this);
		}
		item.transform.position = position;
		item.transform.rotation = rotation;
		item.GetComponent<Projectile>().Activate();
	}

	public void Reclaim(GameObject item)
	{
		item.transform.position = transform.position;
		itemPool.Enqueue (item);
	}

}
